using TempModbusProject.Service.IService;

namespace TempModbusProject.Service
{
    public class ConsoleChangeColor : IConsoleChangeColors
    {
        public void ConsoleChangeRed(String str)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(str);
            Console.ForegroundColor = originalColor;
        }
    }
   
}
