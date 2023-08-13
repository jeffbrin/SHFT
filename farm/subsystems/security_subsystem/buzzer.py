# Written by Jeffrey Bringolf

from enum import Enum
from time import sleep
import seeed_python_reterminal.core as rt
from ..interfaces.reading import Reading
from ..interfaces.sensors import ISensor
from ..interfaces.command import Command
from ..interfaces.actuators import IActuator


class Buzzer(ISensor, IActuator):

    class State(Enum):
        ON = True
        OFF = False

        @classmethod
        def contains(cls, value: "Buzzer.State") -> bool:
            """
            Checks whether the state is present in this enum.

            Parameters
            ----------
            state: Buzzer.State
                The state to check whether it is contained in the enum.

            Returns
            -------
            bool
                True if the state is in the enum. False otherwise.
            """

            return len([x for x in iter(cls) if x.value == value]) > 0

        @classmethod
        def has_value(cls, value: bool) -> bool:
            """
             Checks whether the state value is present in this enum.

             Parameters
             ----------
             state: bool
                 The boolean value to check whether it is contained in the enum as a value.

             Returns
             -------
             bool
                 True if the state value is in the enum. False otherwise.
             """

            return value in cls._value2member_map_

    def __init__(self, gpio=None) -> None:
        self._reading_types = [Reading.Type.BUZZER]
        self._reading_units = [Reading.Unit.BOOL]

    def read(self) -> list[Reading]:
        return [Reading(rt.buzzer, self.reading_types[0], self.reading_units[0])]

    @property
    def reading_types(self) -> list[Reading.Type]:
        return self._reading_types

    @property
    def reading_units(self) -> list[Reading.Unit]:
        return self._reading_units

    def control_actuator(self, command: Command) -> bool:

        # Guard on command validity
        if not self.validate_command(command):
            return False

        if command.value == Buzzer.State.ON.value:
            # Buzzer on
            rt.buzzer = True
        elif command.value == Buzzer.State.OFF.value:
            # Buzzer off
            rt.buzzer = False
        else:
            return False

        return True

    def validate_command(self, command: Command) -> bool:

        return Buzzer.State.has_value(command.value) \
            and command.type == Command.Type.BUZZER


# Must be run with sudo
if __name__ == "__main__":

    buzzer = Buzzer()
    delay = 1

    try:
        while True:

            print("Turning On")
            buzzer.control_actuator(
                Command(
                    Command.Type.BUZZER,
                    Command.Unit.BOOL,
                    Buzzer.State.ON.value)
            )
            print(buzzer.read())

            # Pause for results
            sleep(delay)

            print("Turning Off")
            buzzer.control_actuator(
                Command(
                    Command.Type.BUZZER,
                    Command.Unit.BOOL,
                    Buzzer.State.OFF.value)
            )
            print(buzzer.read())

            # Pause for results
            sleep(delay)

    except KeyboardInterrupt:
        # Turn off when done
        buzzer.control_actuator(
            Command(
                Command.Type.BUZZER,
                Command.Unit.BOOL,
                Buzzer.State.OFF.value
            )
        )
