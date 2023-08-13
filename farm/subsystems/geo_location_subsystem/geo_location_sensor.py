# Written by Jeffrey Bringolf

from time import sleep, time

from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading
from ..helpers.serial_connection import SerialConnection

import serial.tools.list_ports
import pynmea2


class NMEASerialConnection(SerialConnection):
    """
    An extended version of the SerialConnection class which parses serial data
    in an nmea format.
    """

    _DESIRED_NMEA_PREFIX = "$GNGLL"
    TIMEOUT = 4

    def readline(self) -> pynmea2.NMEASentence:
        """
        Reads a line of data coming from the serial port and parses it from nmea data.

        Raises
        ------
        serial.SerialException
            Raised when something went wrong reading from the serial connection.
        pynmea.ParseError
            Raised when the line read from the serial connection is not in proper
            nmea format.
        UnicodeDecodeError
            Raised when the read message could not be decoded properly. Usually happens when
            The device first connects.
        TimeoutError
            Raised when the Serial Connection fails to receive data for a set amount of time.

        Returns
        -------
        pynmea2.NMEASentence
            An NMEASentence object with location data read from the serial connection.
            Returns None if the nmea line is not the type in the desired prefix.
        """

        try:

            # If it doesn't read in the timeout amount of time just raise a pynmea.ParseError
            # and let the parent catch it.
            start_time = time()

            # Read lines from the serial port until the right line is found
            # and parse it as an NMEASentence
            line = ""
            while not line.startswith(NMEASerialConnection._DESIRED_NMEA_PREFIX):
                if time() - start_time > NMEASerialConnection.TIMEOUT:
                    raise TimeoutError("Serial connection timed out")
                line = super().readline()

            return pynmea2.parse(line)

        # Added try except for clarity
        except serial.SerialException:
            raise
        except pynmea2.ParseError:
            raise
        except UnicodeDecodeError:
            raise


class GPS(ISensor):

    SERIAL_NAME = "/dev/ttyAMA0"

    def __init__(self, gpio=None) -> None:
        self.serial_connection = NMEASerialConnection(GPS.SERIAL_NAME)
        self._reading_types = [Reading.Type.GEO_LOCATION]
        self._reading_units = [Reading.Unit.NONE]

    def read(self) -> list[Reading]:
        """
        Generates a list of readings based on sensor data.

        Returns
        -------
        list[Reading]
            A list of reading objects generated from sensor data. Returns None
            if the reading from the gps was invalid.

        """
        try:
            data = self.serial_connection.readline()
        except (pynmea2.ParseError, serial.SerialException, UnicodeDecodeError, TimeoutError):
            return []

        return [Reading(GPS._nmea_to_dict(data),
                        self.reading_types[0],
                        self.reading_units[0])
                ]

    @property
    def reading_types(self) -> list[Reading.Type]:
        return self._reading_types

    @property
    def reading_units(self) -> list[Reading.Unit]:
        return self._reading_units

    @classmethod
    def _nmea_to_dict(cls, data: pynmea2.NMEASentence) -> dict:
        """
        Converts the NMEASentence to a dict to use in azure

        Parameters
        ----------
        data: pynmea2.NMEASentence
            The nmea data to be parsed.

        Returns
        -------
        dict
            A dictionary containing {field: value} for each field of the nmea data.

        """

        # Get all the NMEA data in a dictionary
        parsed_data = {field[0]: val for field, val in zip(
            data.fields, str(data).split(",")[1:])}

        # Separate the degrees and seconds from the latitude and longitude
        latitude_degree_end_index = 2
        longitude_degree_end_index = 3
        latitude = {"degrees": parsed_data["Latitude"][:latitude_degree_end_index],
                    "minutes": parsed_data["Latitude"][latitude_degree_end_index:]}
        longitude = {"degrees": parsed_data["Longitude"][:longitude_degree_end_index],
                     "minutes": parsed_data["Longitude"][longitude_degree_end_index:]}

        parsed_data["Latitude"] = latitude
        parsed_data["Longitude"] = longitude

        return parsed_data


if __name__ == "__main__":

    # Call this method if a new gps is attached and you need to find its name.
    def find_all_used_serial_ports():
        ports = serial.tools.list_ports.comports()
        print([port.name for port in ports])

    delay = 1

    gps = GPS()
    while True:
        reading = gps.read()
        print(reading)
        sleep(delay)
