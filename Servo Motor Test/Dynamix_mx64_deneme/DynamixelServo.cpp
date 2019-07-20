#include "Arduino.h"
#include "DynamixelServo.h"

DynamixelServo::DynamixelServo(HardwareSerial &port, const uint32_t baud, const int dataControlPin): _port (port), _baud (baud)
{
  this->dataControlPin = dataControlPin;
}

void DynamixelServo::beginComm()
{
  pinMode(dataControlPin, OUTPUT);
  digitalWrite(dataControlPin, LOW);
  _port.begin(_baud);
}

void DynamixelServo::end()
{
  _port.end();
}

//  llllll    lllllll
//  ll   ll   ll
//  lllll     lllllll
//  ll  ll    ll
//  ll   ll   lllllll

uint8_t DynamixelServo::read_raw(uint8_t Id, uint8_t address, uint8_t datas[], const int datas_size)
{
  //															                                 len2, inst     , add_l  ,add_h, dataLen_l , dataLen_h
  uint8_t TxPacket[14] = { H1, H2, H3, RSRV, Id, ReadPacketLength, 0x00, Read_inst, address, 0x00, datas_size,   0x00   ,  CRC_L, CRC_H };
  CRC = update_crc(0, TxPacket, 12);	// 12 = 5 + Packet Length(7)
  CRC_L = (CRC & 0x00FF);				//Little-endian
  CRC_H = (CRC >> 8) & 0x00FF;
  TxPacket[12] = CRC_L;
  TxPacket[13] = CRC_H;

#ifndef DEBUG // debug modunda değilse

  digitalWrite(dataControlPin, HIGH);
  clearRXbuffer();
  for (int i = 0; i < 14; i++)
  {
    _port.write(TxPacket[i]);  // write fonksiyonuyla byte byte gönder
  }
  //noInterrupts();
  _port.flush();
  digitalWrite(dataControlPin, LOW);
  //interrupts();

#else
  _port.println(); _port.print("Read TxPacket: ");
  for (int i = 0; i < 14; i++)  // print fonksiyonuyla ascii olarak gönder ki okunabilir olsun
  {
    _port.print(TxPacket[i], HEX); _port.print("\t");
  }
#endif

  const int statusPacket_size = 11 + datas_size;
  uint8_t statusPacket[statusPacket_size];
  for (uint8_t q = 0; q < statusPacket_size; q++)
  {
    statusPacket[q] = q;
  }
  uint8_t incomingByte;
  uint8_t Time_Counter = 0;

  delay(readDelay);
  while (_port.available() > 0)
  {
    incomingByte = _port.read();
    if (incomingByte == H1 && (_port.peek() == H2))
    {
      statusPacket[0] = incomingByte;
      incomingByte = _port.read();
      if (incomingByte == H2 && (_port.peek() == H3))
      {
        statusPacket[1] = incomingByte;
        for (int q = 2; q < statusPacket_size; q++)
        {
          statusPacket[q] = _port.read();
        }
      }
    }
  }
  
  CRC = update_crc(0, statusPacket, statusPacket_size - 2);
  CRC_L = (CRC & 0x00FF);
  CRC_H = (CRC >> 8) & 0x00FF;

  if ((statusPacket[statusPacket_size - 2] == CRC_L) && (statusPacket[statusPacket_size - 1] == CRC_H) && (statusPacket[8] == 0))//chksm doğru ve error 0
  {
    data_okey = true;
    for (int i = 9; i < statusPacket_size - 2; i++)
    {
      params[i - 9] = statusPacket[i];
    }
  }
}

long DynamixelServo::read_position(uint8_t Id)
{
  uint8_t veri[4];
  read_raw(Id, 0x84, veri, 4);

  long sonuc = 0;
  sonuc += (long)params[3] << 24;
  sonuc += (long)params[2] << 16;
  sonuc += (long)params[1] << 8;
  sonuc += (long)params[0];
  if (data_okey)
  {
    data_okey = false;
    params[0] = 0; params[1] = 0; params[2] = 0; params[3] = 0;
    return sonuc;
  }
  else
  {
    return -1000;
  }
}

int DynamixelServo::read_hardware_error(uint8_t Id)
{
  uint8_t veri[1];
  read_raw(Id, 0x46, veri, 1);

  uint8_t sonuc;
  if(params[0] == 0) sonuc = 0;
  else sonuc = 1;
  

  if (data_okey)  
  {
    data_okey = false;
    params[0] = 0;
    return sonuc;
  }
  else
  {
    return -1;
  }
}

int DynamixelServo::read_temperature(uint8_t Id)
{
  uint8_t veri[1];
  read_raw(Id, 0x92, veri, 1);

  uint8_t sonuc = params[0];

  if (data_okey)
  {
    data_okey = false;
    params[0] = 0;
    return sonuc;
  }
  else
  {
    return -1000;
  }
}


///////////////////~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~



