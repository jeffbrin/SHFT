from gpiozero import Button
from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading


class SecurityMagneticDoorSensor(ISensor):

    def __init__(self, gpio: int = None) -> None:
        self._sensor_ = Button(gpio)
        self._reading_types = [Reading.Type.DOOR_LOCKED]
        self._reading_units = [Reading.Unit.BOOL]

    def read(self) -> list[Reading]:
        if self._sensor_.is_pressed:
            self._reading_value = True
        else:
            self._reading_value = False

        return [Reading(value=self._reading_value,
                        reading_type=self._reading_types[0],
                        reading_unit=self._reading_units[0])]

    @property
    def reading_types(self) -> Reading.Type:
        return self._reading_types

    @property
    def reading_units(self) -> Reading.Unit:
        return self._reading_units


if __name__ == "__main__":

    DEFAULT_GPIO_PIN = 24

    DoorSensor = SecurityMagneticDoorSensor(DEFAULT_GPIO_PIN)

    while True:
        DoorReadings = DoorSensor.read()
        print(DoorReadings)
