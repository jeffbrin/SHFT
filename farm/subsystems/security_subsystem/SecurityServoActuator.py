from time import sleep
from gpiozero import Servo
from gpiozero.pins.pigpio import PiGPIOFactory

from ..interfaces.actuators import IActuator
from ..interfaces.command import Command
from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading


from enum import Enum


class SecurityServoActuator(IActuator, ISensor):

    class State(Enum):
        OPEN = True
        CLOSE = False

        @classmethod
        def contains(cls, value: bool) -> bool:
            return value in [member.value for member in cls]

    def __init__(self, gpio: int) -> None:
        self._factory_ = PiGPIOFactory()
        self._servo_ = Servo(gpio, min_pulse_width=0.5/1000,
                             max_pulse_width=2.5/1500,
                             pin_factory=self._factory_)

        self._reading_types = [Reading.Type.DOOR_OPENED]
        self._reading_units = [Reading.Unit.BOOL]

    def control_actuator(self, command: Command) -> bool:
        if self.validate_command(command):
            if command.value == SecurityServoActuator.State.OPEN.value:
                self._servo_.max()
            elif command.value == SecurityServoActuator.State.CLOSE.value:
                self._servo_.min()
            else:
                return False
        return False

    def validate_command(self, command: Command) -> bool:
        return SecurityServoActuator.State.contains(command.value) and command.type == Command.Type.MICRO_SERVO_MOTOR

    def read(self) -> list[Reading]:
        return [Reading(self._servo_.value, self._reading_types[0].value, self._reading_units[0].value)]

    @property
    def reading_types(self) -> list[Reading.Type]:
        return self._reading_types

    @property
    def reading_units(self) -> list[Reading.Unit]:
        return self._reading_units


if __name__ == "__main__":
    servo = SecurityServoActuator(12)

    ServoCloseCommand = Command(Command.Type.MICRO_SERVO_MOTOR,
                                Command.Unit.BOOL,
                                SecurityServoActuator.State.CLOSE)

    ServoOpenCommand = Command(Command.Type.MICRO_SERVO_MOTOR,
                               Command.Unit.BOOL,
                               SecurityServoActuator.State.OPEN)
    while True:
        servo.control_actuator(ServoCloseCommand)
        sleep(1)
        servo.control_actuator(ServoOpenCommand)
        sleep(1)
