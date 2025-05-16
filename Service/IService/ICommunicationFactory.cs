namespace TempModbusProject.Service.IService
{
    public interface ICommunicationFactory
    {
        ICommunication Create(float temp,ushort startAddress,string type);
    }
}
