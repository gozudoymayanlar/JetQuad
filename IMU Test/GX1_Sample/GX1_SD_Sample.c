/*----------------------------------------------------------------------
 *
 * I3DM-GX1 Interface Software
 *
 *----------------------------------------------------------------------
 * (c) 2010 Microstrain, Inc.
 *----------------------------------------------------------------------
 * THE PRESENT SOFTWARE WHICH IS FOR GUIDANCE ONLY AIMS AT PROVIDING
 * CUSTOMERS WITH CODING INFORMATION REGARDING THEIR PRODUCTS IN ORDER
 * FOR THEM TO SAVE TIME. AS A RESULT, MICROSTRAIN SHALL NOT BE HELD LIABLE
 * FOR ANY DIRECT, INDIRECT OR CONSEQUENTIAL DAMAGES WITH RESPECT TO ANY
 * CLAIMS ARISING FROM THE CONTENT OF SUCH SOFTWARE AND/OR THE USE MADE BY
 * CUSTOMERS OF THE CODING INFORMATION CONTAINED HEREIN IN CONNECTION WITH
 * THEIR PRODUCTS.
 *---------------------------------------------------------------------*/
// GX1_SDK.cpp : Defines the entry point for the console application.
//
#include <windows.h>
#include "I3dmgx1_errors.h"
#include "I3dmgx1_Serial.h"
#include "I3dmgx1_Cmd.h"
#include "I3dmgx1_Utils.h"

int GetComPort(); //prompt user for comport and opens it
int OnGetSerialPorts();
int GetDeviceInfo(int portNum);
void PollData(int portNum); 

int main(int argc, char **argv) {
	
	int i=0;
	BOOL bQuit = FALSE;
	BOOL bPrintHeader = TRUE;
      int portNum = 0;
	int Ccount = 0;
	int  tryPortNum = 1;
	printf("\n   3DM-GX1 Sample Application \n");

	/*-------- If user specifies a port, then use it */
	if (argc > 1) {
		tryPortNum = atoi(argv[1]);
		if (tryPortNum < 2 || tryPortNum > 256) {
			printf("   usage:  i3dmgx1 <portNumber>\n");
			printf("        valid ports are 2..256\n");
			exit(1);
		}

	        /*-------- open a port, map a device */
	        portNum = i3dmgx1_openPort(tryPortNum, 115200, 8, 0, 1, 1024, 1024);
	        if (portNum<0) {
		    printf("   port open failed.\n");
		    printf("   Comm error %d, %s: ", portNum, explainError(portNum));
		    exit(1);
	        }

        }else{
          portNum=OnGetSerialPorts();
          if(portNum<0)
             exit(1);

        }
	printf("\n   Using COM Port #%d \n", portNum);
		while(!bQuit){
		int chOption = 0;

		if(bPrintHeader)
		{
			printf("\n");
			printf("Enter an Option: (P)oll (C)ontinuous (L)og_Continuous (Q)uit\n");
			printf("P Poll           - Poll the node for a single x0E record of data and print it\n");
			printf("D Device Info    - Display the 3DM-GX1 Device Info\n");
			printf("Q Quit           - Quit the application\n");

			bPrintHeader = FALSE;
		}

		//read option from the keyboard
		while(!ReadCharNoReturn(&chOption))
		{
			Sleep(50);
		}

		//
		switch(chOption)
		{
			case 'p':
			case 'P': PollData(portNum); bPrintHeader = TRUE; break;

			case 'D':
			case 'd': GetDeviceInfo(portNum); bPrintHeader = TRUE; break;


			case 'q':
			case 'Q': bQuit = TRUE; break;

			case 'h':
			case 'H': bPrintHeader = TRUE; break;

			default: printf("Invalid Option\n\n"); bPrintHeader = TRUE;
		}
	}
				
	return 0;
}

/* ***************************************************************************************
**  OnGetSerialPorts                   Checks for a valid 3dm-GX1-25 device and if found
**                                     gets a connection and returns the portnumber
**
*****************************************************************************************/
int OnGetSerialPorts()
{
	int i_count, t_count=0;
	unsigned char c_name[20];
    int  i_errorCode = 0;
    int i_portNum = 0;
	char c_check[]="3DM-GX1";
	char Verity[20];
	short modNum;
	
 
	for(i_count=1; i_count<257; i_count++)
	{
		int i_portNum = i3dmgx1_openPort(i_count, 38400, 8, 0, 1, 2048, 2048); //1024
                if (i_portNum>0) {
					i_errorCode = MicroStraingGX1GetModNum( i_portNum, &modNum);

					//printf("returned device identity %i at count %i port %i error %i\n",modNum, i_count, i_portNum, i_errorCode);
				
					if (i_errorCode == 1){
						if(modNum > 0)
			               return i_portNum;
						else
							printf("Model number is %i\n",modNum);
	                 }
					
                       else i3dmgx1_closeDevice(i_count);  
				
                 }// end open port
				
	}// end for loop
	   printf("No 3DM-GX1 devices found, please check connections\n");
       return -1;                
 }

