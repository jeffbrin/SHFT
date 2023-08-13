"""
This module defines a Fan class that implements both the IActuator and ISensor interfaces.
It represents a fan actuator device that can be controlled through a GPIO pin and provides 
the current state of the fan. The class also includes methods for controlling the state 
of the fan and validating commands.

Classes:
    Fan: A class that represents a fan actuator device that can be controlled through a 
    GPIO pin and provides the current state of the fan.

Methods:
    control_actuator(command: Command) -> bool: Controls the state of the fan according 
                                to the given command. 
    validate_command(command: Command) -> bool: Validates the given command to ensure it 
                                is valid for the fan actuator.
    read() -> List[Reading]: Returns the current state of the fan.

Attributes:
    reading_type: The type of reading provided by the fan actuator - FAN.
    reading_unit: The unit of measurement for the readings provided by the fan 
                    actuator - BOOL.

Usage:
    fan = Fan(24)
    delay = 5
    on_command = Command(Command.Type.FAN, Command.Unit.BOOL, Fan.State.ON)
    off_command = Command(Command.Type.FAN, Command.Unit.BOOL, Fan.State.OFF)
    try:
        while True:
            print("Turning fan on")
            fan.control_actuator(on_command)
            print("State: ", fan.read()[0].value)
            sleep(delay)

            print("Turning fan off")
            fan.control_actuator(off_command)
            print("State: ", fan.read()[0].value)
            sleep(delay)
    except KeyboardInterrupt:
        # Turn off when done
        fan.control_actuator(off_command)
"""
from gpiozero import DigitalOutputDevice
from time import sleep
from ..interfaces.actuators import IActuator
from ..interfaces.sensors import ISensor
from ..interfaces.command import Command
from ..interfaces.reading import Reading
from enum import Enum


class Fan(IActuator, ISensor):
    """
    Represents a fan actuator device that can be controlled through a GPIO pin.

    Attributes:
    -----------
    fan : DigitalOutputDevice
        A digital output device representing the fan controlled by the actuator.

    Methods:
    --------
    control_actuator(command: Command) -> bool:
        Controls the state of the fan according to the given command.

    validate_command(command: Command) -> bool:
        Validates the given command to ensure it is valid for the fan actuator.

    read() -> List[Reading]:
        Returns the current state of the fan.

    Properties:
    -----------
    reading_type : Reading.ReadingType
        The type of readings provided by the fan actuator.

    reading_unit : Reading.ReadingUnit
        The unit of measurement for the readings provided by the fan actuator.
    """

    class State(Enum):
        """
        Represents the state of the fan actuator, either ON or OFF.
        """
        ON = True
        OFF = False

        @classmethod
        def contains(cls, value: "Fan.State") -> bool:
            return len([x for x in iter(cls) if x.value == value]) > 0

    def __init__(self, gpio: int) -> None:
        """
        Represents the state of the fan actuator, either ON or OFF.
        """
        self.fan = DigitalOutputDevice(gpio)
        self._reading_types = [Reading.Type.FAN]
        self._reading_units = [Reading.Unit.BOOL]

    def validate_command(self, command: Command) -> bool:
        """
        Validates the given command to ensure it is valid for the fan actuator.

        Parameters:
        -----------
        command : Command
            The command to validate for the fan actuator.

        Returns:
        --------
        bool
            True if the command is valid for the fan actuator, False otherwise.
        """
        return Fan.State.contains(command.value) and command.type == Command.Type.FAN

    def control_actuator(self, command: Command) -> bool:
        """
        Controls the state of the fan according to the given command.

        Parameters:
        -----------
        command : Command
            The command to execute on the fan actuator.

        Returns:
        --------
        bool
            True if the command was successfully executed on the fan actuator, False otherwise.
        """
        if not self.validate_command(command):
            return False

        if command.value == Fan.State.ON.value:
            self.fan.on()
        elif command.value == Fan.State.OFF.value:
            self.fan.off()
        else:
            return False

        return True

    def read(self) -> list[Reading]:
        """
        Returns the current state of the fan.

        Returns:
        --------
        List[Reading]
            A list containing a single Reading instance representing the state of the fan.
        """
        return [Reading(self.fan.value == 1, self.reading_types[0], self.reading_units[0])]

    @property
    def reading_types(self) -> Reading.Type:
        """
        The type of readings provided by the fan actuator.
        """
        return self._reading_types

    @property
    def reading_units(self) -> Reading.Unit:
        """
        The unit of measurement for the readings provided by the fan actuator.
        """
        return self._reading_units


if __name__ == "__main__":
    # Example usage
    fan = Fan(24)
    delay = 5
    # turn on command
    on_command = Command(Command.Type.FAN,
                         Command.Unit.BOOL, Fan.State.ON)
    # turn off command
    off_command = Command(Command.Type.FAN,
                          Command.Unit.BOOL, Fan.State.OFF)
    try:
        while True:
            print("Turning fan on")
            fan.control_actuator(on_command)
            print("State: ", fan.read()[0].value)

            sleep(delay)

            print("Turning fan off")
            fan.control_actuator(off_command)
            print("State: ", fan.read()[0].value)
            sleep(delay)

    except KeyboardInterrupt:
        # Turn off when done
        fan.control_actuator(off_command)
