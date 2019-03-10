#include "DynamixelServo.h"

const unsigned long serialBaud = 9600;

const unsigned long servoBaud = 57600;
DynamixelServo servo(Serial1,servoBaud);

void setup() 
{
  Serial.begin(serialBaud);
  servo.beginComm();

  Serial.println("Read Instructions Initializing...");
}

void loop() 
{
  uint8_t gelenVeri[Goal_Position_size];
  servo.read_raw(broadcastID_, Goal_Position, gelenVeri);
  
  // say what you got:
  Serial.print("received: ");
  for (int i = 0; i < Goal_Position_size + 11; i++)
  {
    Serial.print(gelenVeri[i], HEX); Serial.print(" ");
  }
  Serial.print("\r\n");
  delay(25);

  uint8_t yazilacakVeri[LED_size] = {0x01};
  servo.write_raw(broadcastID_, LED, yazilacakVeri);
  delay(450);
  
  yazilacakVeri[0] = {0x00};
  servo.write_raw(broadcastID_, LED, yazilacakVeri);
  delay(25);
}
