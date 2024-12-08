namespace Task_Management_System.Models
{
    public class AppSettingsModel
    {
        public string PostgresDbConn { get; set; } = string.Empty;
        public string JWT_Key { get; set; } = string.Empty;
        public string JWT_Issuer { get; set; } = string.Empty;
        public string JWT_Audience { get; set; } = string.Empty;
        public string Encryption_Key { get; set; } = string.Empty;
        public string Encyption_IV { get; set; } = string.Empty;

    }
}
