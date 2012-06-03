//============================================================================
//RF Explorer - A Handheld Spectrum Analyzer for everyone!
//Linux Serial connection sample
//Copyright © 2010-12 Ariel Rocholl, www.rf-explorer.com
//
//This application is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 3.0 of the License, or (at your option) any later version.
//
//This software is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//General Public License for more details.
//
//You should have received a copy of the GNU General Public
//License along with this software; if not, write to the Free Software
//Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//=============================================================================

//-----------------Information
//
//This is a simple code example under GCC that works in Ubuntu Linux 11.x
//It may work in other versions as well, but you may need to modify certain
//compiler include settings or define files.
//
//This code will connect with RF Explorer, will reset it (restart) and display version information 
//received after reset. Note: you have to be in Spectrum Analyzer screen mode in RF Explorer for 
//this to work.
//
//If you have problems setting up a compatible development environment, you can
//get a VirtualBox image for free, with all included Codelite IDE, GCC and Ubuntu setup
//ready to compile and work with RF Explorer for development.
//Contact rfexplorer@arocholl.com for more details.
//
//To run this utility example, just type
//
//     ./RFExplorerReset /dev/ttyUSB0
//
//Select a different USB port if needed.
//
//You can easily debug USB and Serial port data transfer by enabling Linux core debug dump
//
//     echo 1 > /sys/bus/usb-serial/drivers/generic/module/parameters/debug
//     echo 1 > /sys/bus/usb-serial/drivers/cp210x/module/parameters/debug
//
//and then use dmesg to read last activity traced
//
//     dmesg
//
//If you run into questions, please use distribution list to get help from the community at
// www.rf-explorer.com/forum
