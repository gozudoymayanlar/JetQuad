#include "i3dmgx1_imu.h"

i3dmgx1_imu myImu(Serial1, 34800);

void setup() 
{
  // put your setup code here, to run once:
  myImu.beginComm();
  delay(200);
  Serial.begin(38400);
}

void loop() {
  // put your main code here, to run repeatedly:
  myImu.GetDeviceInfo();
}
