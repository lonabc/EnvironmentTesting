using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Security.Claims;
using System.Threading.Tasks;
using TempModbusProject.Configure;
using TempModbusProject.Configure.AttributeConfigure;
using TempModbusProject.Model;
using TempModbusProject.Service;
using TempModbusProject.Service.IService;

namespace TempModbusProject.Controllers
{
    [ApiController]
    public class DataAccessController : Controller
    {
        readConfig _readConfig;
        SqlToolsServices _sqlToolsServices;
        Esp8266Connect _esp8266Connect ; //ESP8266连接类实例
        ICacheMy _cacheMy; //缓存接口实例
        public DataAccessController(readConfig readConfig, SqlToolsServices sqlToolsServices,Esp8266Connect esp8266Connect,ICacheMy cacheMyImp)
        {
            _readConfig = readConfig;
            _sqlToolsServices = sqlToolsServices;
            _cacheMy = cacheMyImp; //注入缓存接口
            _esp8266Connect = esp8266Connect;
        }
        [NotTransactional]//标记该方法不需要事务
        [HttpGet("GetDataAccess", Name = "GetDataAccess")]
        public void GetDataAccess()
        {
            SocketConfig socketConfig = new SocketConfig();
            socketConfig.StartAsync(); // 启动socket监听
            // 这里是获取数据访问的逻辑
            Console.WriteLine("获取数据访问");
            for (int i = 0; i < SenSorData._senSorData.Length; i++)
            {
                Console.WriteLine(SenSorData._senSorData[i]);
            }
        }


        [HttpPost("LoginUser", Name = "LoginUser")]
        public string LoginUser([FromBody] UserModels userModels) //用户登录
        {
            Console.WriteLine("登入功能触发");
            if (userModels == null) return "参数为空";
            UserModels user = _sqlToolsServices.selectSql(userModels.name, userModels.password); //查询用户信息
            if (user != null)
            {
                JwtSettings jwtSettings = _readConfig.read();
                //Console.WriteLine(userModels.name + " " + userModels.password);
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, userModels.name));
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
                claims.Add(new Claim(ClaimTypes.Email, "3401531269@qq.com"));

                SenSorData._Id = userModels.Id; //存储用户ID
                _cacheMy.setCacge("userId", userModels.Id);
              
                string jwt = JwtSettings.buildeToken(claims, jwtSettings.SecKey, DateTime.Now.AddSeconds(jwtSettings.ExpireSeconds));
                SocketConfig socketConfig = new SocketConfig();
                socketConfig.StartAsync(); // 启动socket监听
                return jwt;
            }
            else
            {
                return "登入失败";
            }
        }
        [HttpPost("RegisterUser", Name = "RegisterUser")]
        public string registerUser([FromBody] RegisterModel userModels) //用户注册
        {

            Console.WriteLine("注册功能触发");

            if (userModels == null) return "参数为空";
            if (_sqlToolsServices.insertSql(userModels))
            {
                return "注册成功";
            }
            else
            {
                return "注册失败";
            }
        }
        [HttpGet("GetWarningTimes", Name = "GetWarningTimes")]
        public string GetWarningTimes() //获取警告次数
        {
            Console.WriteLine("获取警告次数功能触发");
            int warningTimes = SenSorData._senSorDataInt[0];
            return $"当前警告次数为: {warningTimes}";
        }

        [HttpGet("GetLightStatus", Name = "GetLightStatus")]
        public int GetLightStatus(int id)
        {
            //bool result = _sqlToolsServices.getLightStatus(id);
            //if (result)
            //{
            //    return 1; // 返回灯光状态，1表示亮，0表示灭
            //}
            //Console.WriteLine("获取灯光状态功能触发");
            ushort result= (ushort)_cacheMy.getCacge("lightStatus"); // 从缓存中获取灯光状态
            if (result == 1)
            {
                return result;
            }
            Console.WriteLine("获取灯光状态功能触发");
            return 0;
        }

        [HttpPost("SetLightStatus", Name = "SetLightStatus")]
        public async Task<int> SetLightStatus(string name, ushort status) //设置灯光状态1开启，0关闭
        {
            Console.WriteLine("设置灯光状态功能触发");
            await  _esp8266Connect.communicationWrite(0x0006,status,"temp"); // 发送状态到ESP8266
            _cacheMy.setCacge("lightStatus", status);// 更新缓存中的灯光状态
            return 1; // 设置成功
           
        }

        [HttpPost("InsertDevice", Name = "InsertDevice")]
        public string InsertDevice([FromBody] DeviceModel deviceModel) //插入设备
        {
            Console.WriteLine("插入设备功能触发");
            if (deviceModel == null) return "参数为空";
            _sqlToolsServices.insertDevice(deviceModel);
            return "插入设备成功";
        }

        [HttpGet("getCurrentDeviceStatus", Name = "getDeviceStatus")]
        public string GetDeviceStatus() { 
            string result=(string) _cacheMy.getCacge("ConnectStatus"); // 从缓存中获取设备状态
            return result;
        }
    }
 }
