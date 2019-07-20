// KABLOLAMADA UNUTULMAYACAK ŞEYLER !!!
// Farklı güç kaynaklarının topraklarını birleştirmeyi unutma. Acı tecrübeyle sabittir.
//

// PROGRAMLAMADA UNUTULMAYACAK ŞEYLER !!!
// if bloğunun içinde 2 tane eşittir (==) kullanılmalı
// if bloğunun dışında 1 tane eşittir (=) kullanılmalı

//stm32f103C'ile denenecek kod
//#define DEBUG

#include "DynamixelServo.h"

const uint32_t serialBaud = 115200;
const uint32_t servoBaud = 1000000;
DynamixelServo servo(Serial1, servoBaud, PA4);

unsigned int integerValue = 0; // Max value is 65535
char incomingByte;
long golpos = 0;
uint8_t goalPos1;
uint8_t goalPos2;
int temper = 0;
int err = 0;

void setup()
{
  servo.beginComm();
  delay(500);
#ifndef DEBUG // debug modunda değilse
  Serial.begin(serialBaud);
  //  Serial.println(); Serial.println();
  //  Serial.println("Read Instructions Initializing...");
#else // debug modundaysa
  Serial.println("DEBUG MODE ON...");
#endif

  //ID1 iç eksen
  //ID2 dış eksen
  servo.write_torque(1, 1);
  //  servo.write_acc_prof(1,455);  servo.write_acc_prof(2,345);      bunklar 0.7 kasnak oranı için geçerliydi!!!
  //  servo.write_vel_prof(1,120);  servo.write_vel_prof(2,120);

  servo.write_acc_prof(1, 240);   servo.write_acc_prof(2, 150);
  servo.write_vel_prof(1, 84);    servo.write_vel_prof(2, 84);
}

void loop()
{

  //servo.write_position(1,0);
  //delay(1000);
  //servo.write_position(1,2048);
  delay(1000);
  temper = servo.read_temperature(1);
  err = servo.read_hardware_error(1);
  Serial.print(temper);
  Serial.print('\t');
  Serial.println(err);
}
