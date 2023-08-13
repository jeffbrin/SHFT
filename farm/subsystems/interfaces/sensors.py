from abc import ABC, abstractmethod, abstractproperty
from .reading import Reading
from typing import Optional


class ISensor(ABC):
    """
    Interface for sensors in an IoT system.

    Attributes
    ----------
    gpio : int or None
        The GPIO port or bus to which the actuator is attached. This may be None if the 
        actuator does not require a GPIO port.
    """

    @abstractmethod
    def __init__(self, gpio: Optional[int] = None) -> None:
        """
        Initializes a new sensor object. Concrete sensor classes are responsible for 
        setting their own reading types.

        Parameters
        ----------
        gpio : int or None, optional
            The GPIO port or bus to which the sensor is attached. This may be None if the 
            sensor does not require a GPIO port.

        Raises
        ------
        NotImplementedError
            If the concrete sensor class does not provide an implementation for this method.

        Attributes
        ----------
        __gpio : int or None
            The GPIO port or bus to which the sensor is attached. This may be None if the 
            sensor does not require a GPIO port.
        """
        self.__gpio = gpio

    @abstractmethod
    def read(self) -> list[Reading]:
        """
        Generates a list of readings based on sensor data.

        Raises
        ------
        NotImplementedError
            If the concrete sensor class does not provide an implementation for this method.

        Returns
        -------
        list[Reading]
            A list of reading objects generated from sensor data.
        """

    @abstractproperty
    def reading_types(self) -> list[Reading.Type]:
        """
        Returns the reading types. Used to force implementations to have reading types.

        Returns
        -------
        list[Reading.Type]
            This sensor's reading types.
        """

    @abstractproperty
    def reading_units(self) -> list[Reading.Unit]:
        """
        Returns the reading units. Used to force implementations to have reading units.

        Returns
        -------
        list[Reading.Unit]
            This sensor's reading units.
        """
