using Task_Managament_System.Models;

namespace Task_Managament_System.Repositories
{
    public interface ITaskRepository
    {
        Task<GetAllTasksRS> GetAllTasksAsync(string? user_id, string correlationID);
        Task<TaskRS> GetTaskAsync(string? user_id, GetTaskRQ getTaskRQ, string correlationID);
        Task<TaskRS> CreateTaskAsync(string? user_id, TaskModel task, string correlationID);
        Task<TaskRS> UpdateTaskAsync(string? user_id, UpdateTaskRQ updateTaskRQ, string correlationID);
        Task<TaskRS> DeleteTaskAsync(string? user_id, DeleteTaskRQ deleteTaskRQ, string correlationID);


    }
}
