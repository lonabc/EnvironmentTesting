using NModbus;
using NModbus.IO; // 包含 StreamResource
using System.IO.Ports;
using System.Threading.Tasks;
using TempModbusProject.Configure;
using TempModbusProject.Model;
using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class PortLineVm : PortSuper
    {
        private byte _address = 0x01; //从机地址
        private SerialPort _serialPort;

        private bool checkTimeOut = false; //超时标志位

        //private static float[] _sensor_data = new float[4]; //传感器数据
        private static int _sensor_count = 0; //传感器数据计数器

        public static ushort startAddressCopy;
       

      

        public override bool MessageInit(String portId)
        {
            _serialPort = new SerialPort($"COM{portId}", 115200, Parity.None, 8, StopBits.One);
            _serialPort.WriteTimeout = 1000; // 设置写入超时

            try
            {
                _serialPort.Open();
                _serialPort.DataReceived+= OnDataReceived; // 订阅数据接收事件
                return true; // 端口初始化成功
            } catch (Exception e)
            {

                TimeTaskImp._initialized = false; //设置为未初始化
                NLogConfigure.WirteLogError("端口初始化失败\"+e.ToString()");
            //    Console.WriteLine("端口初始化失败"+e.ToString());
                return false; // 端口初始化失败

            }
        }

        public void initAddress(byte address)
        {
            _address = address;
        }
        public async Task ReadHoldingRegisters(ushort startAddress, ushort numberOfPoints) //读取寄存器
        {
            checkTimeOut = false;
            startAddressCopy = startAddress;
            byte[] frame = BuildModbusFrame(_address, 0x03, startAddress, numberOfPoints);
            
            SendModbusFrame(frame);
            await Task.Run(async() =>
            {
                try
                {
                    await Task.Delay(3000); // 等待2秒钟
                    if (!checkTimeOut)
                    {
                        Console.WriteLine("读取寄存器超时");
                        _serialPort.Close();
                    }
                  
                }
                catch (Exception e)
                {
                    Console.WriteLine("寄存器读取异常，已关闭端口");
                }
            });
    
        }
        // ✅ 2. 写入单个寄存器（功能码 0x06）
        public void WriteSingleRegister(ushort registerAddress, ushort value)
        {
            byte[] frame = BuildModbusFrame(_address, 0x06, registerAddress, value);
            SendModbusFrame(frame);
        }

        //写多个寄存器
        public async Task WriteMultipleRegisters(ushort startAddress, ushort[] values) 
        {
            try
            {
                byte[] data = new byte[values.Length * 2];
                for (int i = 0; i < values.Length; i++)
                {
                    data[i * 2] = (byte)(values[i] >> 8);      // 高字节
                    data[i * 2 + 1] = (byte)(values[i] & 0xFF);  // 低字节
                }

                byte[] frame = BuildModbusFrame(_address, 0x10, startAddress, (ushort)values.Length, data);
                SendModbusFrame(frame);
                await Task.Run(async() =>
                {
                    checkTimeOut=false; //重置超时标志位
                    try
                    {
                        await Task.Delay(3000); // 等待3秒钟

                        if (!checkTimeOut)
                        {
                            Console.WriteLine("写入寄存器超时");
                            _serialPort.Close();

                        }
                       
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("寄存器写入异常");
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"写入寄存器失败: {e.Message}");
            }
        }

        private byte[] BuildModbusFrame(byte address, byte functionCode, ushort startAddress, ushort dataOrCount, byte[] extraData = null) //构建啊数据帧
        {
            List<byte> frame = new List<byte>();
            frame.Add(address); // 从机地址
            frame.Add(functionCode); // 功能码
            frame.Add((byte)(startAddress >> 8)); // 起始地址高字节
            frame.Add((byte)(startAddress & 0xFF)); // 起始地址低字节
            if (functionCode == 0x10)
            {
                frame.Add((byte)(dataOrCount >> 8)); // 寄存器器数量高字节
                frame.Add((byte)(dataOrCount & 0xFF)); // 寄存器数量低字节
                frame.Add((byte)(extraData.Length)); // 字节数
                frame.AddRange(extraData); // 写入数据
            }
            else
            {
                frame.Add((byte)(dataOrCount >> 8)); // 寄存器值 /寄存器数量 高字节
                frame.Add((byte)(dataOrCount & 0xFF)); // 寄存器值 /寄存器数量 低字节

            }
            ushort crc = CalculateCRC(frame.ToArray(), frame.Count);
            frame.Add((byte)(crc & 0xFF)); // CRC 低字节
            frame.Add((byte)((crc >> 8) & 0xFF)); // CRC 高字节
            
            return  frame.ToArray(); // 返回完整的 Modbus 帧
        }

        // ✅ 发送数据帧
        public void SendModbusFrame(byte[] frame)
        {
            try
            {
                if (_serialPort==null)
                { 
                    Console.WriteLine("端口未初始化");
                    TimeTaskImp._initialized = false;
                }
                _serialPort.DiscardInBuffer(); // 清空输入缓冲区

                _serialPort.Write(frame, 0, frame.Length);
                //  Console.WriteLine($"发送数据帧: {BitConverter.ToString(frame)}");
                NLogConfigure.WirteLogTest($"发送数据帧: {BitConverter.ToString(frame)}");
        
            }
            catch (Exception e)
            {
                TimeTaskImp._initialized = false;
                Console.WriteLine($"发送失败: {e.Message}");
            }
        }
        // ✅ 数据接收处理（保持不变）
        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(50);
                int bytesToRead = _serialPort.BytesToRead;
                byte[] buffer = new byte[bytesToRead];
                _serialPort.Read(buffer, 0, bytesToRead);

                // Console.WriteLine($"收到数据: {BitConverter.ToString(buffer)}");
                NLogConfigure.WirteLogTest($"收到数据: {BitConverter.ToString(buffer)}");

                if (IsValidModbusFrame(buffer))
                {
                   // Console.WriteLine("有效 Modbus 帧");
                    checkTimeOut = true; // 设置超时标志位为 true
                    // 解析功能码、数据等...
                    switch (buffer[1])
                    {
                        case 0x03 :

                          // 读取保持寄存器的响应
                           ushort byteCount =(ushort)( buffer[2]/2);

                            ushort[] values = new ushort[byteCount];
                            for (int i = 0; i < byteCount; i++)
                            {
                                values[i] = (ushort)(buffer[3 + i * 2] << 8 | buffer[4 + i * 2]);
                            }
                            if (byteCount >= 2)
                            {
                                byte[] floatBytes = new byte[4]; //原本valaue大端序，高位在前,现在需要转换成小端序
                                floatBytes[0] = (byte)(values[1] & 0xFF); //按位与00001111，提取低位16的低8位
                                floatBytes[1] = (byte)(values[1] >> 8);   //低位右移8位，提取高位16的低8位
                                floatBytes[2] = (byte)(values[0] & 0xFF);
                                floatBytes[3] = (byte)(values[0] >> 8);

                                float result = BitConverter.ToSingle(floatBytes, 0); //将字节数组转换为浮点数，参数二是起始位置
                             //   Console.WriteLine($"读取保持寄存器(浮点数): {result}");
                                NLogConfigure.WirteLogTest($"读取保持寄存器(浮点数): {result}{Environment.NewLine}");
                            
                                //if (_sensor_count == 3)
                                //{
                                //    _sensor_count = 0; //重置计数器
                                //    SenSorData._senSorData[0] = result; //存储传感器数据
                                //}
                                //else
                                //{
                                //    SenSorData._senSorData[_sensor_count] = result; //存储传感器数据
                                //    _sensor_count++;
                                //}
                                switch (startAddressCopy)
                                {
                                    case 0x0000:
                                        SenSorData._senSorData[0] = result; //存储传感器数据                                   
                                        break;
                                    case 0x0002:
                                        SenSorData._senSorData[1] = result; //存储传感器数据                                      
                                        break;
                                    case 0x0004:
                                        SenSorData._senSorData[2] = result; //存储传感器数据                                      
                                        break;
                                    default:
                                        break;
                                }
                                //  Console.WriteLine("\n");
                            }
                          
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("无效 Modbus 帧");
                    _sensor_count--;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"接收错误: {ex.Message}");
               
            }

        }

        


        // 校验 CRC 是否正确
        public bool IsValidModbusFrame(byte[] frame)
        {
            if (frame.Length < 5) return false; // 至少要有地址+功能码+CRC

            ushort crcCalc = CalculateCRC(frame, frame.Length - 2);
            ushort crcRecv = (ushort)(frame[frame.Length - 2] | (frame[frame.Length - 1] << 8));

            return crcCalc == crcRecv;
        }

        public ushort CalculateCRC(byte[] data,int length)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < length; i++)
            {
                crc ^= data[i];

                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x0001) != 0)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc >>= 1;
                }
            }

            return crc;
        }
    }
}
