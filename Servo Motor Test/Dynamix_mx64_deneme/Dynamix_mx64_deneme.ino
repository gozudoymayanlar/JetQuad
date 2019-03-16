// KABLOLAMADA UNUTULMAYACAK ŞEYLER !!!
// Farklı güç kaynaklarının topraklarını birleştirmeyi unutma. Acı tecrübeyle sabittir.
// 

// PROGRAMLAMADA UNUTULMAYACAK ŞEYLER !!!
// if bloğunun içinde 2 tane eşittir (==) kullanılmalı
// if bloğunun dışında 1 tane eşittir (=) kullanılmalı

//stm32f103C'ile denenecek kod
//#define DEBUG
#define servo_mx64
#include "DynamixelServo.h"

const uint32_t serialBaud = 9600;
const uint32_t servoBaud = 57600;
DynamixelServo servo(Serial1,servoBaud, 2);

unsigned int integerValue=0;  // Max value is 65535
char incomingByte;
uint8_t goalPositionVeri[Goal_Position_size] = {0xFF, 0x0F, 0x00, 0x00};
uint8_t goalPos1;
uint8_t goalPos2;

void setup() 
{
  delay(2000);
  servo.beginComm();
  delay(2000);
  
  #ifndef DEBUG // debug modunda değilse
  Serial.begin(serialBaud);
  Serial.println(); Serial.println();
  Serial.println("Read Instructions Initializing...");
  #else // debug modundaysa
  Serial.println("DEBUG MODE ON...");
  #endif

  uint8_t torqueEnableVeri[Torque_Enable_size] = {0x01};
  servo.write_raw(0x01, Torque_Enable, torqueEnableVeri, Torque_Enable_size);
}

void loop() 
{
  // Konum al
  Serial.println();
  uint8_t veri[Present_Position_size];
  servo.read_raw(0x01, Present_Position, veri, Present_Position_size);

 #ifdef DEBUG
  Serial.print("\t");
 #endif
  
//  //konum verisini okunabilir şekilde pc serial portuna yaz
//  Serial.println(); Serial.print("received: ");
//  for (int i = 0; i < Present_Position_size; i++)
//  {
//    Serial.print(veri[i], HEX); Serial.print("\t");
//  }
//  Serial.println();

  delay(2000);

//  // LED i aç
//  uint8_t yazilacakVeri[LED_size] = {0x01};
//  servo.write_raw(0x01, LED, yazilacakVeri, LED_size);
//  delay(2000);
//
// #ifdef DEBUG
//  Serial.print("\t");
// #endif

  // goalPosition yaz
  servo.write_raw(0x01, Goal_Position, goalPositionVeri, Goal_Position_size);
//
//  delay(2000);
//  
//  // LED i kapa
//  yazilacakVeri[0] = {0x00};
//  servo.write_raw(0x01, LED, yazilacakVeri, LED_size);
//
//  delay(2000);
//
//  // goalPosition yaz
//  goalPositionVeri[0] = 0x00;  goalPositionVeri[1] = 0x00;
//  servo.write_raw(0x01, Goal_Position, goalPositionVeri, Goal_Position_size);
//  
////  torqueEnableVeri[0] = {0x00};
////  servo.write_raw(0x01, Torque_Enable, torqueEnableVeri, Torque_Enable_size);
// #ifdef DEBUG
//  Serial.print("\t");
// #endif
// 
//  delay(2000);
if (Serial.available() > 0) {   // something came across serial
    integerValue = 0;         // throw away previous integerValue
    while(1) {            // force into a loop until 'n' is received
      incomingByte = Serial.read();
      if (incomingByte == '\n') break;   // exit the while(1), we're done receiving
      if (incomingByte == -1) continue;  // if no characters are in the buffer read() returns -1
      integerValue *= 10;  // shift left 1 decimal place
      // convert ASCII to integer, add, and shift left 1 decimal place
      integerValue = ((incomingByte - 48) + integerValue);
    }
    // Do something with the value
    goalPos1 = (integerValue & 0x00FF);
    goalPos2= (integerValue >> 8) & 0x00FF;
    goalPositionVeri[0] = goalPos1;
    goalPositionVeri[1] = goalPos2;
  }
}

 //FF FF  FD  0 1 4 0 55  0 A1  C 
 //7F  1 0 3 0 0  0 BF  84  7F  1 0 
