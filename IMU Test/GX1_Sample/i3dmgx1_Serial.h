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
 * i3dmgx1Serial.h
 *
 * Definitions and prototype declarations for the serial port functions.
 *----------------------------------------------------------------------*/

#define MAX_PORT_NUM 500

#define I3DMGX1_COMM_NOPARITY 0
#define I3DMGX1_COMM_ODDPARITY 1
#define I3DMGX1_COMM_EVENPARITY 2

#define I3DMGX1_COMM_ONESTOPBIT 1
#define I3DMGX1_COMM_TWOSTOPBITS 2



/*----------------------------------------------------------------------
 * function prototypes
 *----------------------------------------------------------------------*/

int openPort(int, int, int);
void closePort(int);
int setCommParameters(int, int, int, int, int);
int setCommTimeouts(int, int, int);
int sendBuffData(int, unsigned char *, int);
int receiveData(int, unsigned char*, int);
int purge_port( int);
int MicroStraingGX1GetModNum( int, short *);

/*-------------- end of i3dmGX1Serial.h ---------------------------------*/
