#include "i3dmgx1_imu.h"


i3dmgx1_imu::i3dmgx1_imu(HardwareSerial &port, const uint32_t baud): _port (port), _baud (baud)
{
}

i3dmgx1_imu::~i3dmgx1_imu()
{
}

void i3dmgx1_imu::beginComm()
{
	_port.begin(_baud);
}

void i3dmgx1_imu::endComm()
{
	_port.end();
}


/*----------------------------------------------------------------------
* parameters:  command  - a buffer containing multiple bytes corresponding to
*                           the MicroStrain command being issued
*              commandLength - the length of the command buffer
*                              which is the number of bytes to send
*
* returns:    COMM_OK if write/read succeeded
*             COMM_WRITE_ERROR if there was an error writing to port
*--------------------------------------------------------------------*/
int i3dmgx1_imu::sendBuffData(unsigned char *command, int commandLenght)
{
	bool status;
	int totalBytesSent = 0;
	int singleByteSent;

	for (int i = 0; i < commandLenght; i++)
	{
		singleByteSent = _port.write(command[i]);
		totalBytesSent += singleByteSent;
	}											

	//baska alternatif ????????????????
	//bytesSent = _port.write(command, commandLenght);


	if (totalBytesSent != commandLenght)
		return I3DMGX1_COMM_WRITE_ERROR;
	else
		return I3DMGX1_COMM_OK;
}

/*---------------------------------------------------------------------
* parameters:  response - a pointer to a character buffer
*              responseLength - the # of bytes expected for response.
*			   !!! checksum olmayan parametrelerde problem cikabilir. !!!!!!!!!!!
* returns:     COMM_OK if write/read succeeded
*              I3DMGX1_CHECKSUM_ERROR if checksums doesnt match
*              COMM_RLDLEN_ERROR if the length of the response did not
*                            match the number of returned bytes.
*---------------------------------------------------------------------*/
int i3dmgx1_imu::receiveData(unsigned char *response, int responseLength)
{
	int Count = 0;
	int MaxCount = 31;
	int calculatedCheckSum = 0;

	int returnVal = I3DMGX1_COMM_OK;

	for (int i = 0; i < responseLength; i++)
	{
		response[i] = _port.read();
	}

	while (_port.available() > 0 && Count < MaxCount)
	{
		response[Count] = _port.read();
		Count++;
	}

	if (Count != responseLength) /* check for wrong number of bytes returned. */
	{
		returnVal = I3DMGX1_COMM_RDLEN_ERROR;
		#ifdef DEBUG
		//Serial.print("number of bytes read %d and requested were %d\n", Count, responseLength);
		#endif // DEBUG
	}
	else
	{
		calculatedCheckSum = calcChecksum(response, responseLength);
		if (calculatedCheckSum != convert2short(&response[responseLength-2]))
		{
			returnVal = I3DMGX1_CHECKSUM_ERROR;
		}
	}
	return returnVal;
}

int i3dmgx1_imu::GetDeviceInfo()
{
	unsigned char Bresponse[7] = { 0 };
	unsigned short wChecksum = 0;
	unsigned short wCalculatedCheckSum = 0;
	char FirmwareVer[10];
	short firmwareNum = 0;
	short serialNum = 0;
	short modNum = 0;
	short majorNum, minorNum, buildNum;
	char GX1name[10] = "3DM-GX1";
	int i = 0;
	int responseLength = 5;
	unsigned char cmd = 0x28;
	unsigned char sendBuff[3];
	int status;
	int modInt = 234;
	sendBuff[0] = cmd;

	status = sendBuffData(&CMD_SEND_SERIAL_NUMBER, 1);
	if (status >= 0) 
	{
		status = receiveData(&Bresponse[0], responseLength);
		serialNum = convert2short(&Bresponse[1]);
	}

	status = sendBuffData(&CMD_SEND_FIRMWARE_VERSION_NUMBER, 1); //Firmware number
	if (status >= 0) {
		status = receiveData(&Bresponse[0], responseLength);
		firmwareNum = convert2short(&Bresponse[1]);

		if (firmwareNum > 0) {
			/* format for firmware number is #.#.## */
			majorNum = firmwareNum / 1000;
			minorNum = (firmwareNum % 1000) / 100;
			buildNum = firmwareNum % 100;
			sprintf(FirmwareVer, "%d.%d.0%d", majorNum, minorNum, buildNum);
		}
	}

	sendBuff[1] = (modInt & MSB_MASK) >> 8; //1st byte of EEPROM value to write
	sendBuff[2] = modInt & LSB_MASK;       //2ond byte of EEPROM value to write

	status = sendBuffData(&sendBuff[0], 3);
	responseLength = 7;
	status = receiveData(&Bresponse[0], responseLength);
	modNum = convert2short(&Bresponse[1]);

	Serial.print("\n 3DM-GX1 Device Info: \n");
	Serial.print("       Model Name     %s\n");
	Serial.print(GX1name);
	Serial.print("       Model Number   %d\n");
	Serial.print(modNum);
	Serial.print("       Serial Number  %i\n");
	Serial.print(serialNum);
	Serial.print("       FirmWare Ver   %s\n");
	Serial.print(FirmwareVer);

	return 0;
}

