using System.Threading.Tasks;
using Task_Managament_System.Models;
using Task_Managament_System.Repositories;

namespace Task_Managament_System.BL
{
    public class TaskBL
    {
        public async Task<GetAllTasksRS> GetAllTasksAsync(Guid user_id, ITaskRepository taskRepository, HttpClient client)
        {
            var oGetAllTasksRS = new GetAllTasksRS();
            try
            {
                oGetAllTasksRS = await taskRepository.GetAllTasksAsync(user_id);
            }
            catch (Exception ex)
            {
                oGetAllTasksRS.status = "Failed";
                oGetAllTasksRS.statusCode = 0;
                oGetAllTasksRS.statusMessage = ex.Message;
            }

            return oGetAllTasksRS;

        }

        public async Task<TaskRS> GetTaskAsync(Guid user_id, GetTaskRQ getTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oGetTaskRS = new TaskRS();
            try
            {
                oGetTaskRS = await taskRepository.GetTaskAsync(user_id, getTaskRQ);
            }
            catch (Exception ex)
            {
                oGetTaskRS.status = "Failed";
                oGetTaskRS.statusCode = 0;
                oGetTaskRS.statusMessage = ex.Message;
            }

            return oGetTaskRS;
        }

        public async Task<TaskRS> CreateTaskAsync(Guid user_id, TaskModel task, ITaskRepository taskRepository, HttpClient client)
        {
            var oCreateTaskRS = new TaskRS();
            try
            {
                oCreateTaskRS = await taskRepository.CreateTaskAsync(user_id, task);
            }
            catch (Exception ex)
            {
                oCreateTaskRS.status = "Failed";
                oCreateTaskRS.statusCode = 0;
                oCreateTaskRS.statusMessage = ex.Message;
            }

            return oCreateTaskRS;
        }

        public async Task<TaskRS> UpdateTaskAsync(Guid user_id, UpdateTaskRQ updateTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oUpdateTaskRS = new TaskRS();
            try
            {
                oUpdateTaskRS = await taskRepository.UpdateTaskAsync(user_id, updateTaskRQ);
            }
            catch (Exception ex)
            {
                oUpdateTaskRS.status = "Failed";
                oUpdateTaskRS.statusCode = 0;
                oUpdateTaskRS.statusMessage = ex.Message;
            }

            return oUpdateTaskRS;
        }

        public async Task<TaskRS> DeleteTaskAsync(Guid user_id, DeleteTaskRQ deleteTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oDeleteTaskRS = new TaskRS();
            try
            {
                oDeleteTaskRS = await taskRepository.DeleteTaskAsync(user_id, deleteTaskRQ);
            }
            catch (Exception ex)
            {
                oDeleteTaskRS.status = "Failed";
                oDeleteTaskRS.statusCode = 0;
                oDeleteTaskRS.statusMessage = ex.Message;
            }

            return oDeleteTaskRS;
        }
    }
}
