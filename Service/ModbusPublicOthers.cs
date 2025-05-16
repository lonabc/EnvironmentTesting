using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class ModbusPublicOthers : IModbusPublic
    {

        protected IModbusPublic modbusPublic;
        public void decorate(IModbusPublic modbusPublic)
        {
            this.modbusPublic = modbusPublic;
        }
        public byte[] BuildModbusFrame(byte address, byte functionCode, ushort startAddress, ushort dataOrCount, byte[] extraData = null)
        {
            throw new NotImplementedException();
        }

        public ushort CalculateCRC(byte[] data, int length)
        {
            throw new NotImplementedException();
        }

        public bool IsValidModbusFrame(byte[] frame)
        {
            throw new NotImplementedException();
        }

        public void inintParamaters(ushort aimAddress)
        {
            throw new NotImplementedException();
        }

        public byte[] writeModbusFrame(ushort startAddress, ushort[] values)
        {
            throw new NotImplementedException();
        }

        public void readModbusFrame()
        {
            throw new NotImplementedException();
        }

        public void readDoubleFrame()
        {
            throw new NotImplementedException();
        }

        public void readModbusFrame(ushort startAddress, ushort numberOfPoints)
        {
            throw new NotImplementedException();
        }

        byte[] IModbusPublic.readModbusFrame(ushort startAddress, ushort numberOfPoints)
        {
            throw new NotImplementedException();
        }
    }
}
