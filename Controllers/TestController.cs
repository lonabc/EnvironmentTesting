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
        IEsp8266Conncet _esp8266Conncet;


        public TestController(PortLineVm portLineVm,ICommunicationFactory communicationFactory,readConfig readConfig,SqlToolsServices sqlToolsServices,IEsp8266Conncet esp8266Conncet)
        {
            _portSuper = portLineVm;
            _communicationFactory = communicationFactory;
            _readConfig = readConfig;
            _sqlToolsServices = sqlToolsServices;
            _esp8266Conncet = esp8266Conncet;


        }
        [HttpPost("TestController", Name = "TestController")]
        public void TestController1()
        {
            _esp8266Conncet.connectEsp8266Async("47.121.112.154", 1883);
           
        }
        [HttpPost("GetController", Name = "GetController")]
        public async void GetController()
        {
            await _esp8266Conncet.subTopicEsp8266("temp");
        }
     
    
    }
}
