using System.ComponentModel.DataAnnotations;

namespace Task_Management_System.Models
{
    public class Login
    {
        [Required]
        public string user_email { get; set; } = string.Empty;
        [Required]
        public string user_password { get; set; } = string.Empty;
    }

    public class LoginRS
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }

    public class ValidateTokenRS
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public string? user_id { get; set; }
    }

}
