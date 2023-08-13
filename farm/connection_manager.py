"""
This module defines two classes: ConnectionConfig and ConnectionManager taken partially 
from Connected Objects' Assignment 2. The ConnectionConfig class represents all the 
necessary information required to connect the client to the cloud gateway. The 
ConnectionManager class is a wrapper for all the connection logic and includes 
functionality to connect to the IoT hub, handle message and device twin update handlers 
and handle direct method requests.

Classes:
ConnectionConfig: Represents all information required to successfully connect client 
to cloud gateway.

ConnectionManager: A wrapper for all logic related to the connection to the IoT hub.

Methods:
_load_connection_config(): Loads connection credentials from a .env file in the project's 
                            top-level directory. Returns a ConnectionConfig object with 
                            the configuration information.

connect(): Connects to the IoT hub using connection credentials and sets up message and 
            device twin update handlers.

_direct_method_request_handler(): A callback method for direct method requests. Deals with 
                                    direct method requests by calling the appropriate 
                                    hardware method and responds accordingly.

is_connected: Indicates whether the ConnectionManager is currently connected to the IoT hub.

close(): Gracefully closes the connection.

Usage:
conManager = ConnectionManager()
await conManager.connect()

def stdin_listener():
    while True:
        selection = input("Press Q to quit\n")
        if selection == "Q" or selection == "q":
            print("Quitting...")
            break

loop = asyncio.get_running_loop()
await loop.run_in_executor(None, stdin_listener)

await conManager.close()
"""
import asyncio
from os import environ, getenv
from os.path import isfile
from typing import Dict, List, Any

from subsystems.interfaces.command import Command
from farm import Farm

from azure.iot.device.aio import IoTHubDeviceClient
from azure.iot.device import MethodResponse, MethodRequest
from azure.iot.device.exceptions import ClientError
from dotenv import load_dotenv


class ConnectionConfig:
    """
    Represents all information required to successfully connect client to cloud gateway.
    """

    # Key names for configuration values inside .env file. See .env.example
    # Constants included as static class property
    DEVICE_CONN_STR = "IOTHUB_DEVICE_CONNECTION_STRING"

    def __init__(self, device_str: str) -> None:
        """
        Initializes the connection config object

        Parameters
        ----------
        device_str: str
            The connection string for an IoT device to connect to.

        Returns
        -------
        None

        """

        self._device_connection_str = device_str


