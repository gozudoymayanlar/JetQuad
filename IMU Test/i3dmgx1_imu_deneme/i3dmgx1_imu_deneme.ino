#include "i3dmgx1_imu.h"

i3dmgx1_imu myImu(Serial1, 34800);

float euler[4] = { 0 };
int result = 0;

void setup() 
{
  // put your setup code here, to run once:
  //myImu.beginComm();
	Serial1.begin(38400);
  delay(200);
  Serial.begin(38400);
}

void loop() {
  // put your main code here, to run repeatedly:
  //myImu.GetDeviceInfo();

	/*result = myImu.EulerAngles(euler);

	Serial.print("\n result:");	Serial.print(result);
	Serial.print("\n roll:");	Serial.print(euler[0]);
	Serial.print("\n pitch:");	Serial.print(euler[1]);
	Serial.print("\n yaw:");	Serial.print(euler[2]);*/

	status = sendBuffData(&CMD_SEND_SERIAL_NUMBER, 1);
	Serial.print("\n\n status:");	Serial.print(status);

	while (Serial1.available() > 0)
	{
		result = Serial1.read();
		Serial.print("\n result:");	Serial.print(result);
	}
	delay(500);
}
