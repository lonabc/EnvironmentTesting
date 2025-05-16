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
        ICommunication _communication;
        ICommunication _espCommuntication;
        readConfig _readConfig;
        SqlToolsServices _sqlToolsServices;
       


        public TestController(PortLineVm portLineVm,ICommunicationFactory communicationFactory,readConfig readConfig,SqlToolsServices sqlToolsServices)
        {
            _portSuper = portLineVm;
         
            _readConfig = readConfig;
            _sqlToolsServices = sqlToolsServices;
            _espCommuntication = communicationFactory.Create(0.0f,0,"Esp8266");
            _communication = communicationFactory.Create(0.0f, 0, "Modbus");

        }
        [HttpPost("TestController", Name = "TestController")]
        public void TestController1()
        {
           _espCommuntication.communicationInit(1883.ToString());
           
        }
        [HttpPost("GetController", Name = "GetController")]
        public async void GetController() 
        {
            await _espCommuntication.subTopicEsp8266("test");

            await _espCommuntication.communicationSend("test",1,"temp",0x0004);

        }
     
    
    }
}
