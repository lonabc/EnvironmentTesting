namespace TempModbusProject.Service.IService
{
    public interface IModbusFactory
    {
        public IModbusPublic create(string type);
    }
}
