# Introduction #

This page describes the specification of supported remote commands from/to RF Explorer device.

The commands are split into "Any Model" so are valid to all RF Explorer device models. Some others are split in specific tables for Signal Generator or Spectrum Analyzer models.

Note different versions of the firmware may support different commands.



# Command list #

### These are commands available from PC to RF Explorer _Any Model_ ###

| **Command**                | **Syntax** | **Comment / Description** | **Firmware Version** |
|:---------------------------|:-----------|:--------------------------|:---------------------|
| `Request_Config`     | `#<Size>C0` | Request RF Explorer to send `Current_Config` where `<Size>=4 bytes` | 1.05 |
| `Request_Shutdown`   | `#<Size>CS` | Shutdown RF Explorer unit where `<Size>=4 bytes` | 1.11 |
| `Request_Hold`       | `#<Size>CH` | Stop spectrum analyzer data dump where `<Size>=4 bytes` | 1.06 |
| `Request_Reboot`     | `#<Size>r` | Restart RF Explorer unit where `<Size>=3 bytes` | 1.11 |
| `Change_baudrate`    | `#<Size>c<baudrate_code>` | Switch RF Explorer communication baudrate where `<Size>=4 bytes`. The switch is immediate and it is lost after a reset. Reliable and recommended communication baudrates are 2400bps and 500Kbps only, other modes can be used but may not be reliable in the whole temperature range. Note 'c' is lowercase. | 1.06 |
| `Disable_LCD`        | `#<Size>L0` | Request RF Explorer to switch LCD OFF and stop all draw activity `<Size>=4 bytes` | 1.08 |
| `Enable_LCD`         | `#<Size>L1` | Request RF Explorer to switch LCD back ON and resume all draw activity `<Size>=4 bytes` | 1.08 |
| `Enable_DumpScreen`  | `#<Size>D1` | Request RF Explorer to dump all screen data where `<Size>=4 bytes`. It will remain active until `Disable_DumpScreen` is sent. | 1.06 |
| `Disable_DumpScreen` | `#<Size>D0` | Request RF Explorer to stop dump screen data where `<Size>=4 bytes` | 1.06 |

### These are commands available from PC to RF Explorer _Spectrum Analyzer_ only ###

| **Command**                | **Syntax** | **Comment / Description** | **Firmware Version** |
|:---------------------------|:-----------|:--------------------------|:---------------------|
| `AnalyzerConfig`     | `#<Size>C2-F:<Start_Freq>, <End_Freq>, <Amp_Top>, <Amp_Bottom>` | It will change current configuration for RF Explorer where `<Size>=32 bytes` and send current Spectrum Analyzer configuration data back to PC. | 1.06 |
| `SwitchModuleMain`   | `#<Size>CM<0>` | Request RF Explorer to enable Mainboard module where `<0> is a binary 0 byte` and `<Size>=5 bytes`| 1.11 |
| `SwitchModuleExp`    | `#<Size>CM<1>` | Request RF Explorer to enable Expansion module (if available) `<1> is a binary 1 byte` and `<Size>=5 bytes`| 1.11 |
| `SetCalculator`      | `#<Size>C+<CalcMode>` | Request RF Explorer to set onboard calculator mode `<Size>=5 bytes`| 1.11 |
| `Request_Tracking`   | `#<Size>C3-K:<Start_Freq>, <Freq_Step_KHZ>` | Request RF Explorer to enter tracking mode to work with RFEGEN `<Size>=22 bytes`| 1.12 |
| `Tracking_Step`      | `#<Size>k<TrackingStep>` | Request RF Explorer to jump over the tracking step frequency and measure it immediately `<Size>=5 bytes`| 1.12 |
|  | ` ` |  |  |


### These are commands available from PC to RF Explorer _Signal Generator_ only ###

| **Command**                | **Syntax** | **Comment / Description** | **Firmware Version** |
|:---------------------------|:-----------|:--------------------------|:---------------------|
| `EnableCW`           | `#<Size>C3-F:<CW_Freq>, <HighPowerSW>, <PowerLevel>, <TotalSteps>, <Freq_Step_KHZ>` |  Request RFEGen to enable CW signal at frequency and power level specified where `<Size>=31 bytes`. A RF load is required when enabling RF power| 1.12 |
| `SetSweepStep`       | `#<Size>C4-F:<Freq_Step_KHZ>, <StepMode>` |  Set internal RFEGen step size for Sweep or Tracking `<Size>=16 bytes`. | 1.12 |
| `EnableRFPower`      | `#<Size>CP1` | Enable RF power with current power level and frequency configuration `<Size>=5 bytes`. A RF load is required when enabling RF power| 1.12 |
| `DisableRFPower`     | `#<Size>CP0` | Disable RF power `<Size>=5 bytes`| 1.12 |
| `Tracking_Step`      | `#<Size>k<TrackingStep>` | Request RFEGen to jump over the tracking step frequency and generate it `<Size>=5 bytes`| 1.12 |
|  | ` ` |  |  |

