using System.Threading.Tasks;
using Task_Managament_System.Models;
using Task_Managament_System.Repositories;

namespace Task_Managament_System.BL
{
    public class TaskBL
    {
        public async Task<GetAllTasksRS> GetAllTasksAsync(string? user_id, ITaskRepository taskRepository, HttpClient client)
        {
            var oGetAllTasksRS = new GetAllTasksRS();
            try
            {
                oGetAllTasksRS = await taskRepository.GetAllTasksAsync(user_id);
            }
            catch (Exception ex)
            {
                oGetAllTasksRS.status = "Failed";
                oGetAllTasksRS.statusCode = 2;
                oGetAllTasksRS.statusMessage = $"Exception occurred in TaskBL.GetAllTasksAsync(): {ex.Message}";
            }

            return oGetAllTasksRS;

        }

        public async Task<TaskRS> GetTaskAsync(string? user_id, GetTaskRQ getTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oGetTaskRS = new TaskRS();
            try
            {
                oGetTaskRS = await taskRepository.GetTaskAsync(user_id, getTaskRQ);
            }
            catch (Exception ex)
            {
                oGetTaskRS.status = "Failed";
                oGetTaskRS.statusCode = 2;
                oGetTaskRS.statusMessage = $"Exception occurred in TaskBL.GetTaskAsync(): {ex.Message}";
            }

            return oGetTaskRS;
        }

        public async Task<TaskRS> CreateTaskAsync(string? user_id, TaskModel task, ITaskRepository taskRepository, HttpClient client)
        {
            var oCreateTaskRS = new TaskRS();
            try
            {
                oCreateTaskRS = await taskRepository.CreateTaskAsync(user_id, task);
            }
            catch (Exception ex)
            {
                oCreateTaskRS.status = "Failed";
                oCreateTaskRS.statusCode = 2;
                oCreateTaskRS.statusMessage = $"Exception occurred in TaskBL.CreateTaskAsync(): {ex.Message}";
            }

            return oCreateTaskRS;
        }

        public async Task<TaskRS> UpdateTaskAsync(string? user_id, UpdateTaskRQ updateTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oUpdateTaskRS = new TaskRS();
            try
            {
                oUpdateTaskRS = await taskRepository.UpdateTaskAsync(user_id, updateTaskRQ);
            }
            catch (Exception ex)
            {
                oUpdateTaskRS.status = "Failed";
                oUpdateTaskRS.statusCode = 2;
                oUpdateTaskRS.statusMessage = $"Exception occurred in TaskBL.UpdateTaskAsync(): {ex.Message}";
            }

            return oUpdateTaskRS;
        }

        public async Task<TaskRS> DeleteTaskAsync(string? user_id, DeleteTaskRQ deleteTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oDeleteTaskRS = new TaskRS();
            try
            {
                oDeleteTaskRS = await taskRepository.DeleteTaskAsync(user_id, deleteTaskRQ);
            }
            catch (Exception ex)
            {
                oDeleteTaskRS.status = "Failed";
                oDeleteTaskRS.statusCode = 2;
                oDeleteTaskRS.statusMessage = $"Exception occurred in TaskBL.DeleteTaskAsync(): {ex.Message}";
            }

            return oDeleteTaskRS;
        }
    }
}