void DynamixelServo::write_raw(uint8_t Id, uint8_t address, uint8_t datas[], int datas_size)
{
  int packetSize = 12 + datas_size;
  uint8_t TxPacket[packetSize];//                                 len2, inst      , add_l  , add_h
  uint8_t basePacket[] = { H1, H2, H3, RSRV, Id, datas_size + 5 , 0x00, Write_inst, address, 0x00 };

  for (int q = 0; q < 10; q++)
  {
    TxPacket[q] = basePacket[q];
  }

  for (int i = 0; i < datas_size; i++)
  {
    TxPacket[i + 10] = datas[i];
  }

  TxPacket[packetSize - 2] = CRC_L;
  TxPacket[packetSize - 1] = CRC_H;
  CRC = update_crc(0, TxPacket, packetSize - 2);
  CRC_L = (CRC & 0x00FF);
  CRC_H = (CRC >> 8) & 0x00FF;
  TxPacket[packetSize - 2] = CRC_L;
  TxPacket[packetSize - 1] = CRC_H;

#ifndef DEBUG  // debug modunda değilse
  clearRXbuffer();

  digitalWrite(dataControlPin, HIGH);
  for (int i = 0; i < packetSize; i++)
  {
    _port.write(TxPacket[i]);  // write fonksiyonuyla byte byte gönder
  }
  //noInterrupts();
  _port.flush();
  digitalWrite(dataControlPin, LOW);
  //interrupts();

#else  // debug modundaysa
  _port.println(); _port.print("Write TxPacket: ");
  for (int i = 0; i < packetSize; i++)  // print fonksiyonuyla ascii olarak gönder ki okunabilir olsun
  {
    _port.print(TxPacket[i], HEX); _port.print("\t");
  }
#endif

    const int statusPacket_size = 11;
    uint8_t statusPacket[statusPacket_size] = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11};
    uint8_t incomingByte;
    uint8_t Time_Counter = 0;
  
    delay(writeDelay);
    while (_port.available() > 0)
    {
      incomingByte = _port.read();
      if (incomingByte == H1 && (_port.peek() == H2))
      {
        statusPacket[0] = incomingByte;
        incomingByte = _port.read();
        if (incomingByte == H2 && (_port.peek() == H3))
        {
          statusPacket[1] = incomingByte;
          for (int q = 2; q < statusPacket_size; q++)
          {
            statusPacket[q] = _port.read();
          }
        }
      }
    }
}

void DynamixelServo::clearRXbuffer(void)
{
  //while (_port.read() != -1);  // Clear RX buffer;
  while(_port.available()){_port.read();}
}

void DynamixelServo::write_position(uint8_t Id, long gol_pos)
{
  uint8_t goalPositionVeri[4] = {0x00, 0x00, 0x00, 0x00};
  goalPositionVeri[0] = (byte)(gol_pos & 0xFF);
  goalPositionVeri[1] = (byte)((gol_pos >> 8) & 0xFF);
  goalPositionVeri[2] = (byte)((gol_pos >> 16) & 0xFF);
  goalPositionVeri[3] = (byte)((gol_pos >> 24) & 0xFF);
  write_raw(Id, Goal_Position, goalPositionVeri, Goal_Position_size);
}

void DynamixelServo::write_torque(uint8_t Id, int val)
{
  uint8_t torqueEnableVeri[1] = {val};
  write_raw(Id, Torque_Enable, torqueEnableVeri, Torque_Enable_size);
}

void DynamixelServo::write_acc_prof(uint8_t Id, long val) // (0 <= val <= 32767)_____unit = 0.3745 rad/s^2______(214.57 rev/min^2)
{//455 yapmak lazım iç eksende (1 Nm tork ile ivmelenmek için) (0.7 kasnak oranı ile)
  //345 dış eksende (1Nm tork ile ivmelenmek için)
  uint8_t accProfVeri[4] = {0x00, 0x00, 0x00, 0x00};
  accProfVeri[0] = (byte)(val & 0xFF);
  accProfVeri[1] = (byte)((val >> 8) & 0xFF);
  accProfVeri[2] = (byte)((val >> 16) & 0xFF);
  accProfVeri[3] = (byte)((val >> 24) & 0xFF);
  write_raw(Id, Profile_Acceleration, accProfVeri, Profile_Acceleration_size);
}

void DynamixelServo::write_vel_prof(uint8_t Id, long val) // (0 <= val <= vel limit(0-1023))_____unit = 0.024 rad/s______(0.229 rpm)
{// her iki motor için 2rad/s maksimum hız olacak, yani val = 120 olacak (0.7 kasnak oranı ile)
  uint8_t velProfVeri[4] = {0x00, 0x00, 0x00, 0x00};
  velProfVeri[0] = (byte)(val & 0xFF);
  velProfVeri[1] = (byte)((val >> 8) & 0xFF);
  velProfVeri[2] = (byte)((val >> 16) & 0xFF);
  velProfVeri[3] = (byte)((val >> 24) & 0xFF);
  write_raw(Id, Profile_Velocity, velProfVeri, Profile_Velocity_size);
}







