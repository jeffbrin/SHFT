# Written by Jeffrey Bringolf

from time import sleep
from math import atan, pi, sqrt

from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading

from ..helpers.acceleration_sensor import AccelerationSensor


class AngleSensor(ISensor):

    DEGREES = 180
    PITCH_TYPE_INDEX = 0
    ROLL_TYPE_INDEX = 1
    TIMEOUT = 4

    def __init__(self, gpio=None) -> None:
        self.accelerationSensor = AccelerationSensor()
        self._reading_types = [Reading.Type.PITCH, Reading.Type.ROLL]
        self._reading_units = [Reading.Unit.DEGREES]

    def read(self) -> list[Reading]:

        try:
            accelX, accelY, accelZ = self.accelerationSensor.read(
                timeout=AngleSensor.TIMEOUT)
        except (BlockingIOError, TimeoutError):
            return []

        # https://engineering.stackexchange.com/questions/3348/calculating-pitch-yaw-and-roll-from-mag-acc-and-gyro-data
        # Pitch and roll are the same whether they're flat or upside down. I can not find
        # an equation which differentiates between the two. Hopefully the farm container
        # doesn't do a handstand.
        try:
            pitch = 180 * \
                atan(accelX/sqrt(accelY*accelY + accelZ*accelZ))/pi
        except ZeroDivisionError:
            # Impossible to calculate pitch when accelY and accelZ are both 0.
            pitch = None

        try:
            roll = 180 * \
                atan(accelY/sqrt(accelX*accelX + accelZ*accelZ))/pi
        except ZeroDivisionError:
            # Impossible to calculate roll when accelX and accelZ are both 0.
            roll = None

        # Return readings, filtering out Nones
        return [reading for reading in
                [
                    Reading(pitch, self.reading_types[self.PITCH_TYPE_INDEX],
                            self.reading_units[0]),
                    Reading(roll, self.reading_types[self.ROLL_TYPE_INDEX],
                            self.reading_units[0]),
                ]
                if reading.value is not None]

    @property
    def reading_types(self) -> list[Reading.Type]:
        return self._reading_types

    @property
    def reading_units(self) -> list[Reading.Unit]:
        return self._reading_units


if __name__ == "__main__":

    delay = 1
    a = AngleSensor()
    while True:
        readings = a.read()
        if readings:
            print(readings)
            sleep(delay)
