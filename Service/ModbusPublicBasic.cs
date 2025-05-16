using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class ModbusPublicBasic : IModbusPublic
    {

        private ushort aimAddress;
        public void inintParamaters(ushort aimAddress)
        {
            this.aimAddress = aimAddress;
        }

        public byte[] BuildModbusFrame(byte address, byte functionCode, ushort startAddress, ushort dataOrCount, byte[] extraData = null)
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
         //   Console.WriteLine("CRC:" + crc.ToString("X4"));
         //   Console.WriteLine("Modbus Frame:" + BitConverter.ToString(frame.ToArray()).Replace("-", " ")); // 打印 Modbus 帧
            return frame.ToArray(); // 返回完整的 Modbus 帧
        }

        // 校验 CRC 是否正确
        public bool IsValidModbusFrame(byte[] frame)
        {
            if (frame.Length < 5) return false; // 至少要有地址+功能码+CRC

            ushort crcCalc = CalculateCRC(frame, frame.Length - 2);
            ushort crcRecv = (ushort)(frame[frame.Length - 2] | (frame[frame.Length - 1] << 8));

            return crcCalc == crcRecv;
        }

        public ushort CalculateCRC(byte[] data, int length) //CRC校验
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

        public byte[] writeModbusFrame(ushort startAddress, ushort[] values) 
        {
            byte[] data = new byte[values.Length * 2];
            for (int i = 0; i < values.Length; i++)
            {
                data[i * 2] = (byte)(values[i] >> 8);
                data[i * 2 + 1] = (byte)(values[i] & 0xFF);
            }
            byte[] frame = BuildModbusFrame(0x01, 0x10, startAddress, (ushort)values.Length, data); //构建写入寄存器指令
            return frame;
        }

        public byte[] readModbusFrame(ushort startAddress, ushort numberOfPoints)
        {
            byte[] frame = BuildModbusFrame(0x01, 0x03, startAddress, numberOfPoints); //构建读取寄存器指令
            return frame;
        }




        public void readDoubleFrame()
        {
            throw new NotImplementedException();
        }
    }
}
