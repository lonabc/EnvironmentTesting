using Microsoft.Extensions.Options;
using System.Runtime;
using TempModbusProject.Model;

namespace TempModbusProject.Configure
{
    public class readConfig
    {
        private readonly IOptionsSnapshot<JwtSettings> optDbSettings;
        private readonly IOptionsSnapshot<SqlSettings> sqlConnectSettings;

        public readConfig(IOptionsSnapshot<JwtSettings> optDbSettings, IOptionsSnapshot<SqlSettings> sqlConnectSettings)
        {
            this.optDbSettings = optDbSettings;
            this.sqlConnectSettings = sqlConnectSettings;
        }

        public JwtSettings read()
        {
            var secKey = optDbSettings.Value.SecKey;
            var expireSeconds = optDbSettings.Value.ExpireSeconds;
     
            var jwtSettings = new JwtSettings
            {
                SecKey = secKey,
                ExpireSeconds = expireSeconds
            };
            return jwtSettings;
        }

        public String readSqlConnectSetting()
        {
          string sqlConnectString = sqlConnectSettings.Value.SqlConnection;
          
            return sqlConnectString;
        }
    }
}
