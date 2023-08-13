from abc import ABC, abstractmethod
from .command import Command
from typing import Optional


class IActuator(ABC):
    """
    Interface for actuators in an IoT system.

    Attributes
    ----------
    gpio : int or None
        The GPIO port or bus to which the actuator is attached. This may be None if the 
        actuator does not require a GPIO port.
    """

    @abstractmethod
    def __init__(self, gpio: Optional[int] = None) -> None:
        """
        Initializes a new actuator object.

        Parameters
        ----------
        gpio : int or None, optional
            The GPIO port or bus to which the actuator is attached. This may be None if 
            the actuator does not require a GPIO port.

        Raises
        ------
        NotImplementedError
            If the concrete actuator class does not provide an implementation for this method.

        Returns
        -------
        None
        """

    @abstractmethod
    def control_actuator(self, command: Command) -> bool:
        """
        Sets the actuator to the value passed as argument.

        Parameters
        ----------
        command : Command
            The command used to control this actuator.

        Returns
        -------
        bool
            True if the actuator is being controlled, False otherwise.
        """

    @abstractmethod
    def validate_command(self, command: Command) -> bool:
        """
        Validates the command for the actuator.

        Parameters
        ----------
        command : Command
            The command which needs to be validated before being controlled.

        Returns
        -------
        bool
            True if the command is validated, False otherwise.
        """
