using TempModbusProject.Service.IService;

namespace TempModbusProject.Service.Context
{
    public class CommunicationFactory : ICommunicationFactory
    {
        private readonly PortLineVm _portSuper;
        private readonly IServiceProvider _serviceProvider;

        public CommunicationFactory(PortLineVm portSuper, IServiceProvider serviceProvider)
        {
            _portSuper = portSuper;
            _serviceProvider = serviceProvider;
        }

        public ICommunication Create(float temp,ushort startaddress,string type)
        {
            if (type == null) {
                throw new ArgumentNullException(nameof(type), "Type cannot be null");
            }
           switch(type)
            {
                case "Modbus":
                    var comm = _serviceProvider.GetService<CommunicationImp>();
                    comm.initModbusParmeter(temp, startaddress);
                    comm._portSuper = _portSuper;
                    return comm;
                case "Esp8266":
                    return _serviceProvider.GetService<Esp8266Connect>();
                default:
                    throw new ArgumentException("Invalid type");
            } 
        }
    }
}