class ConnectionManager:
    """A wrapper for all logic related to the connection to the IoT hub."""

    DEFAULT_TELEMETRY_INTERVAL = 5

    def __init__(self, farm: Farm, debug: bool = False) -> None:
        """
        Constructor for ConnectionManager and initializes an internal cloud gateway client.

        Parameters
        ----------
        farm: Farm
            The farm this connection manager is working within.
        debug: bool
            Indicates whether the connection manager should be run in debug / verbose mode.

        Returns
        -------
        None
        """

        self._telemetry_interval = ConnectionManager.DEFAULT_TELEMETRY_INTERVAL
        self._debug = debug
        self._connected = False
        self._config: ConnectionConfig = self._load_connection_config()
        self._client = IoTHubDeviceClient.create_from_connection_string(
            self._config._device_connection_str)
        self._farm = farm

    def _load_connection_config(self) -> ConnectionConfig:
        """
        Loads connection credentials from .env file in the project's top-level directory.

        Returns
        ------
           ConnectionConfig
            An Object with configuration information loaded from .env file.
        """

        # Look for .env
        if not isfile('.env'):
            raise FileNotFoundError(
                ".env file does not exist in the current directory.")

        # Remove previously loaded connection string. This is necessary in case the .env file
        # changes at runtime
        try:
            environ.pop(ConnectionConfig.DEVICE_CONN_STR)
        except:
            pass

        # Load .env and get the connection string
        load_dotenv()
        connection_string = getenv(ConnectionConfig.DEVICE_CONN_STR)
        if connection_string is None:
            raise RuntimeError(
                ".env file does not contain a device connection string.")

        return ConnectionConfig(connection_string)

    async def connect(self) -> None:
        """
        Connects to cloud gateway using connection credentials and setups message and
        device twin update handlers.

        Returns
        -------
        None
        """

        await self._client.connect()
        self._connected = True

        if self._debug:
            print("Connected")

        # Initialize the the telemetry interval
        TELEMETRY_INTERVAL = "telemetryInterval"
        twin = await self._client.get_twin()
        try:
            self._telemetry_interval = twin["desired"][TELEMETRY_INTERVAL]
        except KeyboardInterrupt:
            pass

        # Define behavior for receiving a twin patch
        def twin_patch_handler(patch):
            # update telemetry interval if need be
            if TELEMETRY_INTERVAL in patch:
                self._telemetry_interval = patch[TELEMETRY_INTERVAL]
                # _ = stopped static type checker from complaining.
                asyncio.run(self._client.patch_twin_reported_properties(
                    {TELEMETRY_INTERVAL: self._telemetry_interval}))
                if self._debug:
                    print("New telemetry interval: {} seconds".format(
                        self._telemetry_interval))

        # Set the twin update handler on the client
        self._client.on_twin_desired_properties_patch_received = twin_patch_handler

        # Set the method request handler on the client
        self._client.on_method_request_received = self._direct_method_request_handler

    # Define behavior for handling methods
    async def _direct_method_request_handler(self, method_request: MethodRequest) -> None:
        """
        The callback method for direct method requests. Deals with direct method requests
        by calling the appropriate hardware method and responds accordingly.

        Parameters
        ----------
        method_request: MethodRequest
            The direct method request coming from the IoT hub.

        Returns
        -------
        None

        """

        # A dictionary to lookup direct messages from the iot hub.
        # Each value is a tuple containing (payload, device_method)
        # where device_method is the method which the iot hub wants this
        # device to call.
        method_responses_dict = {
            'is_online': (None, None),
            'buzzer-on': (None, lambda state: self._farm.control_subsystems(Command(
                Command.Type.BUZZER, Command.Unit.BOOL, state)
            )),
            'door-lock': (None, lambda state: self._farm.control_subsystems(Command(
                Command.Type.MICRO_SERVO_MOTOR, Command.Unit.BOOL, state)
            )),
            'fan-on': (None, lambda state: self._farm.control_subsystems(Command(
                Command.Type.FAN, Command.Unit.BOOL, state)
            )),
            'led-on': (None, lambda state: self._farm.control_subsystems(Command(
                Command.Type.RGB_LED_STICK, Command.Unit.BOOL, state)
            )),

        }

        # Determine how to respond to the method request based on the method name
        try:
            payload, method = method_responses_dict[method_request.name]
            status = 200
            if self._debug:
                print("executed method: " + method_request.name)

            if method is not None:
                # Call the method with the value from the payload if the method exists
                # Respond with 400 if the payload is invalid.
                try:
                    state = method_request.payload["value"]
                    method(state)

                    # Display the new state
                    print(f"{method_request.name} set to {state}")
                except KeyError:
                    status = 400
                    payload = {"details": "No value was found in the payload"}
                except Exception as e:
                    status = 400
                    payload = {"details": f"Invalid payload {e}"}
        except KeyError:
            # set response for unknown methods
            status = 400
            payload = {"details": "method name unknown"}
            if self._debug:
                print("executed unknown method: " + method_request.name)

        # Send the response
        method_response = MethodResponse.create_from_method_request(
            method_request, status, payload)
        await self._client.send_method_response(method_response)

    async def send_the_d2c_message(self, telemetry: Dict[str, List[Any]]) -> None:
        """
        Sends telemetry data to the IoT Hub.

        Parameters
        ----------
        telemetry: dict
            A dictionary containing telemetry data.

        Returns
        -------
        None
        """
        if self._debug:
            print(f"Sending Telemetry: {telemetry}")

        await self._client.send_message(telemetry)

    @property
    def is_connected(self) -> bool:
        """
        Indicates whether the connection manager is currently connected to the IoT hub.

        Returns
        -------
        bool
            True if the connection manager is connected, false otherwise.

        """

        return self._client.connected

    async def close(self) -> None:
        """
        Gracefully closes the connection. 
        This should always be called at the end of the script.

        Returns
        -------
        None

        """

        try:
            await self._client.shutdown()
        except (ClientError, AttributeError):
            pass


if __name__ == "__main__":

    DEBUG = False

    async def demo():
        conManager = ConnectionManager(DEBUG)
        await conManager.connect()

        # Define behavior for halting the application
        def stdin_listener():
            while True:
                selection = input("Press Q to quit\n")
                if selection == "Q" or selection == "q":
                    print("Quitting...")
                    break

        # Run the stdin listener in the event loop
        loop = asyncio.get_running_loop()
        await loop.run_in_executor(None, stdin_listener)

        await conManager.close()

    asyncio.run(demo())
