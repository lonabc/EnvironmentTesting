using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using TempModbusProject.Configure;
using TempModbusProject.Service.Context;
using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class TimeTaskImp : IHostedService, IDisposable
    {
        private Timer _timer; //定时对象

        private readonly IServiceScopeFactory _serviceScopeFactory; 
        private int lock_Flag = 0;
        private static int count = 0; // 计数器
        public static bool _initialized = false;// 是否已初始化
        public static string portId = "5"; // 端口ID

        private static ICommunication com;  // 通信对象，工厂模式创建


        public TimeTaskImp(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            //// 启动时立即执行一次，之后每隔10秒执行一次
            _timer = new Timer(
                callback: _ => _ = ExecuteTaskEespAsync(),
                state: null,
                dueTime: TimeSpan.Zero,
                period: TimeSpan.FromSeconds(6)
            );
            return Task.CompletedTask;
        }

        private async Task ExecuteTaskEespAsync() // esp8266 串口通信
        {
            // 如果任务正在执行，则跳过本次触发（避免重叠执行）
            if (!get_Enlock()) //获取锁失败
            {
                Console.WriteLine("前一个任务仍在执行，跳过本次触发");
                return;
            }

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var myCom = scope.ServiceProvider.GetRequiredService<ICommunicationFactory>();

                    /**
                     *  这里使用了懒加载的方式进行初始化，确保只在第一次调用时创建通信对象。
                     *  如果通信端口偶然断开连接，_initialized会在PortLineMV类里的 SendModbusFrame(byte[] frame)中被设置为false，
                     *  之后可以重新创建通信对象。
                     **/
                    if (!_initialized) // 如果没有初始化,重新创建通信对象
                    {
                        com = myCom.Create(25.0f, 0x0000, "Esp8266"); //重新创建通信对象
                        if (await com.communicationInit(1883.ToString()))//初始化端口
                        {
                            await com.subTopicEsp8266("test");
                            _initialized = true; // 设置为已初始化
                        }
                        else
                        {
                            for (int i = 2; i < 10; i++)
                            {
                              
                                if (await com.communicationInit(1883.ToString()))
                                {
                                    await com.subTopicEsp8266("test");
                                    _initialized = true; // 设置为已初始化
                                    Thread.Sleep(500); // 等待0.5秒
                                    Console.WriteLine($"端口初始化成功，端口号：{1883}");
                                    break; // 端口初始化成功，跳出循环
                                }
                                _initialized = false; // 设置为未初始化

                            }
                        }
                    }

                    // Console.WriteLine($"轮询读取温度开始： {DateTime.Now}");
                    NLogConfigure.WirteLogTest($"轮询读取温度开始： {DateTime.Now}");
                    await com.communicationSend("test",1,"temp",0x0000);
                    await Task.Delay(700);
                    //   Console.WriteLine($"轮询读取光照强度结束： {DateTime.Now}");
                    NLogConfigure.WirteLogTest($"轮询读取光照强度结束： {DateTime.Now}");
                    await com.communicationSend("test", 1, "temp", 0x0002);
                    await Task.Delay(700);

                    //  Console.WriteLine($"轮询读取空气质量结束： {DateTime.Now}");
                    NLogConfigure.WirteLogTest($"轮询读取空气质量结束： {DateTime.Now}");
                    await com.communicationSend("test", 1, "temp", 0x0004);
                    Console.WriteLine($"轮询获取灯光状态和报警次数 {DateTime.Now}");
                    await com.communicationSend("test", 1, "temp", 0x0006); // 获取灯光状态和报警次数
                    await Task.Delay(700);
    

                    count++;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("任务被取消");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"任务执行失败: {ex.Message}");
            }
            finally
            {
                if (count >= 20)
                {
                    count = 0;
                    Console.Clear();//清除控制台
                }
                exit_Enlock(); // 释放锁
            }
        }


        private async Task ExecuteTaskAsync() // Modbus 串口通信暂时废弃
        {
            // 如果任务正在执行，则跳过本次触发（避免重叠执行）
            if (!get_Enlock()) //获取锁失败
            {
                Console.WriteLine("前一个任务仍在执行，跳过本次触发");
                return;
            }

            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var myCom = scope.ServiceProvider.GetRequiredService<ICommunicationFactory>();

                    /**
                     *  这里使用了懒加载的方式进行初始化，确保只在第一次调用时创建通信对象。
                     *  如果通信端口偶然断开连接，_initialized会在PortLineMV类里的 SendModbusFrame(byte[] frame)中被设置为false，
                     *  之后可以重新创建通信对象。
                     **/
                    if (!_initialized) // 如果没有初始化,重新创建通信对象
                    {
                        com = myCom.Create(25.0f, 0x0000,"Modbus"); //重新创建通信对象
                        if (await com.communicationInit(portId))//初始化端口
                        {
                            _initialized = true; // 设置为已初始化
                        }
                        else
                        {
                            for (int i = 2; i < 10; i++)
                            { 
                                portId = (i + 1).ToString(); // 端口ID从1到10
                                if (await com.communicationInit(portId))
                                { 
                                    _initialized=true; // 设置为已初始化
                                    Thread.Sleep(500); // 等待0.5秒
                                    Console.WriteLine($"端口初始化成功，端口号：{portId}");
                                    break; // 端口初始化成功，跳出循环
                                }
                                _initialized=false; // 设置为未初始化

                            }
                        }
                    }
                        
                   // Console.WriteLine($"轮询读取温度开始： {DateTime.Now}");
                    NLogConfigure.WirteLogTest($"轮询读取温度开始： {DateTime.Now}");
                    await com.communicationReceive(0x0000);
                    await Task.Delay(500);
                    //   Console.WriteLine($"轮询读取光照强度结束： {DateTime.Now}");
                    NLogConfigure.WirteLogTest($"轮询读取光照强度结束： {DateTime.Now}");
                    await com.communicationReceive(0x0002);
                    await Task.Delay(500);

                  //  Console.WriteLine($"轮询读取空气质量结束： {DateTime.Now}");
                    NLogConfigure.WirteLogTest($"轮询读取空气质量结束： {DateTime.Now}");
                    await com.communicationReceive(0x0004);
                    count++;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("任务被取消");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"任务执行失败: {ex.Message}");
            }
            finally
            {
                if (count>=20)
                {
                    count=0;
                    Console.Clear();//清除控制台
                }
                exit_Enlock(); // 释放锁
            }     
        }
        // 停止时的清理工作
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);  // 停止定时器
            return Task.CompletedTask;
        }

        // 释放资源
        public void Dispose()
        {
            _timer?.Dispose();
        }

        bool get_Enlock()
        {
           
            return Interlocked.CompareExchange(ref lock_Flag,1,0)==0; //获取锁成功
        }
        void exit_Enlock()
        {
            Interlocked.Exchange(ref lock_Flag, 0); //释放锁
        }
    }
}