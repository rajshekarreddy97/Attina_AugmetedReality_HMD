#include <ESP8266WiFi.h>
#include <WiFiUdp.h>
#include <WiFiClient.h> 
#include <ESP8266WebServer.h>

#include "I2Cdev.h"
#include "MPU6050_6Axis_MotionApps20.h"
#include "Wire.h"

MPU6050 mpu;

// MPU control/status vars
bool dmpReady = false;  // set true if DMP init was successful
uint8_t devStatus;      // return status after each device operation (0 = success, !0 = error)
uint16_t packetSize;    // expected DMP packet size (default is 42 bytes)
uint16_t fifoCount;     // count of all bytes currently in FIFO
uint8_t fifoBuffer[64]; // FIFO storage buffer

// orientation/motion vars
Quaternion q;           // [w, x, y, z]         quaternion container
VectorInt16 aa;         // [x, y, z]            accel sensor measurements
VectorInt16 aaReal;     // [x, y, z]            gravity-free accel sensor measurements
VectorInt16 aaWorld;    // [x, y, z]            world-frame accel sensor measurements
VectorFloat gravity;    // [x, y, z]            gravity vector
float euler[3];         // [psi, theta, phi]    Euler angle container
float ypr[3];           // [yaw, pitch, roll]   yaw/pitch/roll container and gravity vector

// Button controls

//int ledpin = 0; // D1(gpio5)
int button = 14; //D2(gpio4)
int buttonState=0;

const char* ssid = "************";
const char* password = "************";

WiFiUDP Udp;
unsigned int localUdpPort = 1999;  // local port to listen on
IPAddress IP_Remote(192, 168, 0, 255);   // Broadcast IP Address

void setup()
{
  Wire.begin();
  Wire.setClock(400000); // 400kHz I2C clock. Comment this line if having compilation difficulties
  
  Serial.begin(115200);

  while (!Serial); // wait for Leonardo enumeration, others continue immediately

  mpu.initialize();
  devStatus = mpu.dmpInitialize();
  mpu.setXGyroOffset(54); //++
  mpu.setYGyroOffset(-21); //--
  mpu.setZGyroOffset(5);

  if (devStatus == 0) {
    mpu.setDMPEnabled(true);
    // set our DMP Ready flag so the main loop() function knows it's okay to use it
    dmpReady = true;
    // get expected DMP packet size for later comparison
    packetSize = mpu.dmpGetFIFOPacketSize();
  } else {
    // Error
    Serial.println("Error!");
  }

  Serial.printf("Connecting to %s ", ssid);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }
  Serial.println(" connected");

  Udp.begin(localUdpPort);
  Serial.printf("Now listening at IP %s, UDP port %d\n", WiFi.localIP().toString().c_str(), localUdpPort);

  pinMode(button, INPUT);
}

void loop()
{
  Udp.beginPacket(IP_Remote, 1999);
  String message;

  buttonState=digitalRead(button); // put your main code here, to run repeatedly:
  if (buttonState == 1)
  {
    Serial.println("Button pressed");
    message += "button/";
    delay(10);
  }

  if (!dmpReady) 
  {
      Serial.println("IMU not connected.");
      delay(10);
      return;
  }

  message += "r/";

  int  mpuIntStatus = mpu.getIntStatus();
  fifoCount = mpu.getFIFOCount();

  if ((mpuIntStatus & 0x10) || fifoCount == 1024) 
  { 
    // check if overflow
      mpu.resetFIFO();
  } 
  else if (mpuIntStatus & 0x02) 
  {
      while (fifoCount < packetSize) fifoCount = mpu.getFIFOCount();

      mpu.getFIFOBytes(fifoBuffer, packetSize);
      fifoCount -= packetSize;

      mpu.dmpGetQuaternion(&q, fifoBuffer);
      message += String(q.w, 4); message += "/";
      message += String(q.x, 4); message += "/";
      message += String(q.y, 4); message += "/";
      message += String(q.z, 4);
      
//      Udp.write("r/");
//      Udp.write(q.w, 4); Udp.write("/");
//      Udp.write(q.x, 4); Udp.write("/");
//      Udp.write(q.y, 4); Udp.write("/");
//      Udp.write(q.z, 4);

        byte buffer[message.length() + 1];
        message.getBytes(buffer, message.length() + 1);
        Udp.write(buffer, sizeof(buffer));
  }
       
  Udp.endPacket();
}
