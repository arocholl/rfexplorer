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

//-----------------Includes
#include <termios.h>
#include <stdio.h>
#include <unistd.h>
#include <fcntl.h>
#include <sys/signal.h>
#include <sys/types.h>
#include <string.h>

//-----------------Useful defines

#define BAUDRATE B500000			//Change this to B2400 if you want low speed, but change RFExplorer too!
#define MODEMDEVICE "/dev/ttyUSB0"	//By default use first USB port
#define FALSE 0
#define TRUE 1

//-----------------Function headers

int getch(void);
void signal_handler_IO (int status);
void SendCommandToRFExplorer(int fd, char* pCommand);

//-----------------Globals

int wait_flag=TRUE;                    /* TRUE while no signal received */

//-----------------Functions
int main ( int argc, char **argv )
{
    int fd,c,res;
    struct termios oldtio,newtio;
    struct sigaction saio;           /* definition of signal action */
    char buf[1040];
    char sPortName[255];
    strcpy(sPortName,MODEMDEVICE);

    if (argc>1)
    {
        printf("COM port ID specified: %s\r\n",argv[1]);
        strcpy(sPortName,argv[1]);
    }

    printf("Using COM port ID: %s\r\n",sPortName);

    /* open the device to be non-blocking (read will return immediatly) */
    fd = open(sPortName, O_RDWR | O_NOCTTY | O_NONBLOCK);
    if (fd <0) 
    {
        perror(sPortName); 
        exit(-1); 
    }

    /* install the signal handler before making the device asynchronous */
    memset(&saio,0x0,sizeof(saio));
    saio.sa_handler = signal_handler_IO;
    sigaction(SIGIO,&saio,NULL);
      
    /* allow the process to receive SIGIO */
    fcntl(fd, F_SETOWN, getpid());
    /* Make the file descriptor asynchronous (the manual page says only 
       O_APPEND and O_NONBLOCK, will work with F_SETFL...) */
    fcntl(fd, F_SETFL, FASYNC);

    tcgetattr(fd,&oldtio); //save current port settings for later restoring them
	
    //Set new port settings for canonical input processing
	//We need 5000000,8N1 or 2400,8N1 depending on how RF Explorer was configured
    newtio.c_cflag = BAUDRATE | CS8 | CLOCAL | CREAD;
    newtio.c_iflag = IGNPAR | ICRNL;
    newtio.c_oflag = 0;
    newtio.c_lflag = 0;
    newtio.c_cc[VMIN]=1;
    newtio.c_cc[VTIME]=0;
    tcflush(fd, TCIFLUSH);
    tcsetattr(fd,TCSANOW,&newtio);
    
	SendCommandToRFExplorer(fd,"r"); //Reset RF Explorer
	
	while(wait_flag);	//wait till RF Explorer respond with some data

	memset(buf,0x0,sizeof(buf));
    res = read(fd,buf,1024); //read data received
	printf(buf);
    /* restore old port settings */
    tcsetattr(fd,TCSANOW,&oldtio);
	close(fd);
}

/***************************************************************************
* signal handler. sets wait_flag to FALSE, to indicate above loop that     *
* characters have been received.                                           *
***************************************************************************/

void signal_handler_IO (int status)
{
    wait_flag = FALSE;
}

/***************************************************************************
* Custom stdin manipulation to get a getch() equivalent from getchar()     *
* to make sure we don't need to wait for an ENTER from the keyboard        *
***************************************************************************/

int getch(void)
{
	struct termios oldt, newt;
	int ch;
	tcgetattr( 0, &oldt );
	newt = oldt;
	newt.c_lflag &= ~( ICANON | ECHO );
	tcsetattr( 0, TCSANOW, &newt );
	ch = getchar();
	tcsetattr( 0, TCSANOW, &oldt );
	return ch;
}

/***************************************************************************
* Function to send a string text command formatted as RFExplorer needs     *
***************************************************************************/

void SendCommandToRFExplorer(int fd, char* pCommand)
{
	char pHeader[2]={'#',strlen(pCommand)+2};
	write(fd,pHeader,2);
	write(fd,pCommand,strlen(pCommand));
}
