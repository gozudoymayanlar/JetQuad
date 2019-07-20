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

/*----------------------------------------------------------------------
 * i3dmgx1_Cmd.c
 *
 * 3DM-GX1 Sensor device functions.
 *
 * This platform independent module relies on the communication functions 
 * found in either i3dmGX1SerialWin.c or i3dmgx1SerialLinux.c 
 * (your choice, depending on platform).
 *--------------------------------------------------------------------*/
 
#ifndef DEBUG
#define DEBUG 0
#endif

#include <stdio.h>
#include "i3dmGX1_Cmd.h"
#include "i3dmGX1_Utils.h"
#include "i3dmGX1_Errors.h"
#include "i3dmGX1_Serial.h"


/*----------------------------------------------------------------------
 * i3dmgx1_openPort
 *
 * Open a serial communications port. (platform independent).
 *
 * parameters   portNum  : a port number (1..n).
 *--------------------------------------------------------------------*/

int i3dmgx1_openPort(int portNum, int baudrate, int size, int parity, 
					 int stopbits, int inputBuff, int outputBuff) {
    int errcheck;
    int porth;

    errcheck = openPort(portNum, inputBuff, outputBuff);
    if (errcheck<0) {
        return errcheck;
    }
    porth = errcheck;  /* no error, so this is the port number. */

    /* set communications parameters */
    errcheck = setCommParameters(porth, baudrate, size, parity, stopbits);
    if (errcheck!=I3DMGX1_COMM_OK) {
        return errcheck;
    }

    /* set timeouts */
    errcheck = setCommTimeouts(porth, 50, 50); /* Read Write */
    if (errcheck!=I3DMGX1_COMM_OK) {
        return errcheck;
    }
    return porth;
}
/*----------------------------------------------------------------------
 * i3dmgx1_closeDevice
 *
 * parameters   portNum      : the number of the sensor device 
 *                            
 * Close a device, and also any underlying port.
 *--------------------------------------------------------------------*/
void i3dmgx1_closeDevice(int portNum) {
    
        closePort(portNum);
       portNum = 0;
 }

/*----------------------------------------------------------------------
 * i3dmGX1_EulerAngles	0x0E
 *
 * parameters   portNum    : the number of the sensor device (1..n)
 *             I3dmGX1Set    : pointer to a struct containing the values 
 *                             pitch angle in degrees
 *              
 * returns:     errorCode : I3DMGX1_OK if succeeded, otherwise returns 
 *                          an error code.
 *--------------------------------------------------------------------*/
int i3dmGX1_EulerAngles(int portNum, unsigned char* Bresponse) {
    int responseLength = 11;
	int status;
	unsigned char cmd = 0x0E; //CMD_EULER_ANGLES; /* value is 0x0E */
	
	status = sendBuffData(portNum, &cmd, 1);
    //if (DEBUG) printf("Euler Ang i3dmGX1_send: tx status : %d\n", status);
    if (status == I3DMGX1_COMM_OK) {
    	status = receiveData(portNum, &Bresponse[0], responseLength);   	
	}else
		return status = I3DMGX1_COMM_WRITE_ERROR;
	return status;
 }


/*----------------------------------------------------------------------
 * i3dmGX1_getEEPROMValue	0xE5
 *
 * parameters   portNum    : the number of the sensor device (1..n)
 *              address      : the EEPROM address location
 *              readFlag     : specifies the number of reads required.
 *                             and identifies data type long or float.
 *              value        : the value to get at the address specified
 *
 *
 * returns:     errorCode : I3DMGX1_OK if succeeded, otherwise returns
 *                          an error code.
 *--------------------------------------------------------------------*/
int i3dmGX1_getEEPROMValue(int portNum, char address, int readFlag, int *readval) {
	int status = 0;
	long bytesRead = 0;
	unsigned char Bresponse[5]  = {0};
	unsigned char BFresponse[5]  = {0};
	unsigned char ConvertBuff[4] = {0};
	int responseLength = 5;
	unsigned long nvalL=0;
	unsigned short nvalA=0, nvalB=0;
	float nvalF=0.0;
	unsigned char zoutbuff[4];
	unsigned short wChecksum = 0;
    unsigned short wCalculatedCheckSum = 0;

	zoutbuff[0] = 0xE5;     // EEPROM command identifier
	zoutbuff[1] = 0x00;     // Required identifier
	zoutbuff[2] = 0xFC;     // Required identifier
	zoutbuff[3] = address;  // EEPROM address location

	status = sendBuffData(portNum, &zoutbuff[0], 4);
	if (DEBUG) printf(" eeprom read status is %d\n", status);
    if (status == I3DMGX1_COMM_OK) { 
		status = receiveData(portNum, &Bresponse[0], 5);
        if (status == I3DMGX1_COMM_OK) {
            status = I3DMGX1_OK;
			  wChecksum = convert2ushort(&Bresponse[responseLength-2]);
	          wCalculatedCheckSum = i3dmGX1_Checksum(&Bresponse[0], responseLength-2); //calculate the checkusm, 29 = 31-2 don't include the checksum bytes
			if(wChecksum != wCalculatedCheckSum)
				return status = I3DMGX1_CHECKSUM_ERROR;
			
            nvalA = convert2ushort(&Bresponse[1]);
			
			if (readFlag > 0) {
				ConvertBuff[3] = Bresponse[2];
				ConvertBuff[2] = Bresponse[1];
				zoutbuff[3] = address + 2;

				status = sendBuffData(portNum, &zoutbuff[0], 4);
				if (status != I3DMGX1_COMM_OK)
				   return status;
		
				status = receiveData(portNum, &BFresponse[0], 5);

				if (status == I3DMGX1_COMM_OK) {
					status = I3DMGX1_OK;
					wChecksum = convert2ushort(&Bresponse[responseLength-2]);
					wCalculatedCheckSum = i3dmGX1_Checksum(&BFresponse[0], responseLength-2); //calculate the checkusm, 29 = 31-2 don't include the checksum bytes
					if(wChecksum != wCalculatedCheckSum)
						return status = I3DMGX1_CHECKSUM_ERROR;
					nvalB = convert2ushort(&BFresponse[1]);
					
					ConvertBuff[1] = BFresponse[2];
				    ConvertBuff[0] = BFresponse[1];
					nvalF = FloatFromBytes(&ConvertBuff[0]);
					nvalL = convert2ulong(&ConvertBuff[0]);
					if (readFlag == 1){
						printf("  At addr.:0x%X where value is:   %ld \n", zoutbuff[3], nvalL);
						*readval = nvalL;
					}
					else{
						printf("  At addr.:0x%X where value is: %f \n", zoutbuff[3], nvalF);
						*readval = nvalF;
					}
				}else
					return status = I3DMGX1_COMM_READ_ERROR;
			}else{
					printf("  At addr.:0x%X where value is: %d \t0x%X\n", address, nvalA, nvalA);
					*readval = nvalA;
			}
        }else
            return status = I3DMGX1_COMM_READ_ERROR;
    }
    return status;
}

