import seeed_python_reterminal.core as rt

from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading
from time import sleep


class SecurityLumonisitySensor(ISensor):

    def __init__(self, gpio: int = None) -> None:
        self._reading_types = [Reading.Type.LUMINOSITY]
        self._reading_units = [Reading.Unit.LUX]
        self._current_value = 0

    def read(self) -> list[Reading]:
        return [Reading(rt.illuminance, self._reading_types[0], self._reading_units[0])]

    @property
    def reading_types(self) -> Reading.Type:
        return self._reading_types

    @property
    def reading_units(self) -> Reading.Unit:
        return self._reading_units


if __name__ == "__main__":

    LumonisitySensor = SecurityLumonisitySensor()

    while True:

        LumonisityReadinds = LumonisitySensor.read()
        print(LumonisityReadinds)
        sleep(0.5)
