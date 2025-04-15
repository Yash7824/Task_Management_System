using Dapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Npgsql;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Task_Managament_System.BL;
using Task_Managament_System.Models;
using Task_Managament_System.Repositories;
using Task_Managament_System.Services;
using Task_Management_System.Constants;
using Task_Management_System.Models;

namespace Task_Managament_System.DL
{
    public class TaskDL : ITaskRepository
    {
        public async Task<GetAllTasksRS> GetAllTasksAsync(string? user_id, string correlationID)
        {
            var oGetAllTasksRS = new GetAllTasksRS();
            List<TaskModel> tasks = new List<TaskModel>();

            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = $"SELECT * FROM TASKS WHERE USER_ID = @user_id";
                    var parameters = new { user_id };
                    tasks = (await dbConn.QueryAsync<TaskModel>(query, parameters)).ToList();

                    oGetAllTasksRS.status = "Success";
                    oGetAllTasksRS.statusCode = 0;
                    oGetAllTasksRS.statusMessage = $"Tasks Fetched Successfully And Total Tasks: {tasks.Count}";
                    oGetAllTasksRS.tasks = tasks;
                }
            }
            catch(Exception ex)
            {
                oGetAllTasksRS.status = "Failed";
                oGetAllTasksRS.statusCode = 2;
                oGetAllTasksRS.statusMessage = $"Exception occurred in TaskDL.GetAllTasksAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskDL.GetAllTasksAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oGetAllTasksRS), correlationID, user_id);
            }

