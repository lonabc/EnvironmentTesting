using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TempModbusProject.Configure;
using TempModbusProject.Configure.AttributeConfigure;
using TempModbusProject.Model;
using TempModbusProject.Service;

namespace TempModbusProject.Controllers
{
    [ApiController]
    public class DataAccessController : Controller
    {
        readConfig _readConfig;
        SqlToolsServices _sqlToolsServices;
        public DataAccessController(readConfig readConfig, SqlToolsServices sqlToolsServices)
        {
            _readConfig = readConfig;
            _sqlToolsServices = sqlToolsServices;
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

            if (_sqlToolsServices.selectSql(userModels.name, userModels.password) != null)
            {
                JwtSettings jwtSettings = _readConfig.read();
                //Console.WriteLine(userModels.name + " " + userModels.password);
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, userModels.name));
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
                claims.Add(new Claim(ClaimTypes.Email, "3401531269@qq.com"));

                SenSorData._Id = userModels.Id; //存储用户ID
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
        public int GetErrorTimes(int id)
        {
            bool result = _sqlToolsServices.getLightStatus(id);
            if (result)
            {
                return 1; // 返回灯光状态，1表示亮，0表示灭
            }
            Console.WriteLine("获取灯光状态功能触发");
            return 0;
        }

        [HttpPost("SetLightStatus", Name = "SetLightStatus")]
        public int SetLightStatus(int id, char status) //设置灯光状态
        {
            Console.WriteLine("设置灯光状态功能触发");
            if (_sqlToolsServices.setLightStatus(id, status))
            {
                return 1; // 设置成功
            }
            else
            {
                return 0; // 设置失败
            }
        }
    }
}
