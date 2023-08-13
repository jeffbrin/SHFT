"""
This module defines a SoilMoistureSensor class that implements the ISensor interface.
It reads soil moisture data from a GroveSoilMoistureSensor and returns a Reading object 
containing the data.

Classes:
    SoilMoistureSensor: A class that implements the ISensor interface and reads soil 
    moisture data.

Methods:
    read(): Reads soil moisture data from the sensor and returns a list of Reading objects.

Attributes:
    reading_type: The type of reading to be taken - soil moisture.
    reading_unit: The unit of the reading - percentage.

Usage:
    sensor = SoilMoistureSensor()
    delay = 5
    while True:
        soil_moisture_readings = sensor.read()
        for reading in soil_moisture_readings:
            print(f"Moisture level: {reading.value}{reading.reading_unit.value}")
        sleep(delay)
"""
from time import sleep
from grove.adc import ADC
from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading
from typing import Optional


class SoilMoistureSensor(ISensor):
    """
    A class that represents a soil moisture sensor.

    Attributes:
    -----------
    gpio : Optional[int]
        The GPIO pin number used to connect the sensor. Default is None.
    """

    def __init__(self, gpio: Optional[int] = 0) -> None:
        """
        Initializes a SoilMoistureSensor object.

        Parameters:
        -----------
        gpio : Optional[int]
            The GPIO pin number used to connect the sensor. Default is None.
        """

        self.soil_moisture_sensor = ADC(0x04)
        self.moisture_channel = gpio
        self._reading_types = [Reading.Type.SOIL_MOISTURE]
        self._reading_units = [Reading.Unit.PERCENTAGE]

    def read(self) -> list[Reading]:
        """
        Reads the moisture level from the sensor and returns a list of Reading objects.

        Returns:
        --------
        list[Reading]
            A list of Reading objects that represent the moisture level readings.
        """

        moisture_value = self.soil_moisture_sensor.read(self.moisture_channel)
        moisture_percentage = round(moisture_value / 1023 * 100, 2)
        return [Reading(moisture_percentage, self.reading_types[0], self.reading_units[0])]

    @property
    def reading_types(self) -> Reading.Type:
        """
        Returns the ReadingType of the sensor.

        Returns:
        --------
        Reading.Type
            The Type of the sensor.
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
    # Create a new SoilMoistureSensor object
    sensor = SoilMoistureSensor()

    # Set the delay time between sensor readings
    delay = 5

    while True:
        # Read the moisture level from the sensor
        soil_moisture_readings = sensor.read()

        # Print the moisture level reading to the console
        for reading in soil_moisture_readings:
            print(
                f"Moisture level: {reading.value}{reading.reading_unit.value}")

        # Wait for the specified delay time
        sleep(delay)
