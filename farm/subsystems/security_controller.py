# Written by Jeffrey Bringolf

from time import sleep

from .subsystem import Subsystem
from .interfaces.actuators import Command
from .security_subsystem.SecurityMagneticDoorSensor import SecurityMagneticDoorSensor
from .security_subsystem.SecurityLumonisitySensor import SecurityLumonisitySensor
from .security_subsystem.SecurityMotionSensor import SecurityMotionSensor
from .security_subsystem.SecurityNoiseSensor import SecurityNoiseSensor
from .security_subsystem.SecurityServoActuator import SecurityServoActuator
from .security_subsystem.buzzer import Buzzer


class SecuritySubsystem(Subsystem):

    SERVER_GPIO = 12
    NOISE_SENSOR_BUS = 0x04
    MOTION_SENSOR_GPIO = 16
    MAGNETIC_DOOR_GPIO = 24

    def set_default_periferals(self) -> "SecuritySubsystem":
        buzzer = Buzzer()
        self.set_actuators([
            SecurityServoActuator(SecuritySubsystem.SERVER_GPIO),
            buzzer
        ])
        self.set_sensors([
            SecurityNoiseSensor(SecuritySubsystem.NOISE_SENSOR_BUS),
            SecurityMotionSensor(SecuritySubsystem.MOTION_SENSOR_GPIO),
            SecurityMagneticDoorSensor(SecuritySubsystem.MAGNETIC_DOOR_GPIO),
            SecurityLumonisitySensor(),
            buzzer
        ])

        return self


if __name__ == "__main__":

    TEST_TIME_SLEEP = 2

    """
    Intializing the device controller
    """
    device_manager = SecuritySubsystem()

    while True:

        print("Reading sensors...")
        print(device_manager.read_sensors())
        sleep(TEST_TIME_SLEEP)

        ServoCloseCommand = Command(Command.Type.MICRO_SERVO_MOTOR,
                                    Command.Unit.BOOL,
                                    False)

        ServoOpenCommand = Command(Command.Type.MICRO_SERVO_MOTOR,
                                   Command.Unit.BOOL,
                                   True)

        device_manager.control_actuators(ServoCloseCommand)
        sleep(TEST_TIME_SLEEP)

        device_manager.control_actuators(ServoOpenCommand)
        sleep(TEST_TIME_SLEEP)
