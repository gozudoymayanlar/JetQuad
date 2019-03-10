// push pull deneme
  // ZAZAAZAZAZAZAZAZAZAZAZAZAZA
#define led 0x41
#define led_size 0x01
#define id 0x01

void setup() {
  // put your setup code here, to run once:
  Serial.begin(9600);
}

void loop() {
  // put your main code here, to run repeatedly:
//  Serial.write(240);
//  Serial.print("\t");
//  Serial.write("240");
//  Serial.print("\t");
//  Serial.write(0xf0);
//  Serial.print("\t");
//
//  Serial.print(240);
//  Serial.print("\t");
//  Serial.print("240");
//  Serial.print("\t");
//  Serial.print(0xf0);
//  Serial.print("\t");
//  Serial.println("0xf0");
  uint8_t yazilacakVeri[0x02] = {0x03};
  uint8_t sz = sizeof(yazilacakVeri);
  Serial.print(yazilacakVeri[0],HEX);
  Serial.print("\tSize:");
  Serial.println(sz);  
  delay(1000);

  // ZAZAAZAZAZAZAZAZAZAZAZAZAZA
}
