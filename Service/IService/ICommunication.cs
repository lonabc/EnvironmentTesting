namespace TempModbusProject.Service.IService
{
    public abstract class ICommunication
    {
        public abstract Task communicationSend(String address,ushort portId,string topicName, ushort startAddress); //发送数据

        public abstract Task communicationReceive(ushort portId); //  接收数据
        public abstract Task<bool> communicationInit(string portId); //通讯初始化

        public abstract void initAddress(byte address); //初始化地址

        public abstract Task DisconnectAsync();
        public abstract Task subTopicEsp8266(string topicName);
        public abstract Task communicationSendSignal(string address, ushort portId, string topicName, ushort startAddress); //读取单个寄存器



    }
}
