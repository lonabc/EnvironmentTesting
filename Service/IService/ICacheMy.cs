namespace TempModbusProject.Service.IService
{
    public interface ICacheMy
    {
        public  abstract void setCacge(string key, object value);
        public abstract object getCacge(string key);

    }
}
