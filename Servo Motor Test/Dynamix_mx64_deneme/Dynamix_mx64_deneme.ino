//denemeeeee
#define DEBUG  // debug yapmak için bunu uncomment et.
#define servo_mx64
#include "DynamixelServo.h"

const uint32_t serialBaud = 9600;
const uint32_t servoBaud = 9600;
DynamixelServo servo(Serial,servoBaud, PA4);

void setup() 
{
  #ifndef DEBUG // debug modunda değilse
  Serial.begin(serialBaud);
  Serial.println("Read Instructions Initializing...");
  #else // debug modundaysa
  Serial.println("DEBUG MODE ON...");
  #endif
  
  servo.beginComm();
}

void loop() 
{
  // Konum al
  uint8_t veri[Present_Position_size];
  servo.read_raw(broadcastID, Present_Position, veri, Present_Position_size);

 #ifdef DEBUG
  Serial.print("\t");
 #endif
  
  // konum verisini okunabilir şekilde pc serial portuna yaz
  Serial.print("received: ");
  for (int i = 0; i < Present_Position_size + 11; i++)
  {
    Serial.print(veri[i], HEX); Serial.print("\t");
  }
  Serial.println();
  delay(25);

  // LED i aç
  uint8_t yazilacakVeri[LED_size] = {0x01};
  servo.write_raw(broadcastID, LED, yazilacakVeri, LED_size);
  delay(500);

 #ifdef DEBUG
  Serial.print("\t");
 #endif

  // LED i kapa
  yazilacakVeri[0] = {0x00};
  servo.write_raw(broadcastID, LED, yazilacakVeri, LED_size);
 
 #ifdef DEBUG
  Serial.print("\t");
 #endif
  delay(2000);
}
