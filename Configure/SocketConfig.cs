using System.Net;
using System.Net.Sockets;
using System.Text;
using TempModbusProject.Model;

namespace TempModbusProject.Configure
{
    public class SocketConfig
    {
        private const int Port = 5000;
        private TcpListener _listener; //确让tcp监听器
        public async Task StartAsync()
        {
            /**
             * 这里的TcpListener函数相当于传统的socket()和bind()函数,
             * 在申请网络资源后,对ip和端口进行绑定
             **/
            _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), Port);//相当与socket()和bind()
            _listener.Start(); //开始监听
            Console.WriteLine($"开始监听端口:{Port}");
            while(true)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync(); //等待客户端连接
                Console.WriteLine("客户端连接成功");
                _ = HandleRead(client); //处理客户端连接
                _= HandleSend(client, "Hello from server!"); //处理客户端发送
            }
        }

        public async Task<String> HandleRead(TcpClient client)
        {
            String data = "函数分离测试";
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[1024]; //
                while (true)
                {
                    int byteRead = await stream.ReadAsync(buffer, 0, buffer.Length); //阻塞,直到客户端发送数据,如果客户端断开连接,返回0
                    if (byteRead == 0) break; //客户端断开连接
                    data = Encoding.UTF8.GetString(buffer, 0, byteRead);
                    Console.WriteLine("Received:" + data);
                }
            }
            Console.WriteLine("Client disconnected");
            return data;

        }
        private async Task HandleSend(TcpClient client, String data)
        {

            using (NetworkStream stream = client.GetStream())
            {
                while (true)
                {
                   // string response = "Hello from server!";

                    float[] sensordate=SenSorData._senSorData; //获取传感器数据
                 
                     byte[] byteArray = new byte[sensordate.Length * sizeof(float)]; // 计算字节流长度
        

                    Buffer.BlockCopy(sensordate, 0, byteArray, 0, byteArray.Length); //将数组转换为字节流
                    byte[] newByteArray = BuildFrame(byteArray); //添加帧头和帧尾

                    await stream.WriteAsync(byteArray, 0, byteArray.Length); 
                     // await stream.WriteAsync(byteNewTemp, 0, byteNewTemp.Length);
                   // Console.WriteLine("后端socket命令成功发送 " + response);
                    await Task.Delay(10000);//每10秒发送一次数据
                }
            }
        }

        private byte[] BuildFrame(byte[] payload) //构建数据帧
        {
            using (MemoryStream ms=new MemoryStream())
            {
                //帧头数据
                ms.Write(new byte[] { 0x7e, 0x7e, 0x7e, 0x7e });
                ms.Write(new byte[] { 0x01,0x00 });

                //计算头长度
                byte[] lengthHeader=BitConverter.GetBytes((ushort)payload.Length);
                ms.Write(lengthHeader, 0, lengthHeader.Length);//写入数据长度
                ms.Write(payload, 0, payload.Length); //写入数据
                ms.Write(new byte[] { 0x0D, 0x0A }); //写入帧尾
                return ms.ToArray(); //返回字节数组
            }
        
        }
    }
}