### These are commands available from RF Explorer _Any Model_ to PC ###

| **Command**                | **Syntax** | **Comment / Description** | **Firmware Version** |
|:---------------------------|:-----------|:--------------------------|:---------------------|
| `Screen_data`        | `$D<Byte><Byte>..<Byte><EOL>` | Dump all 128\*8 screen bytes to PC client. The memory is organized as 8 consecutive rows of 128 bytes each. Organized top to bottom. Every bit within the byte represents a pixel ON(1) or OFF(0) being the MSB the upper pixel and the LSB the bottom pixel within the row. | 1.06 |
|  | ` ` |  |  |

### These are commands available from RF Explorer _Spectrum Analyzer_ only to PC ###

| **Command**                | **Syntax** | **Comment / Description** | **Firmware Version** |
|:---------------------------|:-----------|:--------------------------|:---------------------|
| `Current_Setup`      | `#C2-M:<Main_Model>, <Expansion_Model>, <Firmware_Version> <EOL>` | Send current Spectrum Analyzer model setup and firmware version | 1.06 |
| `Current_Config`     | `#C2-F:<Start_Freq>, <Freq_Step>, <Amp_Top>, <Amp_Bottom>, <Sweep_Steps>, <ExpModuleActive>, <CurrentMode>, <Min_Freq>, <Max_Freq>, <Max_Span>, <RBW>, <AmpOffset>, <CalculatorMode> <EOL>` | Send current Spectrum Analyzer configuration data. From RFE to PC, will be used by the PC to control PC client GUI. Note this has been updated in v1.12 | 1.12+ |
| `Current_Config`     | `#C2-F:<Start_Freq>, <Freq_Step>, <Amp_Top>, <Amp_Bottom>, <Sweep_Steps>, <ExpModuleActive>, <CurrentMode>, <Min_Freq>, <Max_Freq>, <Max_Span>, <RBW> <EOL>` | Send current Spectrum Analyzer configuration data. From RFE to PC, will be used by the PC to control PC client GUI. Note this has been updated in v1.09+ to include RBW, so the client software must check for string length to know if the RBW is included or not. | 1.09-1.11 |
| `Current_Config`     | `#C2-F:<Start_Freq>, <Freq_Step>, <Amp_Top>, <Amp_Bottom>, <Sweep_Steps>, <ExpModuleActive>, <CurrentMode>, <Min_Freq>, <Max_Freq>, <Max_Span> <EOL>` | Send current Spectrum Analyzer configuration data. From RFE to PC, will be used by the PC to control PC client GUI | 1.06-1.08 |
| `Sweep_data`         | `$S<Sample_Steps> <AdBm>… <AdBm> <EOL>` | Send all dBm sample points to PC client, in binary | 1.05 |
|  | ` ` |  |  |

### These are commands available from RF Explorer _Signal Generator_ only to PC ###

| **Command**                | **Syntax** | **Comment / Description** | **Firmware Version** |
|:---------------------------|:-----------|:--------------------------|:---------------------|
| `Current_Setup`      | `#C3-M:<Main_Model>, <Expansion_Model>, <Firmware_Version> <EOL>` | Send current model setup and firmware version | 1.12 |
| `Current_Config`     | `#C3-G:<Start_Freq>, <CW_Freq>, <TotalSteps>, <Freq_Step_KHZ>, <HighPowerSW>, <PowerLevel>, <RFPowerON> <EOL>` | Send current Signal Generator configuration data. | 1.12+ |
|  | ` ` |  |  |

# Data field list #

