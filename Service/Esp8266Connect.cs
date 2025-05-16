using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Server;
using System.Net;
using System.Text;
using TempModbusProject.Model;
using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class Esp8266Connect : ICommunication
    {
        private IMqttClient _mqttClient;

        private IModbusPublic _modbusPublic;

        private readonly IModbusFactory _modbusFactory;

        private String ipAddress= "47.121.112.154";

        public static ushort startAddressModbus = 0x0000; //起始地址

        public Esp8266Connect(IModbusFactory modbusFactory)
        {
            _modbusFactory = modbusFactory;
            _modbusPublic = _modbusFactory.create("Basic");
         
        }


        public async override Task<bool> communicationInit(string portId)
        {
            // 创建客户端工厂
            var factory = new MqttFactory();

            

            // 创建 MQTT 客户端
            _mqttClient = factory.CreateMqttClient();


            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
            // 配置客户端选项
            var options = new MqttClientOptionsBuilder()
            .WithTcpServer(ipAddress,1883)       // MQTT 服务器地址和端口
                .WithCredentials("user", "public")        // 用户名和密码
                .WithClientId("Client1")                // 客户端ID
                .WithCleanSession()                    // 清除会话
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(120)) // 保持连接间隔
                .Build();

            try
            {
                await _mqttClient.ConnectAsync(options);
                Console.WriteLine("Connected to MQTT server");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection failed: {ex.Message}");
                return false;
            }
        }
        private Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            try
            {
                var topic = e.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment.Array,
                                                      e.ApplicationMessage.PayloadSegment.Offset,
                                                      e.ApplicationMessage.PayloadSegment.Count);
              
                var qos = e.ApplicationMessage.QualityOfServiceLevel;
                var retain = e.ApplicationMessage.Retain;
                //   _modbusPublic.CalculateCRC
                byte[] bytes=  HexStringToByteArray(payload);
             //   Console.WriteLine($"收到消息 - 主题: {topic},有效载荷：{payload}, QoS: {qos}, Retain: {retain}");


                if (_modbusPublic.IsValidModbusFrame(bytes))
                {
                    switch (bytes[1])
                    {
                        case 0x03:
                            ushort byteCount = (ushort)(bytes[2] / 2); //字节数
                            ushort[] values=new ushort[byteCount];
                            for (int i = 0; i < byteCount; i++)
                            {
                                values[i] = (ushort)(bytes[3 + i * 2] << 8 | bytes[4 + i * 2]);
                            }
                            if (byteCount >= 2)
                            {
                                byte[] floatBytes = new byte[4];
                                floatBytes[0] = (byte)(values[1]& 0xFF);
                                floatBytes[1] = (byte)(values[1] >> 8);
                                floatBytes[2] = (byte)(values[0] & 0xFF);
                                floatBytes[3] = (byte)(values[0] >> 8);

                                float result = BitConverter.ToSingle(floatBytes, 0); //转换位浮点数
                    //            Console.WriteLine($"接收到的浮点数: {result}");
                                switch (startAddressModbus)
                                {
                                    case 0x0000:
                                        SenSorData._senSorData[0] = result;
                                        Console.WriteLine($"接收到的温度: {result}");
                                        break;
                                    case 0x0002:
                                        SenSorData._senSorData[1] = result;
                                        Console.WriteLine($"接收到的空气污染度: {result}");
                                        break;
                                    case 0x0004:
                                        SenSorData._senSorData[2] = result;
                                        Console.WriteLine($"接收到的光照强度: {result}");
                                        Console.WriteLine("/r/n");
                                        break;
                                    default:
                                        break;
                                }
                            }
                            break;


                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex + "处理MQTT消息时出错");
            }
            return Task.CompletedTask;
        }

        //参数一内容，参数二寄存器数量，参数三订阅的主题，参数四起始地址
        public override async Task communicationSend(string address, ushort portId,string topicName,ushort startAddress) 
        {
           
            startAddressModbus = startAddress; //更新起始地址
            portId = (ushort)(portId * 2);
            byte[] frame = _modbusPublic.readModbusFrame(startAddress, portId); //读取寄存器指令 
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("Client not connneceted");
            }
            var sent = new MqttApplicationMessageBuilder().
            WithTopic(topicName).
            WithPayload(frame).
            WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce).Build();
            await _mqttClient.PublishAsync(sent);
            Console.WriteLine($"Published message to topic {topicName}");
            Console.WriteLine(BitConverter.ToString(frame));
         

        }


        public override async Task DisconnectAsync()
        {
            if (_mqttClient != null && _mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
        }
        public override async Task subTopicEsp8266(string topicName)
        {
            if (_mqttClient == null || !_mqttClient.IsConnected)
            {
                throw new InvalidOperationException("Client not connneceted");

            }
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topicName).Build());
            Console.WriteLine($"{topicName}:已被订阅");

        }
        public byte[] HexStringToByteArray(string hex)
        {
            if (string.IsNullOrWhiteSpace(hex))
            {
                throw new ArgumentException("十六进制字符串不能为空或空白", nameof(hex));
            }

            hex = hex.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");

            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException($"十六进制字符串长度必须是偶数。当前长度: {hex.Length}, 内容: '{hex}'");
            }

            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length; i += 2)
            {
                string byteStr = hex.Substring(i, 2);
                try
                {
                    bytes[i / 2] = Convert.ToByte(byteStr, 16);
                }
                catch (FormatException ex)
                {
                    throw new FormatException($"无法解析 '{byteStr}' 为16进制数字，索引位置: {i}, 原始字符串: '{hex}'", ex);
                }
            }

            return bytes;
        }



        public override void initAddress(byte address)
        {
            throw new NotImplementedException();
        }

        public override Task communicationReceive(ushort portId)
        {
            return Task.CompletedTask;
        }

       
    }
}
