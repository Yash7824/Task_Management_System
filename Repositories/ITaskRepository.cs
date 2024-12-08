using Task_Managament_System.Models;

namespace Task_Managament_System.Repositories
{
    public interface ITaskRepository
    {
        Task<GetAllTasksRS> GetAllTasksAsync(Guid user_id);
        Task<TaskRS> GetTaskAsync(Guid user_id, GetTaskRQ getTaskRQ);
        Task<TaskRS> CreateTaskAsync(Guid user_id, TaskModel task);
        Task<TaskRS> UpdateTaskAsync(Guid user_id, UpdateTaskRQ updateTaskRQ);
        Task<TaskRS> DeleteTaskAsync(Guid user_id, DeleteTaskRQ deleteTaskRQ);


    }
}
