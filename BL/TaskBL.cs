using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Web;
using Task_Managament_System.Models;
using Task_Managament_System.Repositories;
using Task_Managament_System.Services;
using Task_Management_System.Constants;
using Task_Management_System.Models;

namespace Task_Managament_System.BL
{
    public class TaskBL
    {
        public async Task<GetAllTasksRS> GetAllTasksAsync(string? user_id, ITaskRepository taskRepository, HttpClient client)
        {
            var oGetAllTasksRS = new GetAllTasksRS();
            string correlationID = Guid.NewGuid().ToString();
            try
            {
                oGetAllTasksRS = await taskRepository.GetAllTasksAsync(user_id, correlationID);
            }
            catch (Exception ex)
            {
                oGetAllTasksRS.status = "Failed";
                oGetAllTasksRS.statusCode = 2;
                oGetAllTasksRS.statusMessage = $"Exception occurred in TaskBL.GetAllTasksAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskBL.GetAllTasksAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oGetAllTasksRS), correlationID, user_id);
            }

            return oGetAllTasksRS;

        }

        public async Task<TaskRS> GetTaskAsync(string? user_id, GetTaskRQ getTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oGetTaskRS = new TaskRS();
            string correlationID = Guid.NewGuid().ToString();
            try
            {
                oGetTaskRS = await taskRepository.GetTaskAsync(user_id, getTaskRQ, correlationID);
            }
            catch (Exception ex)
            {
                oGetTaskRS.status = "Failed";
                oGetTaskRS.statusCode = 2;
                oGetTaskRS.statusMessage = $"Exception occurred in TaskBL.GetTaskAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskBL.GetTaskAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oGetTaskRS), correlationID, user_id);
            }

            return oGetTaskRS;
        }

        public async Task<TaskRS> CreateTaskAsync(string? user_id, TaskModel task, ITaskRepository taskRepository, HttpClient client)
        {
            var oCreateTaskRS = new TaskRS();
            string correlationID = Guid.NewGuid().ToString();
            try
            {
                oCreateTaskRS = await taskRepository.CreateTaskAsync(user_id, task, correlationID);
            }
            catch (Exception ex)
            {
                oCreateTaskRS.status = "Failed";
                oCreateTaskRS.statusCode = 2;
                oCreateTaskRS.statusMessage = $"Exception occurred in TaskBL.CreateTaskAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskBL.CreateTaskAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oCreateTaskRS), correlationID, user_id);
            }

            return oCreateTaskRS;
        }

        public async Task<TaskRS> UpdateTaskAsync(string? user_id, UpdateTaskRQ updateTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oUpdateTaskRS = new TaskRS();
            string correlationID = Guid.NewGuid().ToString();
            try
            {
                oUpdateTaskRS = await taskRepository.UpdateTaskAsync(user_id, updateTaskRQ, correlationID);
            }
            catch (Exception ex)
            {
                oUpdateTaskRS.status = "Failed";
                oUpdateTaskRS.statusCode = 2;
                oUpdateTaskRS.statusMessage = $"Exception occurred in TaskBL.UpdateTaskAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskBL.UpdateTaskAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oUpdateTaskRS), correlationID, user_id);
            }

            return oUpdateTaskRS;
        }

        public async Task<TaskRS> DeleteTaskAsync(string? user_id, DeleteTaskRQ deleteTaskRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oDeleteTaskRS = new TaskRS();
            string correlationID = Guid.NewGuid().ToString();
            try
            {
                oDeleteTaskRS = await taskRepository.DeleteTaskAsync(user_id, deleteTaskRQ, correlationID);
            }
            catch (Exception ex)
            {
                oDeleteTaskRS.status = "Failed";
                oDeleteTaskRS.statusCode = 2;
                oDeleteTaskRS.statusMessage = $"Exception occurred in TaskBL.DeleteTaskAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskBL.DeleteTaskAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oDeleteTaskRS), correlationID, user_id);
            }

            return oDeleteTaskRS;
        }

        public async Task<TaskDescriptionRS> GenerateTaskDescriptionAsync(string? user_id, TaskDescriptionRQ oTaskDescriptionRQ, ITaskRepository taskRepository, HttpClient client)
        {
            var oTaskDescriptionRS = new TaskDescriptionRS();
            try
            {
                string url = oTaskDescriptionRQ.tasKTitle != null ? string.Format(TaskConstant.WikiDescriptionURL, Uri.EscapeDataString(oTaskDescriptionRQ.tasKTitle)) : string.Empty;
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode && response.Content != null)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseContent = JsonConvert.DeserializeObject<TaskDescriptionRS>(content);
                    if (responseContent != null)
                    {
                        oTaskDescriptionRS.status = "Success";
                        oTaskDescriptionRS.statusCode = 0;
                        oTaskDescriptionRS.statusMessage = string.Empty;
                        oTaskDescriptionRS.title = responseContent.title;
                        oTaskDescriptionRS.extract = responseContent.extract;
                    }
                    else
                    {
                        oTaskDescriptionRS.status = "Failed";
                        oTaskDescriptionRS.statusCode = 1;
                        oTaskDescriptionRS.statusMessage = "Unable to fetch the description!";
                        oTaskDescriptionRS.title = oTaskDescriptionRQ.tasKTitle;
                        oTaskDescriptionRS.extract = string.Empty;
                    }
                }
                else
                {
                    oTaskDescriptionRS.status = "Failed";
                    oTaskDescriptionRS.statusCode = 1;
                    oTaskDescriptionRS.statusMessage = "Unable to fetch response!";
                    oTaskDescriptionRS.title = oTaskDescriptionRQ.tasKTitle;
                    oTaskDescriptionRS.extract = string.Empty;
                }
            }
            catch (Exception ex)
            {
                oTaskDescriptionRS.status = "Failed";
                oTaskDescriptionRS.statusCode = 2;
                oTaskDescriptionRS.statusMessage = $"Exception occurred in TaskBL.GenerateTaskDescriptionAsync(): {ex.Message}";
                oTaskDescriptionRS.title = oTaskDescriptionRQ.tasKTitle;
                oTaskDescriptionRS.extract = string.Empty;
                await DBLogger.InsertLog("TaskBL.GenerateTaskDescriptionAsync()", ex.Message, ex.StackTrace, JsonConvert.SerializeObject(oTaskDescriptionRQ), JsonConvert.SerializeObject(oTaskDescriptionRS), Guid.NewGuid().ToString(), user_id);
            }

            return oTaskDescriptionRS;
        }
    }
}
