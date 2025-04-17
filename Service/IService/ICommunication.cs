namespace TempModbusProject.Service.IService
{
    public abstract class ICommunication
    {
        public abstract Task communicationSend(ushort portId);

        public abstract Task communicationReceive(ushort portId);
        public abstract bool communicationInit(string portId);

        public abstract void initAddress(byte address); //初始化地址

    }
}
