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
           // _readConfig.readSqlConnectSetting(); //读取配置文件   
           //UserModels userModels= _sqlToolsServices.selectSql("user","password");
            //Console.WriteLine(userModels.name + " " + userModels.password);
          
                throw new Exception();
           
        }
     
    
    }
}
