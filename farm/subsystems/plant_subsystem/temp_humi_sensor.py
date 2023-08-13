"""
This module defines the TemperatureHumiditySensor class that implements the ISensor
interface. The class reads temperature and humidity data from a
GroveTemperatureHumidityAHT20 sensor and returns a Reading object containing the data.

Classes:
    TemperatureHumiditySensor: A class that implements the ISensor interface and reads
    temperature and humidity data.

Methods:
    read(): Reads temperature and humidity data from the sensor and returns a list
    of Reading objects.

Attributes:
    reading_types: A list of Reading.Type objects representing the type of reading to be
                     taken - temperature, humidity or both.
    reading_units: A list of Reading.Unit objects representing the unit of the reading -
                    Celsius for temperature, percentage for humidity.

Usage:
    temp_humi_sensor = TemperatureHumiditySensor()
    delay = 3
    while True:
        readings = temp_humi_sensor.read()
        for reading in readings:
            print(
                f"{reading.reading_type.name.capitalize()}: {reading.value:.2f}{reading.reading_unit}")
        sleep(delay)
"""
from time import sleep
from typing import Optional
from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading
from grove.grove_temperature_humidity_aht20 import GroveTemperatureHumidityAHT20


class TemperatureHumiditySensor(ISensor):
    """
    A class that implements the ISensor interface and reads temperature data, humidity
    data or both.
    Methods:
        read(): Reads temperature data, humidity data or both from the sensor and returns
        a list of Reading objects.
    Attributes:
        reading_type: The type of reading to be taken - temperature, humidity or both.
        reading_unit: The unit of the reading - Celsius for temperature, percentage for
        humidity.
    """

    def __init__(self, gpio: Optional[int] = None,
                 reading_types: Optional[list[Reading.Type]] = [Reading.Type.TEMPERATURE, Reading.Type.HUMIDITY]):
        """
        Initializes the TemperatureHumiditySensor class.

        Args:
            gpio (Optional[int]): The GPIO pin number. Defaults to None.
            reading_types (Optional[list[Reading.Type]]): The types of reading to be taken
            - temperature,
                humidity or both. Defaults to both.
        """
        self.sensor = GroveTemperatureHumidityAHT20(0x38, 4)
        self._reading_types = reading_types

        self._reading_units = []
        for reading_type in self._reading_types:
            if reading_type == Reading.Type.TEMPERATURE:
                self._reading_units.append(Reading.Unit.CELCIUS)
            elif reading_type == Reading.Type.HUMIDITY:
                self._reading_units.append(Reading.Unit.PERCENTAGE)
            else:
                raise ValueError(
                    f"Reading type {reading_type} is not supported by the TemperatureHumiditySensor.")

    def read(self) -> list[Reading]:
        """
        Reads temperature data, humidity data or both from the sensor and returns a list
        of Reading objects.

        Returns:
            list[Reading]: A list of Reading objects.
        """
        temperature, humidity = self.sensor.read()
        readings = []
        for reading_type in self._reading_types:
            if reading_type == Reading.Type.TEMPERATURE:
                readings.append(
                    Reading(temperature, Reading.Type.TEMPERATURE, Reading.Unit.CELCIUS))
            elif reading_type == Reading.Type.HUMIDITY:
                readings.append(
                    Reading(humidity, Reading.Type.HUMIDITY, Reading.Unit.PERCENTAGE))

        return readings

    @ property
    def reading_types(self) -> Reading.Type:
        """
        The type of reading to be taken - temperature, humidity or both.
        """
        return self._reading_types

    @ property
    def reading_units(self) -> Reading.Unit:
        """
        The unit of the reading - Celsius for temperature, percentage for humidity.
        """
        return self._reading_units


if __name__ == "__main__":
    # Create a TemperatureHumiditySensor object for temperature readings
    temp_humi_sensor = TemperatureHumiditySensor()

    # Set the delay between readings to 3 seconds
    delay = 3

    # Loop indefinitely
    while True:
        # Get the current readings from the sensor
        readings = temp_humi_sensor.read()
        # Iterate over the list of readings
        for reading in readings:
            # Print out the sensor type (temperature or humidity) and the reading value
            print(
                f"{reading.reading_type.name.capitalize()}: {reading.value:.2f}{reading.reading_unit}")
        # Wait for the specified delay before taking another reading
        sleep(delay)
