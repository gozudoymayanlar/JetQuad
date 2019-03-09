#include <Wire.h>
#include <Adafruit_PWMServoDriver.h>
#include "HX711.h"

HX711 scaleFuel(PA4, PA5); // HX711.DOUT  - pin #PB4,   HX711.PD_SCK - pin #PB5   !!!!  BUNU GÜNCELLEDİM. MOTORLA HABERLEŞECEK SERIAL TX2 RX2 PİNLERİ İLE ÇAKIŞIYORDU.
HX711 scaleThrust(PA6,PA7); // HX711.DOUT  - pin #PB6,   HX711.PD_SCK - pin #PB7
Adafruit_PWMServoDriver pwm = Adafruit_PWMServoDriver(); // called this way, it uses the default address 0x40

//===== V A R I A B L E S ==========
const byte numChars = 15;
char receivedChars[numChars];
// variables to hold the parsed data
boolean newData = false;
boolean dataContinues = false;

int MotorStatus = 0, KumandaThrottle = 0, KumandaTrim = 0, MotorThrottle = 0, 
    MotorRPM = 0, EGT = 0, ReferenceRPM = 0;
float Thrust = 0, BatteryVoltage = 0, PumpVoltage = 0, Fuel = 0;
char startMarker[10] = {'"', '!', '^', '+', '%', '&', '/', '(', ')'};
char endMarker = '>';
char rc;
char uMarker;
int x;

//===== F U N C T I O N   P R O T O T Y P E S ==========
void data_update(int receivedChars, char sMarker);
void recvWithStartEndMarkers();
void RAC();
//===============

void setup()
{
  Serial.begin(38400); // normal usart to connect to pc pins PA9=TX1 & PA10=RX1 on stm32f103 board
  Serial1.begin(38400); // 2nd usart to connect to Jetcat Jet motor pins PA2=TX2 & PA3=RX2 on stm32f103 board
  //serial2.begin()... 3rd usart PB10=TX3 & PB11=RX3
  pwm.begin();
  pwm.setPWMFreq(100);  // kumandanın throttle/trim frekansı 100 Hz olarak belirlenmiş.
                        // throttle 1ms ile 2ms arası çalışacak. Arayüzden % cinsinden geliyor.
                        // trim -0.15 ms ile +0.15ms arası çalışacak. Arayüzden % cinsinden geliyor.

  //  Serial.println("Before setting up the scale:");
  //  Serial.print("read: \t\t");
  //  Serial.println(scale.read());     // print a raw reading from the ADC

  scaleFuel.read_average(20);
  scaleFuel.set_scale(-222.7f);   // BUNA NEYE GÖRE KARAR VERİLİYOR !!!-----------> KALİBRE EDİLDİ
  scaleFuel.tare();               // reset the scale to 0

  scaleThrust.read_average(20);
  scaleThrust.set_scale(-98.28); //-103.4f);      // BUNA NEYE GÖRE KARAR VERİLİYOR !!! -----------> KALİBRE EDİLDİ
  scaleThrust.tare();             // reset the scale to 0
  delay(10);
}

void loop()
{
  //  pwm.setPWM(15, 0, 'DEGER');
  //  Serial.println(scale.get_units(), 0);
  Fuel = scaleFuel.get_units();
  Thrust = scaleThrust.get_units();
  RAC();
  recvWithStartEndMarkers();

  Serial.print("%"); Serial.print(MotorStatus);
  Serial.print("#"); Serial.print(KumandaThrottle);
  Serial.print("#"); Serial.print(KumandaTrim);
  Serial.print("#"); Serial.print(MotorThrottle);
  Serial.print("#"); Serial.print(9.81*Thrust/1000);
  Serial.print("#"); Serial.print(ReferenceRPM);
  Serial.print("#"); Serial.print(MotorRPM);
  Serial.print("#"); Serial.print(EGT);
  Serial.print("#"); Serial.print(BatteryVoltage);
  Serial.print("#"); Serial.print(PumpVoltage);
  Serial.print("#"); Serial.println(Fuel);
  delay(20);
}

