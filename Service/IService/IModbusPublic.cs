namespace TempModbusProject.Service.IService
{
    public interface IModbusPublic
    {
       public byte[] BuildModbusFrame(byte address, byte functionCode, ushort startAddress, ushort dataOrCount, byte[] extraData = null);
        public ushort CalculateCRC(byte[] data, int length);
        public void inintParamaters(ushort aimAddress);
        public bool IsValidModbusFrame(byte[] frame);

        public byte[] writeModbusFrame(ushort startAddress, ushort[] values);

        public byte[] readModbusFrame(ushort startAddress, ushort numberOfPoints);

        public void readDoubleFrame();
    }
}