/*----------------------------------------------------------------------
* CMD_SEND_GYRO_STABILIZED_EULER_ANGLES	0x0E
* parameters   portNum    : the number of the sensor device (1..n)
*             I3dmGX1Set    : pointer to a struct containing the values
*                             pitch angle in degrees
*
* returns:     errorCode : I3DMGX1_OK if succeeded, otherwise returns
*                          an error code.
*--------------------------------------------------------------------*/
int i3dmgx1_imu::EulerAngles(float* euler_angles) //TODO - buraya acilari uygun formata cevirip pointerin arrayine atmak gerek
{
	int responseLength = 11;
	int status;
	unsigned char Bresponse[11] = { 0 };

	status = sendBuffData(&CMD_SEND_GYRO_STABILIZED_EULER_ANGLES, 1);
	if (status == I3DMGX1_COMM_OK) 
	{
		delay(100);
		status = receiveData(&Bresponse[0], responseLength);
	}
	else
		status = I3DMGX1_COMM_WRITE_ERROR;

	if (status == I3DMGX1_COMM_OK)
	{
		euler_angles[0] = (float)convert2short(&Bresponse[1]) * EULER_SCALE_FACTOR;	// ROLL
		euler_angles[1] = (float)convert2short(&Bresponse[3]) * EULER_SCALE_FACTOR;	// PITCH
		euler_angles[2] = (float)convert2short(&Bresponse[5]) * EULER_SCALE_FACTOR;	// YAW
		euler_angles[3] = (float)convert2short(&Bresponse[7]) * TIMER_SCALE_FACTOR;	// TimerTicks
	}
	return status;
}

int i3dmgx1_imu::EulerAccelRate(unsigned char* euler_accel_rate)
{
	int responseLength = 23;
	int status;
	unsigned char Bresponse[23] = { 0 };

	status = sendBuffData(&CMD_SEND_GYRO_STABILIZED_EULER_ACCEL_RATE_VECTOR, 1);
	if (status == I3DMGX1_COMM_OK)
	{
		status = receiveData(&Bresponse[0], responseLength);
	}
	else
		status = I3DMGX1_COMM_WRITE_ERROR;

	if (status == I3DMGX1_COMM_OK)
	{
		euler_accel_rate[0] = (float)convert2short(&Bresponse[1]) * EULER_SCALE_FACTOR;	// ROLL
		euler_accel_rate[1] = (float)convert2short(&Bresponse[3]) * EULER_SCALE_FACTOR;	// PITCH
		euler_accel_rate[2] = (float)convert2short(&Bresponse[5]) * EULER_SCALE_FACTOR;	// YAW
		euler_accel_rate[3] = (float)convert2short(&Bresponse[1]) * ACCEL_SCALE_FACTOR;	// ACCEL_X
		euler_accel_rate[4] = (float)convert2short(&Bresponse[1]) * ACCEL_SCALE_FACTOR;	// ACCEL_Y
		euler_accel_rate[5] = (float)convert2short(&Bresponse[1]) * ACCEL_SCALE_FACTOR;	// ACCEL_Z
		euler_accel_rate[6] = (float)convert2short(&Bresponse[1]) * GYRO_SCALE_FACTOR;	// CompAngRate_X
		euler_accel_rate[7] = (float)convert2short(&Bresponse[1]) * GYRO_SCALE_FACTOR;	// CompAngRate_Y
		euler_accel_rate[8] = (float)convert2short(&Bresponse[1]) * GYRO_SCALE_FACTOR;	// CompAngRate_Z
		euler_accel_rate[9] = (float)convert2short(&Bresponse[7]) * TIMER_SCALE_FACTOR;	// TimerTicks
	}
	return status;
}

