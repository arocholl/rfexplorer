# Introduction #

This page describes the specification of supported remote commands from/to RF Explorer device.

This is valid for firmware v1.05 only. Different firmware version may have different valid commands and format.

# Command list #

| **Command** | **Syntax** | **Direction** | **Comment / Description** | **Firmware Version** |
|:------------|:-----------|:--------------|:--------------------------|:---------------------|
| `Current_Config` | `#<Size>C2-F:<Start_Freq>, <Freq_Step>, <Amp_Top>, <Amp_Bottom>` | PC to RFE | Send current Spectrum Analyzer configuration data. From PC to RFE, will change current configuration for RFE where `<Size>=30 bytes`. | 1.05 |
| `Current_Config` | `#C2-F:<Start_Freq>, <Freq_Step>, <Amp_Top>, <Amp_Bottom> <EOL>` | RFE to PC | Send current Spectrum Analyzer configuration data. From RFE to PC, will be used by the PC to control PC client GUI | 1.05 |
| `Request_Config` | `#<Size>C0` | PC to RFE | Request RFE to send `Current_Config` where `<Size>=4 bytes` | 1.05 |
| `Sweep_data` | `$S<Sample_Steps> <AdBm>… <AdBm> <EOL>` | RFE to PC | Send all dBm sample points to PC client, in binary | 1.05 |

# Data field list #

| **Field** | **Syntax** | **Units** | **Comment / Description** | **Firmware Version** |
|:----------|:-----------|:----------|:--------------------------|:---------------------|
| `<Size>` | `Binary byte` | Bytes | Total size of the message in bytes. Size is limited to max 64 bytes. | 1.05 |
| `<Start_Freq>` | `6 ASCII digits, decimal` | KHZ | Value of frequency span start (lower) | 1.05 |
| `<Freq_Step>` | `5 ASCII digits, decimal` | HZ | Value of frequency of sample step | 1.05 |
| `<Amp_Top>` | `4 ASCII digits, decimal` | dBm | Highest value of amplitude for GUI | 1.05 |
| `<Amp_Bottom>` | `4 ASCII digits, decimal` | dBm | Lowest value of amplitude for GUI | 1.05 |
| `<EOL>` | `0x0A, 0x0D` | - | End of line, standard CR + LF | 1.05 |
| `<Sample_Steps>` | `Binary byte` | Steps | Number of value steps which follows | 1.05 |
| `<AdBm>` | `Binary byte` | dBm | Sampled value in dBm, repeated n times one per sample. Note the value is signed. | 1.05 |