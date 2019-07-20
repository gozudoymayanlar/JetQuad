/*----------------------------------------------------------------------
 *
 * I3DM-gx1 Interface Software
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
 * i3dmgx1Utils.h
 *
 * Miscellaneous utility functions used by the 3DM-gx1 Adapter.
 *--------------------------------------------------------------------*/

#define LSB_MASK 0xFF
#define MSB_MASK 0xFF00
#define BIG_ENDIAN      0
#define LITTLE_ENDIAN   1

#include <stdio.h>
#include <windows.h>

short convert2short(unsigned char *);
unsigned short convert2ushort(unsigned char *);
unsigned long convert2ulong(unsigned char *);
long convert2long(unsigned char*);
int calcChecksum(unsigned char *, int);
WORD i3dmGX1_Checksum( unsigned char *, int);
float FloatFromBytes(const unsigned char*); 
float Little_Endian_Float(unsigned char* p);
char * explainError(int);
int TestByteOrder();
BOOL ReadCharNoReturn(int* );

/*-------------- end of i3dmgx1Utils.h ----------------------*/
