# Written by Jeffrey Bringolf

from time import sleep

from .subsystem import Subsystem
from .interfaces.command import Command
from .plant_subsystem.cooling_fan import Fan
from .plant_subsystem.rgb_led_stick import RGBLedStick
from .plant_subsystem.soil_moisture_sensor import SoilMoistureSensor
from .plant_subsystem.temp_humi_sensor import TemperatureHumiditySensor
from .plant_subsystem.water_level_sensor import WaterLevelSensor


class PlantSubsystem(Subsystem):

    FAN_GPIO = 5

    def set_default_periferals(self) -> "PlantSubsystem":
        rgb = RGBLedStick()
        self.set_actuators([
            Fan(PlantSubsystem.FAN_GPIO),
            rgb,
        ])
        self.set_sensors([
            SoilMoistureSensor(),
            TemperatureHumiditySensor(),
            WaterLevelSensor(),
            rgb
        ])

        return self


# Must be run with sudo
if __name__ == "__main__":
    controller = PlantSubsystem().set_default_periferals()
    delay = 2

    buzzer_state = False
    try:
        while True:
            # Toggle the buzzer state and get the right command.
            buzzer_state = not buzzer_state
            commands = [
                Command(Command.Type.FAN, Command.Unit.BOOL, Fan.State.ON),
                Command(Command.Type.RGB_LED_STICK,
                        Command.Unit.BOOL, RGBLedStick.State.ON)
            ]

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
        controller.control_actuators([])
