namespace TempModbusProject.Configure.AttributeConfigure
{
    [AttributeUsage(AttributeTargets.Method)] //限制该特性只能应用于方法
    public class NotTransactionalAttribute : Attribute
    {
    }
}
