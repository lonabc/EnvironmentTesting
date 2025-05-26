using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TempModbusProject.Model
{
 
    public class DeviceModel
    {
        
        public int DeviceId { get; set; }

   
        public int UserId { get; set; }

    
        public string? Location { get; set; }

        public int? Temperature { get; set; }  // 注意: 原表中可能是拼写错误(temeraption)，这里建议更正为Temperature

        public string? AirPollution { get; set; }

      
        public int? WarningTimes { get; set; }

       
        public char? LightStatus { get; set; }  // 注意: 原表中可能是拼写错误(lightStaStus)，这里建议更正为LightStatus

       
        public bool IsDeleted { get; set; } = false;

       
        public DateTime CreateTime { get; set; } = DateTime.Now;

        public DateTime UpdateTime { get; set; } = DateTime.Now;

             
    }
}
