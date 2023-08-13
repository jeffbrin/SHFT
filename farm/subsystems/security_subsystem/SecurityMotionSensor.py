from grove.grove_mini_pir_motion_sensor import GroveMiniPIRMotionSensor
from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading

from time import sleep


class SecurityMotionSensor(ISensor):

    def __init__(self, gpio=int) -> None:
        self._sensor_ = GroveMiniPIRMotionSensor(gpio)
        self._sensor_.on_detect = self.__callback__
        self._reading_types = [Reading.Type.MOTION]
        self._reading_units = [Reading.Unit.BOOL]
        self._current_value = False

    def __callback__(self) -> None:
        self._current_value = True
        print("Motion Detected")

    def read(self) -> list[Reading]:
        reading = [Reading(value=self._current_value,
                        reading_type=self._reading_types[0],
                        reading_unit=self._reading_units[0])]
        self._current_value = False
        return reading

    @property
    def reading_types(self) -> Reading.Type:
        return self._reading_types

    @property
    def reading_units(self) -> Reading.Unit:
        return self._reading_units


if __name__ == "__main__":

    DEFAULT_GPIO_PIN = 16

    MotionSensor = SecurityMotionSensor(DEFAULT_GPIO_PIN)

    while True:
        MotionReadinds = MotionSensor.read()
        print(MotionReadinds)
        sleep(1)
