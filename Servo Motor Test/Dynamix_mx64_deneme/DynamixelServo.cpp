#include "Arduino.h"
#include "DynamixelServo.h"

DynamixelServo::DynamixelServo(HardwareSerial &port, const unsigned long baud): _port (port), _baud (baud)
{
  
}

void DynamixelServo::beginComm()
{
  _port.begin(_baud);
}

void DynamixelServo::read_raw(uint8_t Id, uint8_t address, uint8_t *buffer_arr)
{
  uint16_t CRC;
  uint8_t CRC_L = 0;
  uint8_t CRC_H = 0;

  uint8_t dataLength = sizeof(buffer_arr);
  //															                                 len2, inst     , add_l  ,add_h, dataLen_l , dataLen_h
  uint8_t TxPacket[14] = { H1, H2, H3, RSRV, Id, ReadPacketLength, 0x00, Read_inst, address, 0x00, dataLength,   0x00   ,  CRC_L, CRC_H };
  CRC = update_crc(0, TxPacket, 12);	// 12 = 5 + Packet Length(7)
  CRC_L = (CRC & 0x00FF);				//Little-endian
  CRC_H = (CRC >> 8) & 0x00FF;
  TxPacket[12] = CRC_L;
  TxPacket[13] = CRC_H;

  _port.write(TxPacket, 14);
  delay(2);

  // read the total number of bytes:
  _port.readBytes(buffer_arr, dataLength);
}

void DynamixelServo::write_raw(uint8_t Id, uint8_t address, uint8_t datas[])
{
  uint16_t CRC;
  uint8_t CRC_L = 0;
  uint8_t CRC_H = 0;

  uint8_t datas_size = sizeof(datas);		          //  , len_l			, len_h, inst      , add_h  , add_l
  uint8_t packetBase[10] = { H1, H2, H3, RSRV, Id, datas_size + 3 , 0x00, Write_inst, address,  0x00 };
  int packetSize = 12 + datas_size;
  uint8_t TxPacket[packetSize];
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

  _port.write(TxPacket, packetSize);
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

    for(j = 0; j < data_blk_size; j++)
    {
        i = ((uint16_t)(crc_accum >> 8) ^ data_blk_ptr[j]) & 0xFF;
        crc_accum = (crc_accum << 8) ^ crc_table[i];
    }
    return crc_accum;
}
