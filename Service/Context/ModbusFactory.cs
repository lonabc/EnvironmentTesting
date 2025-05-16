using TempModbusProject.Service.IService;

namespace TempModbusProject.Service.Context
{
    public class ModbusFactory : IModbusFactory
    {
        private readonly IServiceProvider _serviceProvider;
        public ModbusFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public  IModbusPublic create(string type)
        {
            switch (type)
            {
                case "Basic":
                   return _serviceProvider.GetService<ModbusPublicBasic>();

                case "Pro":
                   return _serviceProvider.GetService<ModbusPublicOthers>();
                default:
                    throw new ArgumentException("Invalid type");
            }
        }
    }
}
