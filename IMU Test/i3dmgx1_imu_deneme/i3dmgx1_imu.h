#include "Arduino.h"

/* Definition of error codes.*/
#define SUCCESS          1
#define SYSTEM_ERROR          -1  //use getlasterror to retrieve status

#define I3DMGX1_COMM_OK 0 
#define I3DMGX1_OK 1

#define I3DMGX1_COMM_FAILED -1
#define I3DMGX1_COMM_INVALID_PORTNUM -2
#define I3DMGX1_COMM_WRITE_ERROR -3
#define I3DMGX1_COMM_READ_ERROR -4
#define I3DMGX1_COMM_RDLEN_ERROR -5
#define I3DMGX1_COMM_RDTIMEOUT_ERROR -6
#define I3DMGX1_CHECKSUM_ERROR -7
#define I3DMGX1_INVALID_DEVICENUM -8
#define I3DMGX1_EERPOM_DATA_ERROR -9
#define I3DMGX1_EERPOM_ADDR_ERROR -10
#define I3DMGX1_GYROSCALE_ERROR -11
#define I3DMGX1_INVALID_CMD_ERROR -12

#define LAST_ERROR -12    /* last error number. */



/* Definitions for the serial port functions.*/
#define MAX_PORT_NUM 500

#define I3DMGX1_COMM_NOPARITY 0
#define I3DMGX1_COMM_ODDPARITY 1
#define I3DMGX1_COMM_EVENPARITY 2

#define I3DMGX1_COMM_ONESTOPBIT 1
#define I3DMGX1_COMM_TWOSTOPBITS 2



/* Definitions for the 3dm-gx1 inertia sensor devices
* The continuous mode functions are supported by this adapter.*/
/* START CMD SET FOR 3DM_gx1 INERTIA DEVICES */
#define CMD_EULER_ANGLES        0x0E
#define CMD_GET_DEVICE_ID       0xEA
#define CMD_FIRWARE_VERSION     0xE9
/* END CMD SET FOR 3DM_GX1 INERTIA DEVICES */

#define I3DMGX1_INSTANT    1
#define I3DMGX1_STABILIZED 2
#define I3DMGX1_GYROSCALE_ADDRESS 130
#define I3DMGX1_GYROGAINSCALE 64

#define MAX_DEVICENUM 512



/* Miscellaneous utility functions used by the 3DM-gx1 Adapter.*/
#define LSB_MASK 0xFF
#define MSB_MASK 0xFF00
#define BIG_ENDIAN      0
#define LITTLE_ENDIAN   1



class i3dmgx1_imu
{
public:
	// constructor and deconstructor declerations for the class
	i3dmgx1_imu(HardwareSerial &port, const uint32_t baud);
	~i3dmgx1_imu();

	// bu degiskenleri define ile yapinca sendbuffData fonksiyonu
	// pointer kullandigi icin problem cikiyordu
	// cunku define ile tanimlanan degiskenler lvalue degilmis
	unsigned char CMD_SEND_FIRMWARE_VERSION_NUMBER = 0xF0;
	unsigned char CMD_SEND_SERIAL_NUMBER = 0xF1;

	void beginComm();
	void endComm();

	/* Prototype declarations for the serial port functions.*/
	int setCommParameters(int, int, int, int, int);
	int setCommTimeouts(int, int, int);
	int sendBuffData(unsigned char *, int);			//ADDED
	int receiveData(unsigned char*, int);				//ADDED
	int purge_port(int);
	int MicroStraingGX1GetModNum(int, short *);



	/* Definitions for the 3dm-gx1 inertia sensor devices
	* The continuous mode functions are supported by this adapter.*/
	/* Sensor communication function prototypes.*/
	int i3dmGX1_sendCommand(int, char, char *, int);
	void i3dmGX1_closeDevice(int);
	int i3dmGX1_openPort(int, int, int, int, int, int, int);
	/* 3DM-GX1 Command Function prototypes */
	int i3dmGX1_EulerAngles(int, unsigned char*);	//0x0E



	/* Miscellaneous utility functions used by the 3DM-gx1 Adapter.*/
	int calcChecksum(unsigned char* buffer, int length);	//ADDED
	short convert2short(unsigned char *);					//ADDED
	unsigned short convert2ushort(unsigned char *);			//ADDED
	unsigned long convert2ulong(unsigned char *);			//ADDED
	long convert2long(unsigned char*);						//ADDED
	int TestByteOrder();									//ADDED
	float FloatFromBytes(const unsigned char*);				//ADDED
	
	float Little_Endian_Float(unsigned char* p);
	char * explainError(int);
	bool ReadCharNoReturn(int*);

	int GetDeviceInfo();									//ADDED

private:
	HardwareSerial &_port;
	const uint32_t _baud;
	uint16_t i3dmGX1_Checksum(unsigned char *, int);		//ADDED

};
