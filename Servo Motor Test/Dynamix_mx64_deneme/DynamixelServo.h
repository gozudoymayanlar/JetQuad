/*
 * DynamixelServo.h - Library to communicate with Dynamixel servo motors.
 * supported models MX64 & XL320
 * Created by NightHawk 08.03.2019
 */

#ifndef Dynamixel_falanFilan
#define Dynamixel_falanFilan

#include "Arduino.h"

#ifdef servo_mx64
// EEPROM datas: addresses and sizes
#define Model_Number				    0x00
#define Model_Number_size			  0x02

#define Model_Information			  0x02
#define Model_Information_size	0x04

#define Firmware_Version			  0x06
#define Firmware_Version_size		0x01

#define ID							        0x07
#define ID_size						      0x01

#define Baud_Rate					      0x08
#define Baud_Rate_size				  0x01

#define Return_Delay_Time			  0x09
#define Return_Delay_Time_size	0x01

#define Drive_Mode					    0x0A
#define Drive_Mode_size				  0x01

#define Operating_Mode				  0x0B
#define Operating_Mode_size			0x01

#define Secondary_ID				    0x0C
#define Secondary_ID_size			  0x01

#define Protocol_Version			  0x0D
#define Protocol_Version_size		0x01

#define Homing_Offset			      0x14
#define Homing_Offset_size			0x04

#define Moving_Treshold				  0x18
#define Moving_Treshold_size		0x04

#define Temperature_Limit			  0x1F
#define Temperature_Limit_size	0x01

#define Max_Voltage_Limit			  0x20
#define Max_Voltage_Limit_size  0x02

#define Min_Voltage_Limit			  0x22
#define Min_Voltage_Limit_size	0x02

#define PWM_Limit				      	0x24
#define PWM_Limit_size			  	0x02

#define Current_Limit				    0x26
#define Current_Limit_size			0x02

#define Acceleration_Limit		  	0x28
#define Acceleration_Limit_size		0x04

#define Velocity_Limit			  	0x2C
#define Velocity_Limit_size			0x04

#define Max_Position_Limit			0x30
#define Max_Position_Limit_size	0x04

#define Min_Position_Limit			0x34
#define Min_Position_Limit_size	0x04

#define Shutdown					      0x3F
#define Shutdown_size				    0x01

// RAM datas and addresses
#define Torque_Enable				    0x40
#define Torque_Enable_size      0x01

#define LED							        0x41
#define LED_size                0x01

#define Status_Return_Level			  0x44	
#define Status_Return_Level_size  0x01

#define Registered_Instruction		  0x45
#define Registered_Instruction_size 0x01

#define Hardware_Error_Status		    0x46
#define Hardware_Error_Status_size  0x01

#define Velocity_I_Gain				      0x4C
#define Velocity_I_Gain_size        0x02

#define Velocity_P_Gain				      0x4E
#define Velocity_P_Gain_size        0x02

#define Position_D_Gain				      0x50
#define Position_D_Gain_size        0x02

#define Position_I_Gain				      0x52
#define Position_I_Gain_size        0x02

#define Position_P_Gain				      0x54
#define Position_P_Gain_size        0x02

#define Feedforward_2nd_Gain	    	0x58	
#define Feedforward_2nd_Gain_size   0x02

#define Feedforward_1st_Gain	    	0x5A
#define Feedforward_1st_Gain_size   0x02

#define BUS_Watchdog				        0x62
#define BUS_Watchdog_size           0x01

#define Goal_PWM					          0x64
#define Goal_PWM_size               0x02

#define Goal_Current				        0x66
#define Goal_Current_size           0x02

#define Goal_Velocity				        0x68
#define Goal_Velocity_size          0x04

#define Profile_Acceleration	    	0x6C
#define Profile_Acceleration_size   0x04

#define Profile_Velocity			      0x70
#define Profile_Velocity_size       0x04

#define Goal_Position			        	0x74
#define Goal_Position_size          0x04

#define Realtime_Tick			        	0x78
#define Realtime_Tick_size          0x02

#define Moving						          0x7A
#define Moving_size                 0x01

#define Moving_Status			        	0x7B
#define Moving_Status_size          0x01

#define Present_PWM				        	0x7C
#define Present_PWM_size            0x02

#define Present_Current			      	0x7E
#define Present_Current_size        0x02

#define Present_Velocity			      0x80
#define Present_Velocity_size       0x04

#define Present_Position			      0x84
#define Present_Position_size       0x04

#define Velocity_Trajectory			    0x88
#define Velocity_Trajectory_size    0x04

#define Position_Trajectory			    0x8C
#define Position_Trajectory_size    0x04

#define Present_Input_Voltage	    	0x90
#define Present_Input_Voltage_size  0x02

#define Present_Temperature		    	0x92
#define Present_Temperature_size    0x01

#endif

#ifdef servo_xl320
 // EEPROM datas: addresses and sizes
#define Model_Number          0x00
#define Model_Number_size       0x02

