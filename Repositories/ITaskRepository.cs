using Task_Managament_System.Models;

namespace Task_Managament_System.Repositories
{
    public interface ITaskRepository
    {
        Task<GetAllTasksRS> GetAllTasksAsync(string? user_id);
        Task<TaskRS> GetTaskAsync(string? user_id, GetTaskRQ getTaskRQ);
        Task<TaskRS> CreateTaskAsync(string? user_id, TaskModel task);
        Task<TaskRS> UpdateTaskAsync(string? user_id, UpdateTaskRQ updateTaskRQ);
        Task<TaskRS> DeleteTaskAsync(string? user_id, DeleteTaskRQ deleteTaskRQ);


    }
}
