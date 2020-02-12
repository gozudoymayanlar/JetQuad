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

#define EULER_SCALE_FACTOR	(360/65536)
#define TIMER_SCALE_FACTOR	6.5536
#define ACCEL_SCALE_FACTOR	1		//TODO - BUNLARIN DEGERLERI BULUNACAK
#define GYRO_SCALE_FACTOR	1		// EEPROM OKUMA FONKSIYONU GEREKEBILIR.

class i3dmgx1_imu
{
public:
	// constructor and deconstructor declerations for the class
	i3dmgx1_imu(HardwareSerial &port, const uint32_t baud);
	~i3dmgx1_imu();


	/* Definitions for the 3dm-gx1 inertia sensor devices
	* The continuous mode functions are supported by this adapter.
	* bu degiskenleri define ile yapinca sendbuffData fonksiyonu
	* pointer kullandigi icin problem cikiyordu
	* cunku define ile tanimlanan degiskenler lvalue degilmis*/
	const unsigned char CMD_SEND_FIRMWARE_VERSION_NUMBER = 0xF0;
	const unsigned char CMD_SEND_SERIAL_NUMBER = 0xF1;
	const unsigned char CMD_SEND_GYRO_STABILIZED_EULER_ANGLES = 0x0E;
	const unsigned char CMD_TARE_COORDINATE_SYSTEM = 0x0F;
	const unsigned char CMD_REMOVE_TARE = 0x11;
	const unsigned char CMD_SET_CONTINUOUS_MODE = 0X10;
	const unsigned char CMD_SEND_GYRO_STABILIZED_EULER_ACCEL_RATE_VECTOR = 0x31;
	const unsigned char CMD_INITIALIZE_HARD_IRON_FIELD_CALIBRATION = 0x40;
	const unsigned char CMD_COLLECT_HARD_IRON_CALIBRATION_DATA = 0x41;

	
	/* Prototype declarations for the serial port functions.*/
	void beginComm();
	void endComm();
	int sendBuffData(unsigned char *, int);
	int receiveData(unsigned char*, int);


	/* Definitions for the 3dm-gx1 inertia sensor devices
	* The continuous mode functions are supported by this adapter.*/
	/* Sensor communication function prototypes.*/
	/* 3DM-GX1 Command Function prototypes */
	int GetDeviceInfo();
	int sendCommand(int, char, char *, int);	// TODO - BU IMPLEMENTE EDILEBILIR
	int EulerAngles(float*);					// TODO - BU IMPLEMENTE EDILECEK
	int EulerAccelRate(unsigned char*);			// TODO - BU IMPLEMENTE EDILECEK
	int TareCoordinateSystem();					// TODO - BU IMPLEMENTE EDILECEK
	int RemoveTare();							// TODO - BU IMPLEMENTE EDILECEK
	int SetContinuousMode(bool);				// TODO - BU IMPLEMENTE EDILECEK
	int InitializeHardIronFieldCalibration();	// TODO - BU IMPLEMENTE EDILECEK
	int CollectHardIronCalibrationData();		// TODO - BU IMPLEMENTE EDILECEK
	
	

	/* Miscellaneous utility functions used by the 3DM-gx1 Adapter.*/
	//int calcChecksum(unsigned char* buffer, int length);
	short convert2short(unsigned char *);
	unsigned short convert2ushort(unsigned char *);
	unsigned long convert2ulong(unsigned char *);
	long convert2long(unsigned char*);
	float FloatFromBytes(const unsigned char*);
	char * explainError(int);							// TODO: - bu implemente edilebilir

private:
	HardwareSerial &_port;
	const uint32_t _baud;
	int calcChecksum(unsigned char *, int);				//TODO - bu implemente edilebilir

};
