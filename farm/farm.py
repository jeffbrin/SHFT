import asyncio
import json
from time import sleep
from argparse import ArgumentParser

from subsystems.geo_location_controller import GeoLocationSubsystem
from subsystems.security_controller import SecuritySubsystem
from subsystems.plant_controller import PlantSubsystem
from subsystems.interfaces.command import Command
from subsystems.subsystem import Subsystem


class Farm:
    """
    A class which represents a farm container. Stores all the subsystems and the 
    connection to the cloud
    """

    def __init__(self, debug: bool = False) -> None:
        """
        Initializes all the subsystems and the connection to the cloud.

        Parameters
        ----------
        debug: bool
            Represents whether the farm should be run in debug / verbose mode.

        Returns
        -------
        None
        """

        self._subsystems: list[Subsystem] = [
            GeoLocationSubsystem(),
            SecuritySubsystem(),
            PlantSubsystem()
        ]

        self._connection_manager = ConnectionManager(self, debug)
        asyncio.run(self._connection_manager.connect())

    async def start(self) -> None:
        """
        Starts running the farm. The farm will continously read and send telemetry data
        as well as response to direct messages and device twin updates.

        Returns
        -------
        None
        """

        # Wait for the connection manager to be connected
        while not self._connection_manager.is_connected:
            pass

        # Loop while keeping the thread open for device twin logic and direct methods
        async def stdin_listener():
            try:
                while True:
                    # Send Farm Telemetry
                    await self.send_readings()
                    sleep(self._connection_manager._telemetry_interval)
            except KeyError:
                pass
            finally:
                # Always close the connection manager and connection
                await self._connection_manager.close()

        stdin_listener_task = asyncio.create_task(stdin_listener())
        await stdin_listener_task

    async def send_readings(self) -> None:
        """
        Loops through all of the three subsystems and get's their readings.
        Then it appends the readings to it's subsystem by name. And then it cleans 
        and sends the telemetry to the cloud.


        Returns
        -------
        None
        """
        telemetry = {}

        for subsystem in self._subsystems:
            readings = subsystem.read_sensors()

            # https://www.tutorialspoint.com/How-to-get-the-class-name-of-an-instance-in-Python#:~:text=Using%20__class__%20.&text=Python%27s%20__class__%20property,object%27s%20or%20instance%27s%20class%20name.
            #  We will be making the key the name of the class. The link helped
            #  me understand how to do this
            telemetry[subsystem.__class__.__name__] = []
            for reading in readings:
                telemetry[subsystem.__class__.__name__].append(
                    json.loads(reading.to_json()))

        payload = json.dumps(telemetry)

        await self._connection_manager.send_the_d2c_message(payload)

    def control_subsystems(self, command: Command) -> None:
        """
            Controls the actuators associated with the given command.

            Parameters
            ----------
            command: Command
                The command to use to control the actuators.

            Returns
            -------
            None
            """
        for subsystem in self._subsystems:
            subsystem.control_actuators([command])


if __name__ == "__main__":

    # Importing connection manager only in main to avoid circular imports.
    # Connection manager needs farm for type hinting so when it imports farm,
    # this import won't break everything.
    from connection_manager import ConnectionManager
    parser = ArgumentParser("A farming container device used to collect telemetry data "
                            "and respond to messages from a service.")
    parser.add_argument("--debug", action="store_true", help="Indicates that the script "
                        "should be run in debug mode.")
    args = parser.parse_args()

    farm = Farm(args.debug)
    asyncio.run(farm.start())
