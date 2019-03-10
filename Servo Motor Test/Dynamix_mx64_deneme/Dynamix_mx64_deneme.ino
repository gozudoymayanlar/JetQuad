#include "DynamixelServo.h"

const unsigned long serialBaud = 9600;

const unsigned long servoBaud = 9600;
DynamixelServo servo(Serial,servoBaud);

void setup() 
{
//  pinMode(PA4,OUTPUT);
  //digitalWrite(PA4,LOW);
  
  //Serial.begin(serialBaud);
  servo.beginComm();

  //Serial.println("Read Instructions Initializing...");
}

void loop() 
{
  uint8_t gelenVeri[Goal_Position_size];
  //servo.read_raw(broadcastID_, Goal_Position, gelenVeri);
  
  // say what you got:
  //Serial.print("received: ");
  for (int i = 0; i < Goal_Position_size + 11; i++)
  {
    //Serial.print(gelenVeri[i], HEX); Serial.print(" ");
  }
  //Serial.print("\r\n");
  //delay(25);

  uint8_t yazilacakVeri[LED_size] = {0x01};
  servo.write_raw(broadcastID_, LED, yazilacakVeri, LED_size);
  //Serial.println(sizeof(yazilacakVeri));
  delay(500);
  
  yazilacakVeri[0] = {0x00};
  servo.write_raw(broadcastID_, LED, yazilacakVeri, LED_size);
  //Serial.println("");
  delay(500);
}
