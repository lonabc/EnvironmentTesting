using Dapper;
using Microsoft.Data.SqlClient;
using System.Data.SqlClient;
using System.Threading;
using TempModbusProject.Configure;
using TempModbusProject.Model;

namespace TempModbusProject.Service
{
    public class SqlToolsServices
    {
        private string _connectionString;
        private readConfig _readConfig;
        public SqlToolsServices(readConfig readConfig)
        {
            _readConfig = readConfig;
            _connectionString = _readConfig.readSqlConnectSetting(); //读取配置文件   
        }

        public bool insertSql(RegisterModel userModels)
        {
            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Execute(
                        "insert into userTabel (name,password,email) values (@name,@password,@email)",
                        new { name = userModels.name, password = userModels.password, email = userModels.email }
                        );
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("插入失败" + e.Message);
                return false;
            }
        }
        public UserModels selectSql(string name, string password)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    var result = sqlConnection.Query<UserModels>(
                        "select * from userTabel where name=@name and password=@password",
                        new { name, password }
                        );
                    return result.FirstOrDefault();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("查询失败" + e.Message);
                return null;
            }
        }

        public void insertDevice(DeviceModel deviceModel)
        {
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    var result = sqlConnection.Execute(
                        "insert into device (userId,warningTimes,lightStaStus,deviceId) values (@userId,@warningTimes,@lightStatus,@deviceId)",
                        new { userId = deviceModel.UserId, warningTimes = deviceModel.WarningTimes, lightStatus = deviceModel.LightStatus, deviceId=deviceModel.DeviceId}
                        );

                }
            }
            catch (Exception e)
            {

                Console.WriteLine("插入设备失败" + e.Message);
            }
        }

        public bool getLightStatus(int deviceId)
        {
            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    var result = sqlConnection.Query<char>(
                          "select lightStaStus  from device where deviceId =@deviceId",
                          new { deviceId }
                          );
                    if (result.Equals('Y')) return true;
                    else return false;
                }
               ;
            }
            catch (Exception e)
            {
                Console.WriteLine("插入设备失败" + e.Message);
                return false;
            }
        }

        public bool setLightStatus(int deviceId, ushort lightStatus)
        {
            char status = lightStatus == 1 ? 'Y' : 'N'; // 将状态转换为字符
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Execute(
                        "update device set lightStaStus = @lightStatus where deviceId = @deviceId",
                        new { lightStatus= status, deviceId }
                        );
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("更新设备状态失败" + e.Message);
                return false;
            }

        }
    }
}
