"""
This module defines an RGBLedStick class that implements both the ISensor and IActuator 
interfaces. It represents a Grove RGB LED Stick connected to a Raspberry Pi and can read 
and control its state.

Classes:
    RGBLedStick: A class that implements both the ISensor and IActuator interfaces to 
    read and control the state of an RGB LED Stick.

Methods:
    read(): Returns the current state of the RGB LED Stick as a list of Reading objects.
    validate_command(command: Command) -> bool: Validates if the given command is valid 
                                for the RGB LED Stick.
    control_actuator(command: Command) -> bool: Executes the given command on the RGB 
                                LED Stick.

Attributes:
    reading_type: The type of reading to be taken - RGB_LED_STICK.
    reading_unit: The unit of the reading - Boolean, represents whether the LED is on 
                    or off.

Usage:
    led_strip = RGBLedStick(count=10, color=Color(255, 0, 0))
    delay = 3
    while True:
        readings = led_strip.read()
        for reading in readings:
            print(f"RGB LED Stick: {reading.value}")
            command = Command(Command.Type.RGB_LED_STICK, RGBLedStick.State.ON)
            led_strip.control_actuator(command)
        sleep(delay)
"""
from enum import Enum
from time import sleep
from rpi_ws281x import Color
from grove.grove_ws2813_rgb_led_strip import GroveWS2813RgbStrip, LED_BRIGHTNESS
from ..interfaces.sensors import ISensor
from ..interfaces.actuators import IActuator
from ..interfaces.reading import Reading
from ..interfaces.command import Command
from typing import Optional


class RGBLedStick(ISensor, IActuator):
    LED_ON_BRIGHTNESS = 255
    LED_OFF_BRIGHTNESS = 0
    """
    Represents a RGB LED Stick connected to a Raspberry Pi.

    Attributes:
    -----------
    __strip: GroveWS2813RgbStrip
        The GroveWS2813RgbStrip object that represents the RGB LED Stick.
    __reading_type: Reading.ReadingType
        The type of the Reading object returned by this sensor.
    __reading_unit: Reading.ReadingUnit
        The unit of the Reading object returned by this sensor.

    Methods:
    --------
    read() -> list[Reading]
        Returns the current state of the RGB LED Stick as a Reading object.
    validate_command(command: Command) -> bool
        Validates if the given command is valid for the RGB LED Stick.
    control_actuator(command: Command) -> bool
        Executes the given command on the RGB LED Stick.

    """
    class State(Enum):
        """
        Represents the possible states of the RGB LED Stick.

        Attributes:
        -----------
        ON: int
            The brightness value to turn on the RGB LED Stick.
        OFF: int
            The brightness value to turn off the RGB LED Stick.

        Methods:
        --------
        contains(value: "RGBLedStick.State") -> bool
            Returns True if the given value is a member of the RGBLedStick.State Enum.

        """
        ON = True
        OFF = False

        @classmethod
        def contains(cls, value: "RGBLedStick.State") -> bool:
            """
            Returns True if the given value is a member of the RGBLedStick.State Enum.
            """
            return len([x for x in iter(cls) if x.value == value]) > 0

    def __init__(self, gpio: Optional[int] = 18, count: int = 10, color: Optional[Color] = Color(255, 0, 0)):
        """
        Initializes the RGBLedStick object.

        Parameters:
        -----------
        gpio: int
            The GPIO pin number used to communicate with the RGB LED Strip. Default is 12.
        count: int
            The number of pixels in the RGB LED Strip. Default is 10.
        color: Color
            The default color of the RGB LED Strip. Default is red.

        """
        self.__strip = GroveWS2813RgbStrip(gpio, count)
        self.__reading_types = [Reading.Type.RGB_LED_STICK]
        self.__reading_units = [Reading.Unit.BOOL]
        for i in range(self.__strip.numPixels()):
            self.__strip.setPixelColor(i, color)

    def read(self) -> list[Reading]:
        """
        Returns the current state of the RGB LED Stick as a Reading object.
        """
        return [Reading(self.__strip.getBrightness() == RGBLedStick.LED_ON_BRIGHTNESS,
                        self.__reading_types[0], self.__reading_units[0])]

    @property
    def reading_types(self) -> Reading.Type:
        """
        Returns the type of the Reading object returned by this sensor.
        """
        return self.__reading_types

    @property
    def reading_units(self) -> Reading.Unit:
        """
        Returns the unit of the Reading object returned by this sensor.
        """
        return self.__reading_units

    def validate_command(self, command: Command) -> bool:
        """
        Validates a command object to ensure it is appropriate for controlling the 
        RGBLedStick actuator.

        Parameters:
        -----------
        command : Command
            The command object to validate.

        Returns:
        --------
        bool
            Returns True if the command is valid, False otherwise.
        """
        return RGBLedStick.State.contains(command.value) and command.type == Command.Type.RGB_LED_STICK

    def control_actuator(self, command: Command) -> bool:
        """
        Controls the RGBLedStick actuator according to the specified command.

        Parameters:
        -----------
        command : Command
            The command object that specifies how to control the actuator.

        Returns:
        --------
        bool
            Returns True if the actuator was successfully controlled, False otherwise.
        """
        if not self.validate_command(command):
            return False

        if command.value == RGBLedStick.State.ON.value:
            self.__strip.setBrightness(RGBLedStick.LED_ON_BRIGHTNESS)
        else:
            self.__strip.setBrightness(RGBLedStick.LED_OFF_BRIGHTNESS)
        self.__strip.show()
        return True


if __name__ == "__main__":
    # initialize the RGBLedStick object with the specified parameters
    PIN = 18
    COUNT = 10
    strip = RGBLedStick(PIN, COUNT, Color(255, 0, 0))

    delay = 5

    # create command objects to turn the LED strip on and off
    on_command = Command(Command.Type.RGB_LED_STICK,
                         Command.Unit.BOOL, RGBLedStick.State.ON)
    off_command = Command(Command.Type.RGB_LED_STICK,
                          Command.Unit.BOOL, RGBLedStick.State.OFF)

    try:
        while True:
            # turn the LED strip on and print its state
            print("Turning light on")
            strip.control_actuator(on_command)
            print("State: ", strip.read()[0].value)

            sleep(delay)

            # turn the LED strip off and print its state
            print("Turning strip off")
            strip.control_actuator(off_command)
            print("State: ", strip.read()[0].value)
            sleep(delay)

    except KeyboardInterrupt:
        # Turn off the LED strip when done
        strip.control_actuator(off_command)
