#define servo_mx64
#include "DynamixelServo.h"

const uint32_t serialBaud = 9600;
const uint32_t servoBaud = 9600;
DynamixelServo servo(Serial,servoBaud, A4);

void setup() 
{
  servo.beginComm();
  
  #ifndef DEBUG // debug modunda değilse
  Serial.begin(serialBaud);
  Serial.println("Read Instructions Initializing...");
  #else // debug modundaysa
  Serial.println("DEBUG MODE ON...");
  #endif
}

void loop() 
{
 #ifdef DEBUG
  Serial.println(); Serial.print("konum al:");
 #endif
  // Konum al
  uint8_t veri[Present_Position_size];
  servo.read_raw(broadcastID, Present_Position, veri, Present_Position_size);
  
  // konum verisini okunabilir şekilde pc serial portuna yaz
  //Serial.println();  Serial.print("received: ");
  for (int i = 0; i < Present_Position_size + 11; i++)
  {
    Serial.print(veri[i], HEX); Serial.print("\t");
  }
  delay(500);

  // LED i aç
  #ifdef DEBUG
  Serial.println(); Serial.print("led on: ");
  #endif
  uint8_t yazilacakVeri[LED_size] = {0x01};
  servo.write_raw(broadcastID, LED, yazilacakVeri, LED_size);
  delay(500);

 #ifdef DEBUG
  Serial.println(); Serial.print("led off: ");
 #endif
  // LED i kapa
  yazilacakVeri[0] = {0x00};
  servo.write_raw(broadcastID, LED, yazilacakVeri, LED_size);
  delay(2000);
}
