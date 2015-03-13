# General Description #

RF Explorer is based on a powerful Microchip 16 bits microcontroller: PIC24FJ64GA004. This MCU is the core of all internal functionality, and includes a few pins available in an expansion port. This port will provide room for additional external functionality, including programmable modules users can create or develop on their own.

Click here to get full size [Schematics](http://micro.arocholl.com/download/Circuit%20-%20RFExplorer%20v1.05.png)

![http://micro.arocholl.com/download/Circuit%20-%20RFExplorer%20v1.05.png](http://micro.arocholl.com/download/Circuit%20-%20RFExplorer%20v1.05.png)

The MCU already implements a few external commands and more are coming. A future firmware will allow all important functionality to be externally accessed through simple RS232 commands.

The RF section is a highly integrated Silicon Labs sub-1Ghz Si4432 transceiver, which offers receiver and transmitter features. For 2.4Ghz band, the RF section will be implemented with a Texas Instruments CC2500 transceiver.

Lipo battery charger circuit is based on a highly integrated Microchip MCP73811 device, which works very well for standard USB 500mA charging load. This is an ideal setup for the internal 860mAh lipo battery. The charger offers enough current to feed the device when connected to USB, and use all remaining current for charging the lipo.

RS232 communication is driven by a Silicon Labs CP2102. It offers really high speed communication and has a good tolerance for clock drifts. It works perfectly well with 500Kbps and, based on user feedback, 900Kbps could be added in the future. Drivers works very well in Windows. If you use it in Linux or MacOS and experience any problem, contact me to try to resolve with Silabs Tech support.

Finally, the LCD is a COG 128x64 device working in serial mode to save MCU pins. It has reasonably good refresh rate and includes a backlight led dimmed by a PWM buffered with a NPN transistor. The backlight LED is not regulated with the LDO to save it from excess power handling, therefore the transistor current will vary with the usable lipo battery voltage (4.2V-3.0V).