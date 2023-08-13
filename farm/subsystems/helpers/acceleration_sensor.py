# Written by Jeffrey Bringolf

from time import time
import seeed_python_reterminal.core as rt
import seeed_python_reterminal.acceleration as rt_accel


class AccelerationSensor:

    def __init__(self) -> None:
        """Initializes the AccelerationSensor."""
        self.accelerometer = rt.get_acceleration_device()

    def read(self, timeout: int = None) -> tuple[float]:
        """
        Reads acceleration data from the accelerometer. If a timeout is given, the method
        will throw a AccelerationSensor.TimeoutError if all readings haven't been read in
        the given amount of time.

        Parameters
        ----------
        timeout: int
            The number of seconds to stay in the method for before raising a TimeoutError

        Raises
        ------
        TimeoutError
            Raised when all three acceleration values haven't been read in the given timeout.
        BlockingIOError
            Raised when the accelerometer can not read data.

        Returns
        -------
        tuple[float]
            A tuple of reading values from the accelerometer (x, y, z)
        """

        accelX = None
        accelY = None
        accelZ = None
        start_time = time()

        while None in (accelX, accelY, accelZ) and \
                (timeout is None or (start_time - time()) < start_time + timeout):

            events = self.accelerometer.read()

            # Read accelerometer data
            for event in events:
                accelEvent = rt_accel.AccelerationEvent(event)

                # Filter out invalid events or yaw
                if accelEvent.name is not None:

                    # Get appropriate reading type
                    if accelEvent.name == rt_accel.AccelerationName.X:
                        accelX = accelEvent.value
                    # Get appropriate reading type
                    elif accelEvent.name == rt_accel.AccelerationName.Y:
                        accelY = accelEvent.value
                    # Get appropriate reading type
                    elif accelEvent.name == rt_accel.AccelerationName.Z:
                        accelZ = accelEvent.value

        accelerations = (accelX, accelY, accelZ)
        if None in accelerations:
            raise TimeoutError()

        return accelerations
