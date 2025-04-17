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

        public bool insertSql()
        {
            try
            {

                using (SqlConnection sqlConnection = new SqlConnection(_connectionString))
                {
                    sqlConnection.Execute(
                        "insert into userTabel (name,password) values (@name,@password)",
                        new { name = "user", password = "password" }
                        );
                }
                return true;
            }
            catch (Exception e)
            { 
                Console.WriteLine("插入失败"+e.Message);
                return false;
            }
        }
        public UserModels selectSql(string name,string password)
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
    }
}