//============
void data_update(int receivedChars, char sMarker)
{
  if (sMarker == startMarker[0])
    KumandaThrottle = receivedChars;
  else if (sMarker == startMarker[1])
    KumandaTrim = receivedChars;

  // pwm kütüphanesi normalde 0 ile 4095 arası değer kabul ediyor.
  // arayüzden gelen % cinsinden throttle değerini 1 ile 2ms arasına map etmek için
  //    kütüphanenin 410 ile 819 değerleri arasına map ettim.
  // arayüzden gelen % cinsinden trim değerini -0.15 ile 0.15ms arasına map etmek için
  //    kütüphanenin -62 ile 62 değerleri arasına map ettim.
  // HESAP => 10ms/4095 =~ 0.0024. =>>  1ms/0.0024 = 410, 2/0.0024 = 819
  pwm.setPWM(15, 0, map(KumandaThrottle,0,100,410,819) + map(KumandaTrim,0,100,-62,62)); 
}
//============
void recvWithStartEndMarkers()
{
  boolean recvInProgress = false;
  byte ndx = 0;
  char rc;

  while (Serial.available() > 0)
  {
    while (newData == false)
    {
      rc = Serial.read();
      if (recvInProgress == true)
      {
        if (rc != endMarker)
        {
          receivedChars[ndx] = rc;
          ndx++;
          if (ndx >= numChars)
          {
            ndx = numChars - 1;
          }
        }
        else
        {
          receivedChars[ndx] = '\0'; // terminate the string
          recvInProgress = false;
          ndx = 0;
          newData = true;
        }
      }
      else if (rc == '"' || rc == '!' || rc == '\'' || rc == '^' || rc == '+' ||
               rc == '%' || rc == '&' || rc == '/' || rc == '(' || rc == ')')
      {
        recvInProgress = true;
        uMarker = rc;
      }
    }
    data_update(atoi(receivedChars), uMarker);
    newData = false;
  }
}
//============
void RAC()
{
  char receivedRaw[70];
  int commas[11] = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
  int crs[3] = {0,0,0};
  String raw = "";
  String instruct = "1,RAC,1\r"; // read actual values

  Serial1.print(instruct);
  delay(100);  // ecunun datayı tam olarak göndermesini bekler

  int i = 0;
  while (Serial1.available() > 0)
  { 
    receivedRaw[i] = Serial1.read();
    i++;
  }
  raw = receivedRaw;

  crs[0] = raw.indexOf('\r');   //birinci carriage return pozisyonunu bul
  crs[1] = raw.indexOf('\r',crs[0] + 1); //ikinci carriage return pozisyonunu bul
    
  raw.remove(crs[1]+1);  // ikinci cr den sonrasını sil
  if (raw.startsWith("1,RAC,1\r1,HS,OK,") && raw.endsWith("\r"))       //datanın doğru gelip gelmediğini kontrol eder
  {
    //Serial.print("data doğru,");
    for (int i = 0; i < 11; i++)
    {
      commas[i + 1] = raw.indexOf(',', commas[i] + 1);  // virgullerin pozisyonunu bul
    }
    //virgullere göre gerekli veriyi al
    String strRPM = raw.substring(commas[5] + 1, commas[6]); MotorRPM = strRPM.toInt();
    String strEGT = raw.substring(commas[6] + 1, commas[7]); EGT = strEGT.toInt();
    String strPUMP = raw.substring(commas[7] + 1, commas[8]); PumpVoltage = strPUMP.toFloat();
    String strSTATE = raw.substring(commas[8] + 1, commas[9]); MotorStatus = strSTATE.toInt();
    String strTHR = raw.substring(commas[9] + 1, commas[10]); MotorThrottle = strTHR.toInt();
  }
  else
  { // data yanlışsa bufferı boşalt
    //Serial.print("data yanlış");
    MotorStatus = 20;
    while (Serial1.available() > 0)
    { 
      Serial1.read();
    }
  }
}
