# Written by Jeffrey Bringolf

from time import sleep

from ..helpers.acceleration_sensor import AccelerationSensor
from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading


class VibrationSensor(ISensor):

    TIMEOUT = 4

    def __init__(self, gpio=None) -> None:
        self.accelerationSensor = AccelerationSensor()
        self._reading_types = [Reading.Type.VIBRATION]
        self._reading_units = [Reading.Unit.NONE]
        self.previousAccelerationX = 0
        self.previousAccelerationY = 0
        self.previousAccelerationZ = 0

    def read(self) -> list[Reading]:

        try:
            accelX, accelY, accelZ = self.accelerationSensor.read(
                timeout=VibrationSensor.TIMEOUT)
        except (BlockingIOError, TimeoutError):
            return []

        # Calculate vibration
        vibrationX = accelX - self.previousAccelerationX
        vibrationY = accelY - self.previousAccelerationY
        vibrationZ = accelZ - self.previousAccelerationZ

        axesCount = 3
        meanVibration = (vibrationX + vibrationY + vibrationZ) / axesCount

        # Store previous acceleration
        self.previousAccelerationX = accelX
        self.previousAccelerationY = accelY
        self.previousAccelerationZ = accelZ

        return [
            Reading(meanVibration,
                    self.reading_types[0], self.reading_units[0])
        ]

    @property
    def reading_types(self) -> list[Reading.Type]:
        return self._reading_types

    @property
    def reading_units(self) -> list[Reading.Unit]:
        return self._reading_units


if __name__ == "__main__":

    delay = 1
    sensor = VibrationSensor()

    try:
        while True:
            print(sensor.read())
            sleep(delay)
    except KeyboardInterrupt:
        pass