| **Field**                  | **Syntax** | **Units** | **Comment / Description** | **Firmware Version** |
|:---------------------------|:-----------|:----------|:--------------------------|:---------------------|
| `<Size>`             | `Binary byte`                | Bytes | Total size of the message in bytes. Size is limited to max 64 bytes. | 1.05 |
| `<Start_Freq>`       | `7 ASCII digits, decimal`    | KHZ | Value of frequency span start (lower) | 1.06 |
| `<End_Freq>`         | `7 ASCII digits, decimal`    | KHZ | Value of frequency span end (higher) | 1.06 |
| `<Freq_Step>`        | `7 ASCII digits, decimal`    | HZ | Value of frequency of sample step | 1.06 |
| `<Amp_Top>`          | `4 ASCII digits, decimal`    | dBm | Highest value of amplitude for GUI | 1.05 |
| `<Amp_Bottom>`       | `4 ASCII digits, decimal`    | dBm | Lowest value of amplitude for GUI | 1.05 |
| `<EOL>`              | `0x0A, 0x0D`                 | - | End of line, standard CR + LF | 1.05 |
| `<Sample_Steps>`     | `Binary byte`                | Steps | Number of sweep steps with values which follows | 1.05 |
| `<AdBm>`             | `Binary byte`                | dBm | Sampled value in dBm, repeated n times one per sample. To get the real value in dBm, consider this an unsigned byte, divide it by two and change sign to negative. For instance a byte=0x11 (17 decimal) will be -17/2= -8.5dBm. This is now normalized and consistent for all modules and setups | 1.06 |
| `<baudrate_code>`    | `1 ASCII digit, decimal`     | - | Codified values are: 0-500Kbps, 1-1200bps, 2-2400bps, 3-4800bps, 4-9600bps, 5-19200bps, 6-38400bps, 7-57600bps, 8-115200bps | 1.06 |
| `<Main_Model>`       | `3 ASCII digits, decimal`    | - | Codified values are 433M:0, 868M:1, 915M:2, WSUB1G:3, 2.4G:4, WSUB3G:5 | 1.06 |
| `<Expansion_Model>`  | `3 ASCII digits, decimal`    | - | Codified values are 433M:0, 868M:1, 915M:2, WSUB1G:3, 2.4G:4, WSUB3G:5, NONE:255 | 1.06 |
| `<Firmware_Version>` | `5 ASCII chars `             | - | Standard format xx.yy, may change format for betas or custom versions | 1.06 |
| `<Sweep_Steps>`      | `4 ASCII digits, decimal `   | Steps | Number of sweep steps in the current configuration | 1.06 |
| `<ExpModuleActive>`  | `1 ASCII digit, binary `     | - | 1 if the Expansion module is the active circuit, 0 otherwise (the main board is active) | 1.06 |
| `<CurrentMode>`      | `3 ASCII digits, decimal`    | - | Codified values are SPECTRUM\_ANALYZER:0, RF\_GENERATOR:1, WIFI\_ANALYZER:2, UNKNOWN:255 | 1.06 |
| `<Min_Freq>`         | `7 ASCII digits, decimal`    | KHZ | Min supported frequency value for the selected RF circuit and mode | 1.06 |
| `<Max_Freq>`         | `7 ASCII digits, decimal`    | KHZ | Max supported frequency value for the selected RF circuit and mode | 1.06 |
| `<Max_Span>`         | `7 ASCII digits, decimal`    | KHZ | Max supported Span value for the selected RF circuit and mode | 1.06 |
| `<RBW>`              | `5 ASCII digits, decimal`    | KHZ | Resolution Bandwidth currently selected | 1.09 |
| `<CalcMode>`         | `Binary byte`                | - | 0=NORMAL, 1=MAX, 2=AVG, 3=OVERWRITE, 4=MAX\_HOLD | 1.11 |
| `<Freq_Step_KHZ>`    | `7 ASCII digits, decimal`    | KHZ | Value of frequency sweep/tracking step | 1.12 |
| `<TrackingStep>`     | `Bynary word 16 bits`        | Steps | Number of scan step being used for tracking or sweep mode | 1.12 |
| `<AmpOffset>`        | `4 ASCII digits, decimal`    | dB | OffsetDB manually specified in the unit, including sign | 1.12 |
| `<CalculatorMode>`   | `3 ASCII digits, decimal`    | - | Active calculator mode in the unit, see `<CalcMode>` for values | 1.12 |
| `<CW_Freq>`          | `7 ASCII digits, decimal`    | KHZ | Value of CW frequency for generator, as well as start frequency for Sweep or tracking | 1.12 |
| `<HighPowerSW>`      | `1 ASCII digit, decimal`     | - | 0=Low Power (attenuator ON), 1=High Power (attenuator OFF) | 1.12 |
| `<PowerLevel>`       | `1 ASCII digit, decimal`     | - | 0-3=Increasing power level from 0..3 representing nominal 3dB increase power | 1.12 |
| `<TotalSteps>`       | `4 ASCII digits, decimal`    | Steps | 2-9999 possible steps for Sweep or Tracking mode | 1.12 |
| `<RFPowerON>`        | `1 ASCII digit, decimal`     | - | 0=RF Power is ON (a RF load MUST be connected), 1=RF Power is OFF | 1.12 |
| `<>` | ` ` |  |  |  |

Note: All information while believe to be accurate is not
guaranteed, please contact us if you find any typo or missing data.