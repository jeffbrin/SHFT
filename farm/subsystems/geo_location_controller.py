# Written by Jeffrey Bringolf

from time import sleep

from .subsystem import Subsystem
from .geo_location_subsystem.angle_sensor import AngleSensor
from .geo_location_subsystem.geo_location_sensor import GPS
from .geo_location_subsystem.vibration_sensor import VibrationSensor


class GeoLocationSubsystem(Subsystem):

    def set_default_periferals(self) -> "GeoLocationSubsystem":
        self.set_actuators([
        ]
        )
        self.set_sensors([
            AngleSensor(),
            GPS(),
            VibrationSensor()
        ])

        return self


# Must be run with sudo
if __name__ == "__main__":
    controller = GeoLocationSubsystem().set_default_periferals()
    delay = 2

    buzzer_state = False
    try:
        while True:
            # Toggle the buzzer state and get the right command.
            buzzer_state = not buzzer_state
            commands = []

            # Print the sensor readings and control the buzzer
            controller.control_actuators(commands)
            readings = controller.read_sensors()
            print("\nREADINGS")
            for reading in readings:
                print(reading)

            # Sleep
            sleep(delay)

    except KeyboardInterrupt:
        # Cleanup
        controller.control_actuators(commands)
