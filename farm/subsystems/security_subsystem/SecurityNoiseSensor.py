from ..interfaces.sensors import ISensor
from ..interfaces.reading import Reading
from grove.adc import ADC
from time import sleep
import math
from typing import Optional


class SecurityNoiseSensor(ISensor):

    def __init__(self, i2c_address: Optional[int] = None, adc_channel: Optional[int] = 4) -> None:
        self._sensor_ = ADC(i2c_address)
        self._channel = adc_channel
        self._reading_types = [Reading.Type.NOISE]
        self._reading_units = [Reading.Unit.DECIBEL]

    def read(self) -> list[Reading]:
        reading_value = self._sensor_.read_voltage(self._channel)
        
        if reading_value > 0:
            # http://www.sengpielaudio.com/calculatorVoltagePower.htm#:~:text=For%20us%20
            # decibels%20are%20defined,log%20of%20an%20amplitude%20ratio.&text=Voltage%20
            # gain%20(dB)%20%3D%2020,Used%20in%20audio.
            dB = 20 * math.log10(reading_value)
        else:
            dB = 0
        
        return [Reading(value=dB,
                        reading_type=self._reading_types[0],
                        reading_unit=self._reading_units[0])]
       

    @property
    def reading_types(self) -> Reading.Type:
        return self._reading_types

    @property
    def reading_units(self) -> Reading.Unit:
        return self._reading_units


if __name__ == "__main__":
    DEFAULT_I2C_ADDRESS = 0x04
    
    NoiseSensor = SecurityNoiseSensor(DEFAULT_I2C_ADDRESS)

    while True:
        NoiseReadings = NoiseSensor.read()
        print(NoiseReadings)
        sleep(1)