#define Firmware_Version        0x02
#define Firmware_Version_size     0x01

#define ID                    0x03
#define ID_size                 0x01

#define Baud_Rate           0x04
#define Baud_Rate_size          0x01

#define Return_Delay_Time       0x05
#define Return_Delay_Time_size      0x01

#define CW_Angle_Limit      0x06
#define CW_Angle_Limit_size   

#define CCW_Angle_Limit     0x08
#define CCW_Angle_Limit_size    

#define Control_Mode      0x11
#define Control_Mode      0x01

#define Limit_Temperature   0x12
#define Limit_Temperature_size  0x01

#define Lower_Limit_Voltage 0x13
#define Lower_Limit_Voltage_size  0x02

#define Upper_Limit_Voltage       0x14
#define Upper_Limit_Voltage_size  0x02

#define Max_Torque            0x26
#define Max_Torque_size       0x02

#define Return_Level
#define Alarm_Shutdown



 // RAM datas and addresses
#define Torque_Enable           0x24
#define Torque_Enable_size      0x01

#define LED                     0x25
#define LED_size                0x01

#define D_Gain              0x27
#define D_Gain_size        0x01

#define I_Gain              0x28
#define I_Gain_size        0x01

#define P_Gain              0x29
#define P_Gain_size        0x01

#define Goal_Position               0x30
#define Goal_Position_size          0x02

#define Moving_Speed            0x7A
#define Moving_Speed_size                 0x01

#define Torque_Limit    0x35
#define Torque_Limit    0x02

#define Present_Position            0x84
#define Present_Position_size       0x04

#define Present_Speed           0x80
#define Present_Speed_size       0x04

#define Hardware_Error_Status       0x50
#define Hardware_Error_Status_size  0x01

#define Present_Load            0x80
#define Present_Load_size       0x04
    // BURADA KALDIM




#define BUS_Watchdog                0x62
#define BUS_Watchdog_size           0x01

#define Goal_PWM                    0x64
#define Goal_PWM_size               0x02

#define Goal_Current                0x66
#define Goal_Current_size           0x02

#define Goal_Velocity               0x68
#define Goal_Velocity_size          0x04

#define Profile_Acceleration        0x6C
#define Profile_Acceleration_size   0x04

#define Profile_Velocity            0x70
#define Profile_Velocity_size       0x04



#define Realtime_Tick               0x78
#define Realtime_Tick_size          0x02



#define Moving_Status               0x7B
#define Moving_Status_size          0x01

#define Present_PWM                 0x7C
#define Present_PWM_size            0x02

#define Present_Current             0x7E
#define Present_Current_size        0x02





#define Velocity_Trajectory         0x88
#define Velocity_Trajectory_size    0x04

#define Position_Trajectory         0x8C
#define Position_Trajectory_size    0x04

#define Present_Input_Voltage       0x90
#define Present_Input_Voltage_size  0x02

#define Present_Temperature         0x92
#define Present_Temperature_size    0x01
#endif

//data packet constants
#define	H1		0xFF
#define	H2		0xFF
#define	H3		0xFD
#define RSRV	0X00
#define ReadPacketLength	0x07 //Packet Length = number of Parameters(read'de 4 tane param var) + 3(inst, crc_l, crc_h)

// ID's
#define Motor1Roll    0x01
#define Motor1Pitch   0x02
#define Motor2Roll    0x03
#define Motor2Pitch   0x04
#define Motor3Roll    0x05
#define Motor3Pitch   0x06
#define Motor4Roll    0x07
#define Motor4Pitch   0x08
#define broadcastID   0xFE

//instructions
#define Ping_inst			    0x01
#define Read_inst			    0x02
#define Write_inst			  0x03
#define RegWrite_inst		  0x04
#define Action_inst			  0x05
#define FactoryReset_inst	0x06
#define Reboot_inst			  0x08
#define Clear_inst			  0x10
#define Status_inst			  0x55
#define SyncRead_inst		  0x82
#define SyncWrite_inst	  0x83
#define BulkRead_inst		  0x92
#define BulkWrite_inst	  0x93

class DynamixelServo
{
  public:
//    DynamixelServo(HardwareSerial &port, const unsigned long baud): _port (port), _baud (baud){}
    DynamixelServo(HardwareSerial &port, const uint32_t baud, const int dataControlPin );
    void beginComm();
    void end(void);
	  void read_raw(uint8_t Id, uint8_t address, uint8_t datas[], const int datas_size); // this only supports to reach adrresses 0 to 255
	  void write_raw(uint8_t Id, uint8_t address, uint8_t datas[], int datas_size);
    uint16_t update_crc(uint16_t crc_accum, uint8_t *data_blk_ptr, unsigned short data_blk_size);
  
  private:
    HardwareSerial &_port;
    const uint32_t _baud;
    int dataControlPin;
    void clearRXbuffer(void);
    uint16_t CRC;
    uint8_t CRC_L = 0;
    uint8_t CRC_H = 0;
};

#endif // !DynamixelServo
