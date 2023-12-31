# Written by Jeffrey Bringolf

import datetime
from enum import Enum
from json import dumps
from typing import Union


class Reading:

    class Type(Enum):
        """
        An enum which is used to pick from any of the possible reading types in the system.
        """

        GEO_LOCATION = "Geo-Location"
        PITCH = "Pitch"
        ROLL = "Roll"
        BUZZER = "Buzzer"
        VIBRATION = "Vibration"
        FAN = "Fan"
        SOIL_MOISTURE = "Soil-Moisture"
        WATER_LEVEL = "Water-Level"
        TEMPERATURE = "Temperature"
        HUMIDITY = "Humidity"
        RGB_LED_STICK = "RGB-LED-Stick"
        LUMINOSITY = "Luminosity"
        DOOR_OPENED = "Door-Opened"
        DOOR_LOCKED = "Door-Locked"
        MOTION = "Motion"
        NOISE = "Noise"

    class Unit(Enum):
        """
        An enum which is used to pick from any of the possible reading units in the system
        """
        NONE = None
        DEGREES = "°"
        BOOL = "Bool"
        PERCENTAGE = "%"
        CENTIMETERS = "cm"
        CELCIUS = "°C"
        DECIBEL = "Decibel"
        LUX = "Lux"

        def __str__(self):
            return str(self.value)

    def __init__(self,
                 value: Union[int, float, str],
                 reading_type: Type,
                 reading_unit: Unit) -> None:
        """
        Initializes a new reading.

        Parameters
        ----------
        value: Union[int, float, str]
            The value of this reading generated by a sensor.
        reading_type: Type
            The type of reading this sensor produces.
        reading_unit: Unit
            The unit that the value of this reading is measured in.

        Returns
        -------
        None
        """

        self.reading_type = reading_type
        self.value = value
        self.reading_unit = reading_unit
        self.timestamp = datetime.datetime.now()

    def to_json(self) -> str:
        """
        Creates a json object with the reading value, type, and unit then returns it
        parsed as a string.

        Returns
        -------
        str
            A json object containing the reading value, reading type, and reading unit,
            parsed as a string.
        """

        data = {
            "value": self.value,
            "reading_type": self.reading_type.value,
            "reading_unit": self.reading_unit.value,
            "timestamp": str(self.timestamp)
        }
        return dumps(data)

    def __repr__(self) -> str:
        """
        Returns a string representation of the Reading object.

        Returns
        -------
        str
            A string representation of the Reading object.
        """

        return dumps(self.to_json())
