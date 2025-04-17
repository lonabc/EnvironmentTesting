using TempModbusProject.Service.IService;

namespace TempModbusProject.Service.Context
{
    public class CommunicationFactory : ICommunicationFactory
    {
        private readonly PortLineVm _portSuper;

        public CommunicationFactory(PortLineVm portSuper)
        {
            _portSuper = portSuper;
        }

        public ICommunication Create(float temp,ushort startaddress)
        {          
           
            var comm = new CommunicationImp(temp,startaddress);
            comm._portSuper = _portSuper;
            return comm;
        }
    }
}
