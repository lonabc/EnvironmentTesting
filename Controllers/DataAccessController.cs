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


        [HttpPost("LoginTest", Name = "LoginTest")]
        public string LoginTest([FromBody] UserModels userModels) //用户登录
        {
            Console.WriteLine("登入功能触发");
            if (userModels == null) return "参数为空";

            if (_sqlToolsServices.selectSql(userModels.name, userModels.password) != null)
            {
                JwtSettings jwtSettings = _readConfig.read();
                Console.WriteLine(userModels.name + " " + userModels.password);
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, userModels.name));
                claims.Add(new Claim(ClaimTypes.Role, "admin"));
                claims.Add(new Claim(ClaimTypes.Email, "3401531269@qq.com"));
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
    }
}
