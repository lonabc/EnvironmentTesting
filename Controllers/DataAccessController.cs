using Microsoft.AspNetCore.Mvc;
using TempModbusProject.Configure;
using TempModbusProject.Model;

namespace TempModbusProject.Controllers
{
    [ApiController]
    public class DataAccessController : Controller
    {
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
    }
}
