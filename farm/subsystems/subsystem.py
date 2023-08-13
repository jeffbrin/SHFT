# Written by Jeffrey Bringolf

from abc import ABC, abstractmethod
from .interfaces.actuators import IActuator
from .interfaces.command import Command
from .interfaces.sensors import ISensor
from .interfaces.reading import Reading


class Subsystem(object):
    """A subsystem which is responsible for one portion of the farm."""

    def __init__(self) -> None:
        """
        Initializes the controller.

        Returns
        -------
        None
        """

        self.set_default_periferals()

    # Forward referencing allows us to return type hinting for a class that we are currently inside.
    @abstractmethod
    def set_default_periferals(self) -> "Subsystem":
        """
        Sets the default actuators and sensors for this controller.

        Returns
        -------
        Controller
            returns this controller to serve as a builder method.
        """

        pass

    def add_actuator(self, actuator: IActuator) -> None:
        """
        Adds an actuator to the list of actuators.

        Parameters
        ----------
        actuator: IActuator
            The actuator to add to this controller.

        Returns
        -------
        None
        """

        self.actuators.append(actuator)

    def add_sensor(self, sensor: ISensor) -> None:
        """
        Adds a sensor to the list of sensors.

        Parameters
        ----------
        sensor: ISensor
            The sensor to add to this controller.

        Returns
        -------
        None
        """

        self.sensors.append(sensor)

    def read_sensors(self) -> list[Reading]:
        """
        Reads from all the sensors in the controller and returns their readings.

        Returns
        -------
        list[Reading]
            A list of all the readings from the sensors.
        """

        readings = []
        for sensor in self._sensors:
            readings.extend(sensor.read())

        return readings

    def control_actuators(self, commands: list[Command]) -> None:
        """
        Controls the appropriate actuators in this controller depending on the commands

        Parameters
        ----------
        commands: list[Command]
            The list of commands used to control the actuators.
        """

        # Attempt to use each command on each actuator
        for command in commands:
            for actuator in self._actuators:
                # Control the actuator if the command is valid.
                if actuator.validate_command(command):
                    actuator.control_actuator(command)

    @property
    def sensors(self) -> list[ISensor]:
        """
        All the sensors that this subsystem uses.

        Returns
        -------
        list[ISensor]
            A list of all the sensors associated with this subsystem.
        """
        return self._sensors

    @property
    def actuators(self) -> list[IActuator]:
        """
        All the actuators that this subsystem uses.

        Returns
        -------
        list[IActuator]
            A list of all the actuators associated with this subsystem.
        """
        return self._actuators

    def set_sensors(self, value: list[ISensor]) -> None:
        """
        Sets this subsystem's sensors.

        Parameters
        ----------
        value: list[ISensor]
            The new sensors.
        """

        self._sensors = value

    def set_actuators(self, value: list[IActuator]) -> None:
        """
        Sets this subsystem's actuators.

        Parameters
        ----------
        value: list[IActuator]
            The new actuators.
        """

        self._actuators = value