            return oGetAllTasksRS;
        }

        public async Task<TaskRS> GetTaskAsync(string? user_id, GetTaskRQ getTaskRQ, string correlationID)
        {
            var oGetTaskRS = new TaskRS();
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "SELECT * FROM TASKS WHERE USER_ID = @USER_ID AND TASK_ID = @TASK_ID";
                    var parameters = new { USER_ID = user_id, TASK_ID = getTaskRQ.task_id };
                    var result = await dbConn.QueryFirstOrDefaultAsync<TaskModel>(query, parameters);

                    oGetTaskRS.status = "Success";
                    oGetTaskRS.statusCode = 0;

                    if(result != null)
                    {
                        oGetTaskRS.statusMessage = "Task Fetched Successfully!";
                        oGetTaskRS.task = result;
                    }
                    else
                    {
                        oGetTaskRS.statusMessage = "Unable to Fetch the Task!";
                        oGetTaskRS.task = null;
                    }
                }
            }
            catch (Exception ex)
            {
                oGetTaskRS.status = "Failed";
                oGetTaskRS.statusCode = 2;
                oGetTaskRS.statusMessage = $"Exception occurred in TaskDL.GetTaskAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskDL.GetTaskAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oGetTaskRS), correlationID, user_id);
            }

            return oGetTaskRS;
        }
        public async Task<TaskRS> CreateTaskAsync(string? user_id, TaskModel task, string correlationID)
        {
            var oCreateTaskRS = new TaskRS();

            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    task.task_id = Guid.NewGuid();
                    task.user_id = user_id ?? string.Empty;
                    await dbConn.OpenAsync();
                    string query = "INSERT INTO TASKS (TASK_ID, TASK_TITLE, TASK_DESCRIPTION, TASK_CATEGORY, TASK_CREATED_AT, USER_ID) VALUES (@TASK_ID, @TASK_TITLE, @TASK_DESCRIPTION, @TASK_CATEGORY, @CREATED_AT, @USER_ID)";

                    var parameters = new
                    {
                        TASK_TITLE = task.task_title,
                        TASK_DESCRIPTION = task.task_description,
                        TASK_CATEGORY = task.task_category,
                        CREATED_AT = task.task_created_at,
                        TASK_ID = task.task_id,
                        USER_ID = user_id,
                    };
                    
                    int rowsAffected = await dbConn.ExecuteAsync(query, parameters);
                    if (rowsAffected > 0)
                    {
                        oCreateTaskRS.status = "Success";
                        oCreateTaskRS.statusCode = 0;
                        oCreateTaskRS.statusMessage = "Task Added Successfully!";
                        oCreateTaskRS.task = task;
                    }
                    else
                    {
                        oCreateTaskRS.status = "Failed";
                        oCreateTaskRS.statusCode = 1;
                        oCreateTaskRS.statusMessage = "Unable To Add Task!";
                    }
                }
            }
            catch (Exception ex)
            {
                oCreateTaskRS.status = "Failed";
                oCreateTaskRS.statusCode = 2;
                oCreateTaskRS.statusMessage = $"Exception occurred in TaskDL.CreateTaskAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskDL.CreateTaskAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oCreateTaskRS), correlationID, user_id);
            }

            return oCreateTaskRS;
        }

        public async Task<TaskRS> UpdateTaskAsync(string? user_id, UpdateTaskRQ updateTaskRQ, string correlationID)
        {
            var oUpdateTaskRS = new TaskRS();
            DateTime createdAt = DateTime.Now;

            oUpdateTaskRS.task = new TaskModel
            {
                task_id = updateTaskRQ.task_id,
                task_title = updateTaskRQ.task_title,
                task_description = updateTaskRQ.task_description,
                task_category = updateTaskRQ.task_category,
                task_created_at = createdAt,
                user_id = user_id ?? string.Empty
            };

            try
            {
                using(var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "UPDATE TASKS SET TASK_TITLE = @TASK_TITLE, TASK_DESCRIPTION = @TASK_DESCRIPTION, TASK_CATEGORY = @TASK_CATEGORY WHERE TASK_ID = @TASK_ID AND USER_ID = @USER_ID";

                    var parameters = new
                    {
                        TASK_TITLE = updateTaskRQ.task_title,
                        TASK_DESCRIPTION = updateTaskRQ.task_description,
                        TASK_CATEGORY = updateTaskRQ.task_category,
                        TASK_ID = updateTaskRQ.task_id,
                        USER_ID = user_id,
                    };

                    int rowsAffected = await dbConn.ExecuteAsync(query, parameters);
                    if (rowsAffected > 0)
                    {
                        oUpdateTaskRS.status = "Success";
                        oUpdateTaskRS.statusCode = 0;
                        oUpdateTaskRS.statusMessage = $"Task Updated successfully with ID: {updateTaskRQ.task_id}";
                    }
                    else
                    {
                        oUpdateTaskRS.status = "Failed";
                        oUpdateTaskRS.statusCode = 1;
                        oUpdateTaskRS.statusMessage = $"Unable To Update the Task with ID: {updateTaskRQ.task_id}";
                    }
                }
            }
            catch (Exception ex)
            {
                oUpdateTaskRS.status = "Failed";
                oUpdateTaskRS.statusCode = 2;
                oUpdateTaskRS.statusMessage = $"Exception occurred in TaskDL.UpdateTaskAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskDL.UpdateTaskAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oUpdateTaskRS), correlationID, user_id);
            }
            return oUpdateTaskRS;
        }

        public async Task<TaskRS> DeleteTaskAsync(string? user_id, DeleteTaskRQ deleteTaskRQ, string correlationID)
        {
            var oDeleteTaskRS = new TaskRS();
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "DELETE FROM TASKS WHERE TASK_ID = @TASK_ID AND USER_ID = @USER_ID";
                    var parameters = new { USER_ID = user_id, TASK_ID = deleteTaskRQ.task_id };
                    int rowsAffected = await dbConn.ExecuteAsync(query, parameters);

                    if (rowsAffected > 0)
                    {
                        oDeleteTaskRS.status = "Success";
                        oDeleteTaskRS.statusCode = 0;
                        oDeleteTaskRS.statusMessage = $"Task with Task ID: {deleteTaskRQ.task_id} Deleted Successfully";
                    }
                    else
                    {
                        oDeleteTaskRS.status = "Failed";
                        oDeleteTaskRS.statusCode = 1;
                        oDeleteTaskRS.statusMessage = $"Unable To Delete the Task with ID: {deleteTaskRQ.task_id}";
                    }
                }
            }
            catch (Exception ex)
            {
                oDeleteTaskRS.status = "Failed";
                oDeleteTaskRS.statusCode = 2;
                oDeleteTaskRS.statusMessage = $"Exception occurred in TaskDL.DeleteTaskAsync(): {ex.Message}";
                await DBLogger.InsertLog("TaskDL.DeleteTaskAsync()", ex.Message, ex.StackTrace, user_id, JsonConvert.SerializeObject(oDeleteTaskRS), correlationID, user_id);
            }

            return oDeleteTaskRS;
        }
       
    }
}
