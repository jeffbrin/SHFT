# Written by Pleasure, Touched up by Jeffrey

from enum import Enum
from json import dumps
from typing import Union


# command value-> being a string-> Union -> jeff seems good decision
class Command:

    class Type(Enum):
        """
        Multiple enums which can be used to create a command configured to the 
        correct type
        """
        PIR_MOTION_SENSOR = "PIR-Motion-Sensor"
        MAGNETIC_DOOR_SENSOR = "Magnetic-Door-Sensor"
        MICRO_SERVO_MOTOR = "Micro-Servo-Motor"
        LOUDNESS_SENSOR = "Loundness-Sensor"
        BUZZER = "Buzzer"
        FAN = 'Fan'
        RGB_LED_STICK = 'RGB-Led-Stick'

    class Unit(str, Enum):
        """
        Enum to be used correlated to its unit
        """
        OPEN = 'Open'
        CLOSED = 'Closed'
        DB = 'dB'
        MOTION_LIGHT_ON = 'Motion-light-on'
        MOTION_LIGHT_OFF = 'Motion-light-off'
        BOOL = 'Bool'


    def __init__(self, command_type: Type, command_unit: Unit,
                 command_value: Union[int, float, bool, str]) -> None:
        """
        Creates an actuator command from the cloud; IOT hub messsage
        Parameters
        ----------
        command_type: Type
            Type of reading taken 
        command_unit: Unit
            Unit of reading taken
        command_value: Union[int, float, bool, str]
            Value of reading taken.

        Returns
        -------
        None
        """

        self.type = command_type
        self.value = command_value
        self.unit = command_unit

    def __repr__(self) -> str:
        json = dumps(
            {"type": self.type.value, "value": self.value, "unit": self.unit.value})
        return f"(Command: {json})"
        