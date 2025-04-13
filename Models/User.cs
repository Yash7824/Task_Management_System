
namespace Task_Management_System.Models
{
    public class User
    {
        public string user_id { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
        public string user_email { get; set; } = string.Empty;
        public string user_password { get; set; } = string.Empty;
        public string mobile_num { get; set; } = string.Empty;
        public string[] tasks_assigned { get; set; } = [];
        public DateTime created_at { get; set; } = DateTime.Now;
    }

    public class UserCreated
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public User userDetails { get; set; } = new User();

    }

    public class GetUsers
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public List<User> Users { get; set; } = [];
    }

    public class GetUser
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public User? User { get; set; } = new User();
    }

    public class GetUserRQ
    {
        public string? userID { get; set; }
    }

    public class UpdateUserRQ
    {
        public string username { get; set; } = string.Empty;
        public string user_email { get; set; } = string.Empty;
        public string user_password { get; set; } = string.Empty;
        public string mobile_num { get; set; } = string.Empty;
        public string[] tasks_assigned { get; set; } = [];
    }

    public class UpdateUserRS 
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
    }

    public class DeleteUserRS
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
    }

    public class DecryptPasswordRQ
    {
        public string password { get; set; } = string.Empty;
    }

    public class DecryptPasswordRS
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public string encrypted_password { get; set; } = string.Empty;
        public string decrypyted_password { get; set; } = string.Empty;
    }
}
