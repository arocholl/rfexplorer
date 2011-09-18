RF Explorer Windows Client - Readme
===================================

Version v1.06.08 - Released CET Sep/09, 2011

Thanks for using RF Explorer, we want it to be a community driven development
to cover all workflows you may need to successfully design, finetune and diagnose
your RF link projects.

This application is open source under GPL v3 license. 
Please check our website for more info and source code download.
If you want to contribute with features and ideas, contact me anytime, check www.rf-explorer.com for
more details.

Major changes v1.06.08
======================

* Updated Remote Screen with a separated control which now uses double buffer to avoid flickering.
* Minor GUI changes
* Note versions v1.06.06-7 were not available as public releases

Major changes v1.06.05
======================

* Updated to support RF Explorer firmware v1.07
* RF Explorer firmware version is now checked and reported to the user if an old version is found.
* Communication from PC -> RF Explorer improved. In some USB connections sometimes the PC was not able
  to change configuration settings in RF Explorer connected unit.
* RFE data files are now associated with RF Explorer Windows PC Client application and they will load
  automatically from Windows File Explorer.

Major changes v1.06.04
======================

* Updated to support RF Explorer firmware v1.06
* Added support for comma delimited file CSV
* Remote screen capture to download RF Explorer screenshots realtime, in different sizes for easy reading and share.
* Screenshots can be saved in a stream file RFS or as individual images PNG
* Spectrum Analyzer screen includes automatic Peak Value reads, can be disabled in the View menu
* "Remote Frequency and Power control" control box is now more consistent with RF Explorer unit. 
    You have to click "Send" button to actually deliver the new configuration to RF Explorer.
    You can also restore local settings to that of RF Explorer by clicking on "Reset" button.

Contents
========

This package contains all PC software side as listed below:

* RF Explorer Windows Client : 
    The main tool for communication between your RF Explorer device and your PC.
    It will display in real time high resolution graphics the Spectrum Analyzer 
    activity. From there you can save graphics, print, zoom in/out, etc.
    Please visit our website for updated user instruction manual.
    RF Explorer Windows Client is open source licensed under GNU GPL v3. 
    See included License-GPL.txt for more details.

* RF Explorer Uploader:
    This tool will upgrade your RF Explorer device with new firmware. Please
    visit our website to get the latest firmware available. You are entitled
    for lifetime free upgrades to your RF Explorer device.
    RF Explorer Uploader tool is copyrighted software by (C) Ariel Rocholl.

Requirements
============

The requirements to run this software are:

* Microsoft Windows XP SP3 or higher, including Windows Vista and Windows 7 
    (both 32bits and 64bits are supported).
* Pentium IV 1GHZ or higher processor.
* 512MB RAM or higher
* USB 2.0 compatible port
* Silabs CP2102 USB driver. You can download latest version from Silabs or
    a copy from our website.

Visit us at http://www.rf-explorer.com
