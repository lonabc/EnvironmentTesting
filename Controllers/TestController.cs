using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TempModbusProject.Configure;
using TempModbusProject.Model;
using TempModbusProject.Service;
using TempModbusProject.Service.IService;

namespace TempModbusProject.Controllers
{



    [ApiController]

    public class TestController : Controller
    {

        PortLineVm _portSuper;

        ICommunicationFactory _communicationFactory;
        readConfig _readConfig;
        SqlToolsServices _sqlToolsServices;

        public TestController(PortLineVm portLineVm,ICommunicationFactory communicationFactory,readConfig readConfig,SqlToolsServices sqlToolsServices)
        {
            _portSuper = portLineVm;
            _communicationFactory = communicationFactory;
            _readConfig = readConfig;
            _sqlToolsServices = sqlToolsServices;


        }

        [HttpPost("TestController", Name = "TestController")]
        public void TestController1()
        {
            _readConfig.readSqlConnectSetting(); //读取配置文件   
           UserModels userModels= _sqlToolsServices.selectSql("user","password");
            Console.WriteLine(userModels.name + " " + userModels.password);

        }

       
        [HttpGet("ModbusReadTest", Name = "ModbusRead")]
        [Authorize]
        public void ModbusReadTest(String id)
        {
            _portSuper.MessageInit(id); //初始化端口
            _portSuper.ReadHoldingRegisters(0x0000,4);
            

        }
        [HttpPost("LoginTest", Name = "LoginTest")]
        public string LoginTest([FromBody]UserModels userModels)
        {
            Console.WriteLine("登入功能触发");
        
            if (userModels == null) return "参数为空";
           
            if ( _sqlToolsServices.selectSql(userModels.name, userModels.password)!=null)
            {
                JwtSettings jwtSettings = _readConfig.read();
                Console.WriteLine(userModels.name+" "+userModels.password);
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name,userModels.name));
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