//update_crc function from robotis documentation
uint16_t DynamixelServo::update_crc(uint16_t crc_accum, uint8_t *data_blk_ptr, unsigned short data_blk_size)
{
  uint16_t i, j;
  uint16_t crc_table[256] = {
    0x0000, 0x8005, 0x800F, 0x000A, 0x801B, 0x001E, 0x0014, 0x8011,
    0x8033, 0x0036, 0x003C, 0x8039, 0x0028, 0x802D, 0x8027, 0x0022,
    0x8063, 0x0066, 0x006C, 0x8069, 0x0078, 0x807D, 0x8077, 0x0072,
    0x0050, 0x8055, 0x805F, 0x005A, 0x804B, 0x004E, 0x0044, 0x8041,
    0x80C3, 0x00C6, 0x00CC, 0x80C9, 0x00D8, 0x80DD, 0x80D7, 0x00D2,
    0x00F0, 0x80F5, 0x80FF, 0x00FA, 0x80EB, 0x00EE, 0x00E4, 0x80E1,
    0x00A0, 0x80A5, 0x80AF, 0x00AA, 0x80BB, 0x00BE, 0x00B4, 0x80B1,
    0x8093, 0x0096, 0x009C, 0x8099, 0x0088, 0x808D, 0x8087, 0x0082,
    0x8183, 0x0186, 0x018C, 0x8189, 0x0198, 0x819D, 0x8197, 0x0192,
    0x01B0, 0x81B5, 0x81BF, 0x01BA, 0x81AB, 0x01AE, 0x01A4, 0x81A1,
    0x01E0, 0x81E5, 0x81EF, 0x01EA, 0x81FB, 0x01FE, 0x01F4, 0x81F1,
    0x81D3, 0x01D6, 0x01DC, 0x81D9, 0x01C8, 0x81CD, 0x81C7, 0x01C2,
    0x0140, 0x8145, 0x814F, 0x014A, 0x815B, 0x015E, 0x0154, 0x8151,
    0x8173, 0x0176, 0x017C, 0x8179, 0x0168, 0x816D, 0x8167, 0x0162,
    0x8123, 0x0126, 0x012C, 0x8129, 0x0138, 0x813D, 0x8137, 0x0132,
    0x0110, 0x8115, 0x811F, 0x011A, 0x810B, 0x010E, 0x0104, 0x8101,
    0x8303, 0x0306, 0x030C, 0x8309, 0x0318, 0x831D, 0x8317, 0x0312,
    0x0330, 0x8335, 0x833F, 0x033A, 0x832B, 0x032E, 0x0324, 0x8321,
    0x0360, 0x8365, 0x836F, 0x036A, 0x837B, 0x037E, 0x0374, 0x8371,
    0x8353, 0x0356, 0x035C, 0x8359, 0x0348, 0x834D, 0x8347, 0x0342,
    0x03C0, 0x83C5, 0x83CF, 0x03CA, 0x83DB, 0x03DE, 0x03D4, 0x83D1,
    0x83F3, 0x03F6, 0x03FC, 0x83F9, 0x03E8, 0x83ED, 0x83E7, 0x03E2,
    0x83A3, 0x03A6, 0x03AC, 0x83A9, 0x03B8, 0x83BD, 0x83B7, 0x03B2,
    0x0390, 0x8395, 0x839F, 0x039A, 0x838B, 0x038E, 0x0384, 0x8381,
    0x0280, 0x8285, 0x828F, 0x028A, 0x829B, 0x029E, 0x0294, 0x8291,
    0x82B3, 0x02B6, 0x02BC, 0x82B9, 0x02A8, 0x82AD, 0x82A7, 0x02A2,
    0x82E3, 0x02E6, 0x02EC, 0x82E9, 0x02F8, 0x82FD, 0x82F7, 0x02F2,
    0x02D0, 0x82D5, 0x82DF, 0x02DA, 0x82CB, 0x02CE, 0x02C4, 0x82C1,
    0x8243, 0x0246, 0x024C, 0x8249, 0x0258, 0x825D, 0x8257, 0x0252,
    0x0270, 0x8275, 0x827F, 0x027A, 0x826B, 0x026E, 0x0264, 0x8261,
    0x0220, 0x8225, 0x822F, 0x022A, 0x823B, 0x023E, 0x0234, 0x8231,
    0x8213, 0x0216, 0x021C, 0x8219, 0x0208, 0x820D, 0x8207, 0x0202
  };

  for (j = 0; j < data_blk_size; j++)
  {
    i = ((uint16_t)(crc_accum >> 8) ^ data_blk_ptr[j]) & 0xFF;
    crc_accum = (crc_accum << 8) ^ crc_table[i];
  }
  return crc_accum;
}
