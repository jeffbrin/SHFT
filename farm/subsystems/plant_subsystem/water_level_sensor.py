"""
This module defines a WaterLevelSensor class that implements the ISensor interface.
It reads water level data from a water level sensor and returns a Reading object containing the data.

Classes:
    WaterLevelSensor: A class that implements the ISensor interface and reads water level data.

Methods:
    read(): Reads water level data from the sensor and returns a list of Reading objects.

Attributes:
    reading_type: The type of reading to be taken - water level.
    reading_unit: The unit of the reading - centimeters.

Usage:
    sensor = WaterLevelSensor()
    delay = 5
    while True:
        water_level_readings = sensor.read()
        for reading in water_level_readings:
            print(f"Water level: {reading.value}{reading.reading_unit.value}")
        sleep(delay)
"""
from time import sleep
from grove.adc import ADC
from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading
from typing import Optional


class WaterLevelSensor(ISensor):
    """
    A class that represents a water level sensor.

    Attributes:
    -----------
    gpio : Optional[int]
        The GPIO pin number used to connect the sensor. Default is None.
    """

    def __init__(self, gpio: Optional[int] = 2) -> None:
        """
        Initializes a WaterLevelSensor object.

        Parameters:
        -----------
        gpio : Optional[int]
            The GPIO pin number used to connect the sensor. Default is None.
        """

        self.water_level_sensor = ADC(0x04)
        self.water_channel = gpio
        self._reading_types = [Reading.Type.WATER_LEVEL]
        self._reading_units = [Reading.Unit.CENTIMETERS]

    def read(self) -> list[Reading]:
        """
        Reads the water level from the sensor and returns a list of Reading objects.

        Returns:
        --------
        list[Reading]
            A list of Reading objects that represent the water level readings.
        """

        water_value = self.water_level_sensor.read(self.water_channel)
        water_level = round(water_value / 100, 2)
        return [Reading(water_level, self.reading_types[0], self.reading_units[0])]

    @property
    def reading_types(self) -> Reading.Type:
        """
        Returns the ReadingType of the sensor.

        Returns:
        --------
        Reading.Type
            The ReadingType of the sensor.
        """

        return self._reading_types

    @property
    def reading_units(self) -> Reading.Unit:
        """
        Returns the ReadingUnit of the sensor.

        Returns:
        --------
        Reading.Unit
            The ReadingUnit of the sensor.
        """

        return self._reading_units


if __name__ == "__main__":
    # Create a new WaterLevelSensor object
    sensor = WaterLevelSensor()

    # Set the delay time between sensor readings
    delay = 5

    while True:
        # Read the water level from the sensor
        water_level_readings = sensor.read()

        # Print the water level reading to the console
        for reading in water_level_readings:
            print(
                f"water level: {reading.value}{reading.reading_unit.value}")

        # Wait for the specified delay time
        sleep(delay)
