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
 * i3dmgx1_Cmd.h
 *
 * Definitions for the 3dm-gx1 inertia sensor devices
 * The continuous mode functions are supported by this adapter.
 *----------------------------------------------------------------------*/
 
#include <windows.h>

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


/*----------------------------------------------------------------------
 * Sensor communication function prototypes.
 *----------------------------------------------------------------------*/

int i3dmGX1_sendCommand(int , char, char *, int);
void i3dmGX1_closeDevice(int);
int i3dmGX1_openPort(int, int, int, int, int, int, int);

/*----------------------------------------------------------------------
 * 3DM-GX1 Command Function prototypes
 *
 *----------------------------------------------------------------------*/


int i3dmGX1_EulerAngles(int, unsigned char* );			        //0x0E

  
/*-------------- end of i3dmGX1_Cmd.h ----------------------*/
