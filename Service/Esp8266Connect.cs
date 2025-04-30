using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Text;
using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class Esp8266Connect : IEsp8266Conncet
    {
        private IMqttClient _mqttClient;
        public async Task connectEsp8266Async(string address, int port)
        {
            // 创建客户端工厂
            var factory = new MqttFactory();

            // 创建 MQTT 客户端
            _mqttClient = factory.CreateMqttClient();


             _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
            // 配置客户端选项
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(address, port)       // MQTT 服务器地址和端口
                .WithCredentials("user", "public")        // 用户名和密码
                .WithClientId("Client1")                // 客户端ID
                .WithCleanSession()                    // 清除会话
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(120)) // 保持连接间隔
                .Build();

            try
            {
                await _mqttClient.ConnectAsync(options);
                Console.WriteLine("Connected to MQTT server");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
            }

        }

        private  Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var topic = e.ApplicationMessage.Topic;
             //   var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var qos = e.ApplicationMessage.QualityOfServiceLevel;
                var retain = e.ApplicationMessage.Retain;

                Console.WriteLine($"收到消息 - 主题: {topic}, QoS: {qos}, Retain: {retain}");
              //  Console.WriteLine($"消息内容: {payload}");

                // 在这里处理接收到的消息
              //  Console.WriteLine(topic, payload);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex+"处理MQTT消息时出错");
            }
            return Task.CompletedTask;
        }


        public async Task subTopicEsp8266(string topicName)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("Client not connneceted");

            }
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topicName).Build());
            Console.WriteLine($"{topicName}:已被订阅");

        }

        public async Task pubTopicEsp8266(string topicName, string message)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("Client not connneceted");
            }
            var sent = new MqttApplicationMessageBuilder().
                WithTopic(topicName).
                WithPayload(message).
                WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce).Build();
            await _mqttClient.PublishAsync(sent);
            Console.WriteLine($"Published message to topic {topicName}");

           
        }
        public async Task DisconnectAsync()
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
        }

    }
}
