using Task_Management_System.Models;

namespace Task_Managament_System.Models
{
    public class TaskModel
    {
        public Guid task_id { get; set; }
        public string task_title { get; set; } = string.Empty;
        public string task_description { get; set; } = string.Empty;
        public string task_category { get; set; } = string.Empty;
        public DateTime task_created_at { get; set; } = DateTime.Now;
        public string user_id { get; set; } = string.Empty;
    }

    public class TaskUserInfo
    {
        public Guid task_id { get; set; }
        public string task_title { get; set; } = string.Empty;
        public string task_description { get; set; } = string.Empty;
        public string task_category { get; set; } = string.Empty;
        public DateTime task_created_at { get; set; } = DateTime.Now;
        public User user { get; set; } = new User();
    }

    public class GetTaskRQ
    {
        public Guid task_id { get; set; }
    }

    public class UpdateTaskRQ
    {
        public Guid task_id { get; set; }
        public string task_title { get; set; } = string.Empty;
        public string task_description { get; set; } = string.Empty;
        public string task_category { get; set; } = string.Empty;
        public Guid user_id { get; set; }
    }

    public class DeleteTaskRQ 
    {
        public Guid task_id { get; set; }
    }

    public class GetAllTasksRS 
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public List<TaskModel> tasks { get; set; } = [];
    }

    public class TaskRS
    {
        public string status { get; set; } = string.Empty;
        public int statusCode { get; set; }
        public string statusMessage { get; set; } = string.Empty;
        public TaskModel? task { get; set; }
    }

    public class TaskDescriptionRQ
    {
        public Guid? task_id { get; set; }
        public string? tasKTitle { get; set; }
    }

    public class TaskDescriptionRS
    {
        public string? status { get; set; }
        public int statusCode { get; set; }
        public string? statusMessage { get; set; }
        public string? title { get; set; }
        public string? extract { get; set; }

    }

}
