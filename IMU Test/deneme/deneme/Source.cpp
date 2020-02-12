#include <stdio.h>
#include <stdlib.h>

#define abc 71u
#define BIG_ENDIAN      0
#define LITTLE_ENDIAN   1

int sendBuffData(unsigned int *, int);
int receiveData(unsigned int *response, int responseLength);
short convert2short(unsigned int* buffer);
int TestByteOrder();

void main()
{
	unsigned int a[] = { 0x71, 0x72, 0x73 };
	unsigned int b = 54;

	sendBuffData(a, 4);
	sendBuffData(&b, 1);

	auto temp = abc;
	sendBuffData(&temp, 3);

	int responseLength = 5;
	int status;
	unsigned int response[7] = { 0 };
	short serialNum = 0;


	status = receiveData(&response[0], responseLength);
	for (int i = 0; i < responseLength; i++)
	{
		printf("response[%d]: %d \n", i, response[i]);
	}
	serialNum = convert2short(&response[1]);
	printf("serialNum : %d", serialNum);

	unsigned int pfff[5];
	status = receiveData(pfff, 2);

	getchar();
}

int sendBuffData(unsigned int *command, int commandLenght)
{
	bool status;
	int totalBytesSent = 0;
	int singleByteSent;

	for (int i = 0; i < commandLenght; i++)
	{
		printf("buffdata[%d]: %d \n",i, command[i]);
	}

	//baska alternatif ????????????????
	//bytesSent = _port.write(command, commandLenght);


	if (totalBytesSent != commandLenght)
		return 1;
	else
		return 0;
}

int receiveData(unsigned int *response, int responseLength)
{
	int Count = 0;
	int MaxCount = 31;

	int returnVal = 1;

	while (Count < responseLength)
	{
		response[Count] = rand() % 10;
		Count++;
	}

	return returnVal;
}

short convert2short(unsigned int* buffer) 
{
	short x;
	if (TestByteOrder() != BIG_ENDIAN) 
	{
		x = (buffer[0] << 8) + (buffer[1] & 0xFF);
	}
	else {
		x = (short)buffer;
	}
	return x;
}

int TestByteOrder()
{
	short int word = 0x0001;
	char *byte = (char *)&word;
	return(byte[0] ? LITTLE_ENDIAN : BIG_ENDIAN);
}