/*----------------------------------------------------------------------
 * i3dmGX1_getDeviceIdentity  0xEA
 *
 * parameters   portNum : the number of the sensor device (1..n)
 *              flag      : identifier for which Identity to obtain.
 *              id        : a pointer to char string, already allocated.
 *
 * returns:     errorCode : I3DMGX1_OK if succeeded, otherwise returns
 *                          an error code.
 *
 * WARNING - does not check to see if you have allocated enough space
 *           12 bytes for the string to contain the firmware version.
 *--------------------------------------------------------------------*/
int i3dmgx1_getDeviceIdentiy(int portNum, char flag, unsigned char* Bresponse) {
    char cmd = (char) CMD_GET_DEVICE_ID;		//0xEA
    unsigned short wChecksum = 0;
    unsigned short wCalculatedCheckSum = 0;
    int status; 
	int responseLength = 20; 
	unsigned char iden_buff[2];
	
	iden_buff[0] = 0xEA; // Device Identity Command Code
	iden_buff[1] = flag; // Identifier of specific device identity component to obtain

	status = sendBuffData(portNum, &iden_buff[0], 2);
	//if (DEBUG) 
		printf("Get Identity_send: tx status : %d\n", status);
	if (status == I3DMGX1_COMM_OK) {
    	status = receiveData(portNum, &Bresponse[0], responseLength);
		//if (DEBUG) 
			printf("Get Identity i3dmGX1_send: rx status : %d and responseLength %d\n", status, responseLength);
    	if (status==I3DMGX1_COMM_OK) {
			wChecksum = convert2ushort(&Bresponse[responseLength-2]);
			wCalculatedCheckSum = i3dmGX1_Checksum(&Bresponse[0], responseLength-2); //calculate the checkusm, 29 = 31-2 don't include the checksum bytes
			if(wChecksum != wCalculatedCheckSum)
				return	status = I3DMGX1_CHECKSUM_ERROR;
		}else
			return status = I3DMGX1_COMM_READ_ERROR;
	}else return status;
   	
return I3DMGX1_OK;
}
/*----------------------------------------------------------------------
 * i3dmGX1_getFirmwareVersion  0xE9
 *
 * parameters   portNum : the number of the sensor device (1..n)
 *              firmware  : a pointer to char string, already allocated.
 *
 * returns:     errorCode : I3DMGX1_OK if succeeded, otherwise returns
 *                          an error code.
 *
 * WARNING - does not check to see if you have allocated enough space
 *           12 bytes for the string to contain the firmware version.
 *---------------------------------------------------------------------*/

int i3dmGX1_getFirmwareVersion(int portNum, char *firmware) {
    unsigned char cmd = (char) CMD_FIRWARE_VERSION;
    unsigned char Bresponse[7] = {0}; 
	unsigned short wChecksum = 0;
    unsigned short wCalculatedCheckSum = 0;
	
	short firmwareNum=0;
    short majorNum, minorNum, buildNum;
    int status;
	int responseLength = 7; 
	  	
    status = sendBuffData(portNum, &cmd, 1);
    if (DEBUG) printf("FirmWare_send: tx status : %d\n", status);
	if (status == I3DMGX1_COMM_OK) {
		if (responseLength>0) {
    		status = receiveData(portNum, &Bresponse[0], responseLength);
            if (DEBUG) printf("FirmWare i3dmGX1_send: rx status : %d and responseLength %d\n", status, responseLength);
    		if (status==I3DMGX1_COMM_OK) {
                wChecksum = convert2ushort(&Bresponse[responseLength-2]);
	            wCalculatedCheckSum = i3dmGX1_Checksum(&Bresponse[0], responseLength-2); //calculate the checkusm, 29 = 31-2 don't include the checksum bytes
					if(wChecksum != wCalculatedCheckSum)
						return	status = I3DMGX1_CHECKSUM_ERROR;
			}else
					return status = I3DMGX1_COMM_READ_ERROR;
		}

		firmwareNum = convert2short(&Bresponse[3]);
		if (firmwareNum > 0) {
			/* format for firmware number is #.#.## */
	        majorNum = firmwareNum / 1000;
		    minorNum = (firmwareNum % 1000) / 100;
			buildNum = firmwareNum % 100;
		    sprintf(firmware, "%d.%d.%d", majorNum, minorNum, buildNum);
		} 
		return I3DMGX1_OK;
	}else return status;
}
/*-------------- end of i3dmGX1Adapter.c ----------------------*/