int i3dmgx1_imu::TareCoordinateSystem()
{
	return 0;
}

int i3dmgx1_imu::RemoveTare()
{
	return 0;
}

int i3dmgx1_imu::SetContinuousMode(bool)
{
	return 0;
}

int i3dmgx1_imu::InitializeHardIronFieldCalibration()
{
	return 0;
}

int i3dmgx1_imu::CollectHardIronCalibrationData()
{
	return 0;
}


/*----------------------------------------------------------------------
* Calculate checksum on a received data buffer.
*
* Note: The last two bytes, which contain the received checksum,
*       are not included in the calculation.
*
* parameters:  buffer : pointer to the start of the received buffer.
*              length - the length (in chars) of the buffer.
*
* returns:     the calculated checksum.
*--------------------------------------------------------------------*/
int i3dmgx1_imu::calcChecksum(unsigned char* buffer, int length)
{
	int CHECKSUM_MASK = 0xFFFF;
	int checkSum, i;

	if (length<4)
		return -1;

	checkSum = buffer[0] & LSB_MASK;
	for (i = 1; i<length - 2; i = i + 2) {
		checkSum += convert2short(&buffer[i]);
	}
	return(checkSum & CHECKSUM_MASK);
}

#pragma region Convert functions
/*----------------------------------------------------------------------
* FloatFromBytes
* Converts bytes to Float.
*
* parameters:  pBytes : received buffer containing pointer to 4 bytes
*
* returns:     a float value.
*--------------------------------------------------------------------*/
//float i3dmgx1_imu::FloatFromBytes(const unsigned char* pBytes)
//{
//	float f = 0;
//	if (TestByteOrder() != BIG_ENDIAN) {
//		((unsigned char*)(&f))[0] = pBytes[3];
//		((unsigned char*)(&f))[1] = pBytes[2];
//		((unsigned char*)(&f))[2] = pBytes[1];
//		((unsigned char*)(&f))[3] = pBytes[0];
//	}
//	else {
//		((unsigned char*)(&f))[0] = pBytes[0];
//		((unsigned char*)(&f))[1] = pBytes[1];
//		((unsigned char*)(&f))[2] = pBytes[2];
//		((unsigned char*)(&f))[3] = pBytes[3];
//	}
//
//	return f;
//}

/*----------------------------------------------------------------------
* convert2short
* Convert two adjacent bytes to an integer.
*
* parameters:  buffer : pointer to first of two buffer bytes.
* returns:     the converted value aa a signed short -32 to +32k.
*--------------------------------------------------------------------*/
short i3dmgx1_imu::convert2short(unsigned char* buffer) 
{
	return (buffer[0] << 8) + (buffer[1] & 0xFF);
}

/*----------------------------------------------------------------------
* convert2ushort
* Convert two adjacent bytes to a short.
*
* parameters:  buffer : pointer to first of two buffer bytes.
* returns:     the converted value as a unsigned short 0-64k.
*--------------------------------------------------------------------*/
unsigned short i3dmgx1_imu::convert2ushort(unsigned char* buffer)
{
	return (buffer[0] << 8) + (buffer[1] & 0xFF);
}

/*----------------------------------------------------------------------
* convert2long
* Convert four adjacent bytes to a signed long.
*
* parameters:  buffer : pointer to a 4 byte buffer.
* returns:     the converted value as a signed long.
*--------------------------------------------------------------------*/
long i3dmgx1_imu::convert2long(unsigned char* plbyte) 
{
	return (plbyte[0] << 24) + (plbyte[1] << 16) + (plbyte[2] << 8) + (plbyte[3] & 0xFF);
}

/*----------------------------------------------------------------------
* convert2ulong
* Convert four adjacent bytes to a unsigned long.
*
* parameters:  buffer : pointer to a 4 byte buffer.
* returns:     the converted value as a unsigned long.
*--------------------------------------------------------------------*/
unsigned long i3dmgx1_imu::convert2ulong(unsigned char* plbyte)
{
	return (plbyte[0] << 24) + (plbyte[1] << 16) + (plbyte[2] << 8) + (plbyte[3] & 0xFF);
}
#pragma endregion
