using Microsoft.Extensions.Options;
using Task_Management_System.Models;

namespace Task_Management_System.Constants
{
    public class TaskConstant
    {
        private static IOptions<AppSettingsModel> _appSettings;

        public static void Initialize(IOptions<AppSettingsModel> appSettings)
        {
            _appSettings = appSettings;
        }
        public static string PostgresDbConn { get { return _appSettings.Value.PostgresDbConn; } }
        public static string JWT_Key { get { return _appSettings.Value.JWT_Key; } }
        public static string JWT_Issuer { get { return _appSettings.Value.JWT_Issuer; } }
        public static string JWT_Audience { get { return _appSettings.Value.JWT_Audience; } }
        public static string Encryption_Key { get { return _appSettings.Value.Encryption_Key; } }
        public static string Encyption_IV { get { return _appSettings.Value.Encyption_IV; } }
    }
}