//===========================================================================
// PollData
//---------------------------------------------------------------------------
// Description: Polls the node for a single record of sensor data and prints
//              it to the screen.
//
// Return: HANDLE - handle to the opened comport
//===========================================================================
void PollData(int portNum)
{
	int status = 0, i = 0;
	BYTE Record[11];
	unsigned short sRoll, sPitch, sYaw, ttick, checkS, checkV;
	short ssRoll, ssPitch, ssYaw;
	float roll, pitch, yaw;
	DWORD dRoll;
	float align=((float)360/(float)65536);
	
	 

	status = i3dmGX1_EulerAngles(portNum, &Record[0]);
	if(status == 1)
	{
		
		sRoll = (Record[1]*256)+ Record[2];  //Covert Roll to a 16 bit unsigned integer
		if(sRoll > 32767)                    //Check for rollover and adjust
			ssRoll = sRoll - 65536;
		else ssRoll = sRoll;

		sPitch = (Record[3]*256)+ Record[4];
		if(sPitch > 32767)
			ssPitch = sPitch - 65536;
		else ssPitch = sPitch;

		sYaw = (Record[5]*256)+ Record[6];
		if(sYaw > 32767)
			ssYaw = sYaw - 65536;
		else ssYaw = sYaw;

		ttick = (Record[7]*256)+ Record[8];            //Timer Ticks
		checkS = (Record[9]*256)+ Record[10];         //Checksum value from device

		roll = (float)ssRoll*align;   //apply align == 360/65536
		pitch = (float)ssPitch*align;
		yaw = (float)ssYaw*align;

		checkV=14+sRoll+sPitch+sYaw+ttick;  //add up the 16 bit unsigned ints (roll, pitch, yaw, timer ticks) and cmd (0x0E)
		if (checkV != checkS)
			printf ("Checksum error, packet in error\n");

		else{ //Checksum validation passed 
		     printf("\n");
   		     printf("Roll  %6.2f \n", roll);
		     printf("Pitch %6.2f\n",  pitch);
		     printf("Yaw   %6.2f\n",  yaw);

		     //printf("Timer %i and checksum %i and computed checksum %i\n", ttick, checkS, checkV);
		}
	}
	else
	{
		printf("PollData() Failed\n");
		printf("Error Code: %d\n", status); 
	}
}

int GetDeviceInfo(int portNum){

BYTE Bresponse[7] = {0}; 
unsigned short wChecksum = 0;
unsigned short wCalculatedCheckSum = 0;
char temp[10];
short firmwareNum=0;
short serialNum=0;
short modNum=0;
short majorNum, minorNum, buildNum;
unsigned char SerNum = 0xF1;
unsigned char FirmNum = 0xF0;
char GX1name[10] = "3DM-GX1";
int i =0;
int responseLength = 5; 
unsigned char cmd = 0x28;
unsigned char sendBuff[3];
int status;
int modInt = 234;
sendBuff[0]= cmd;

	  status = sendBuffData( portNum, &SerNum, 1);
	  if (status >= 0 ){
    		status = receiveData(portNum, &Bresponse[0], responseLength);
			serialNum = convert2short(&Bresponse[1]);		    
	  }

	  status =  sendBuffData(portNum, &FirmNum, 1); //Firmware number
	  if (status >= 0 ){
    		status = receiveData(portNum, &Bresponse[0], responseLength);    
			firmwareNum = convert2short(&Bresponse[1]);

		    if (firmwareNum > 0) {
			   /* format for firmware number is #.#.## */
	              majorNum = firmwareNum / 1000;
		          minorNum = (firmwareNum % 1000) / 100;
			      buildNum = firmwareNum % 100;
		          sprintf(temp, "%d.%d.0%d", majorNum, minorNum, buildNum);
		      } 
	  }

	
	sendBuff[1] = (modInt & MSB_MASK) >> 8; //1st byte of EEPROM value to write
	sendBuff[2] =  modInt & LSB_MASK;       //2ond byte of EEPROM value to write
	  	
    status = sendBuffData(portNum, &sendBuff[0], 3);
	responseLength = 7; 
	status = receiveData(portNum, &Bresponse[0], responseLength);
	modNum = convert2short(&Bresponse[1]); 
	
	  
	   printf("\n 3DM-GX1 Device Info: \n");
	            printf("       Model Name     %s\n", GX1name);
	            printf("       Model Number   %d\n", modNum);
	            printf("       Serial Number  %i\n", serialNum);
                printf("       FirmWare Ver   %s\n", temp);

	return 0;
}