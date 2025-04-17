using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class CommunicationImp : ICommunication
    {
        private float temp;
        private ushort startAddress;
        public  PortLineVm _portSuper; //这个是要注入的对象

        public CommunicationImp(float _temp,ushort _startAddress)
        {
            temp = _temp;
            startAddress=_startAddress;
        }

        public override Task communicationSend(ushort portId) //写入浮点数据
        {
        //    _portSuper.MessageInit(portId); //初始化端口
            byte[] bytes = BitConverter.GetBytes(temp); //获取到的字节数组，低位在前，高位在后,小端,windows默认
            Array.Reverse(bytes); // 反转字节数组,大端高位在前,
            ushort reg1 = (ushort)(bytes[0] << 8 | bytes[1]);
            ushort reg2 = (ushort)(bytes[2] << 8 | bytes[3]);
            _portSuper.WriteMultipleRegisters(portId, new ushort[] { reg1, reg2 });
            return Task.CompletedTask; //返回一个完成的任务
        }

        public override Task communicationReceive(ushort portId) //读取寄存器
        {
        
            if (_portSuper == null)
            {
                Console.WriteLine("警告: _portSuper 为空，可能未正确注入！");
                return Task.CompletedTask; // 提前中止
            }

            _portSuper.ReadHoldingRegisters(portId, 4); //读取两个寄存器
            return Task.CompletedTask; //返回一个完成的任务
        }

        public override bool communicationInit(string portId)
        {
           return _portSuper.MessageInit(portId); //初始化端口
        }

        public override void initAddress(byte address)
        {
            startAddress = address; //初始化地址
        }
    }
}
