namespace TempModbusProject.Service.IService
{
    public interface IEsp8266Conncet
    {
        public Task connectEsp8266Async(String address,int port);

        public Task pubTopicEsp8266(String topicName,String message);

        public Task subTopicEsp8266(String topicName);

        public Task DisconnectAsync();
       
    }
}
