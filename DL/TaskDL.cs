using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Task_Managament_System.Models;
using Task_Managament_System.Repositories;
using Task_Management_System.Constants;

namespace Task_Managament_System.DL
{
    public class TaskDL : ITaskRepository
    {
        public async Task<GetAllTasksRS> GetAllTasksAsync(Guid user_id)
        {
            var oGetAllTasksRS = new GetAllTasksRS();
            List<TaskModel> tasks = new List<TaskModel>();

            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "SELECT * FROM \"Tasks\" WHERE user_id = @user_id";
                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                TaskModel taskModel = new TaskModel
                                {
                                    task_id = reader.GetFieldValue<Guid>(reader.GetOrdinal("task_id")),
                                    task_title = reader.GetString(reader.GetOrdinal("task_title")),
                                    task_description = reader.GetString(reader.GetOrdinal("task_description")),
                                    task_category = reader.GetString(reader.GetOrdinal("task_category")),
                                    task_created_at = reader.GetDateTime(reader.GetOrdinal("task_created_at")),
                                    user_id = reader.GetFieldValue<Guid>(reader.GetOrdinal("user_id"))
                                };

                                tasks.Add(taskModel);
                            }
                        }
                    }
                }

                oGetAllTasksRS.status = "Success";
                oGetAllTasksRS.statusCode = 1;
                oGetAllTasksRS.statusMessage = $"Total Tasks: {tasks.Count}";
                oGetAllTasksRS.tasks = tasks;

            }
            catch(Exception ex)
            {
                oGetAllTasksRS.status = "Failed";
                oGetAllTasksRS.statusCode = 0;
                oGetAllTasksRS.statusMessage = ex.Message;
            }

            return oGetAllTasksRS;
        }

        public async Task<TaskRS> GetTaskAsync(Guid user_id, GetTaskRQ getTaskRQ)
        {
            var oGetTaskRS = new TaskRS();
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "SELECT * FROM \"Tasks\" WHERE user_id = @user_id AND task_id = @task_id";
                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        cmd.Parameters.AddWithValue("@task_id", getTaskRQ.task_id);

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                TaskModel taskModel = new TaskModel
                                {
                                    task_id = reader.GetFieldValue<Guid>(reader.GetOrdinal("task_id")),
                                    task_title = reader.GetString(reader.GetOrdinal("task_title")),
                                    task_description = reader.GetString(reader.GetOrdinal("task_description")),
                                    task_category = reader.GetString(reader.GetOrdinal("task_category")),
                                    task_created_at = reader.GetDateTime(reader.GetOrdinal("task_created_at")),
                                    user_id = reader.GetFieldValue<Guid>(reader.GetOrdinal("user_id"))
                                };

                                oGetTaskRS.status = "Success";
                                oGetTaskRS.statusCode = 1;
                                oGetTaskRS.statusMessage = "User Fetched Successfully";
                                oGetTaskRS.task = taskModel;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oGetTaskRS.status = "Failed";
                oGetTaskRS.statusCode = 0;
                oGetTaskRS.statusMessage = ex.Message;
            }

            return oGetTaskRS;
        }
        public async Task<TaskRS> CreateTaskAsync(Guid user_id, TaskModel task)
        {
            var oCreateTaskRS = new TaskRS();

            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    task.task_id = Guid.NewGuid();
                    await dbConn.OpenAsync();
                    string query = "INSERT INTO \"Tasks\" (task_id, task_title, task_description, task_category, task_created_at, user_id) VALUES (@task_id, @task_title, @task_description, @task_category, @created_at, @user_id)";

                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@task_id", task.task_id);
                        cmd.Parameters.AddWithValue("@task_title", task.task_title);
                        cmd.Parameters.AddWithValue("@task_description", task.task_description);
                        cmd.Parameters.AddWithValue("@task_category", task.task_category);
                        cmd.Parameters.AddWithValue("@created_at", task.task_created_at);
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    GetTaskRQ getTaskRQ = new GetTaskRQ { task_id = task.task_id };
                    var oGetTaskRS = await GetTaskAsync(user_id, getTaskRQ);

                    oCreateTaskRS.status = "Success";
                    oCreateTaskRS.statusCode = 1;
                    oCreateTaskRS.statusMessage = "Task added successfully";
                    oCreateTaskRS.task = oGetTaskRS.task;
                }
            }
            catch (Exception ex)
            {
                oCreateTaskRS.status = "Failed";
                oCreateTaskRS.statusCode = 0;
                oCreateTaskRS.statusMessage = ex.Message;
            }

            return oCreateTaskRS;
        }

        public async Task<TaskRS> UpdateTaskAsync(Guid user_id, UpdateTaskRQ updateTaskRQ)
        {
            var oUpdateTaskRS = new TaskRS();
            DateTime createdAt = DateTime.Now;

            try
            {
                using(var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();

                    string get_CreatedTime_Query = "SELECT created_at FROM \"Users\" where user_id = @user_id";

                    string query = "UPDATE \"Tasks\" SET task_title = @task_title, task_description = @task_description, task_category = @task_category WHERE task_id = @task_id AND user_id = @user_id";

                    using (var cmd = new NpgsqlCommand(get_CreatedTime_Query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@task_id", updateTaskRQ.task_id);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                createdAt = reader.GetDateTime(reader.GetOrdinal("task_created_at"));
                            }
                        }
                    }

                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@task_id", updateTaskRQ.task_id);
                        cmd.Parameters.AddWithValue("@task_title", updateTaskRQ.task_title);
                        cmd.Parameters.AddWithValue("@task_description", updateTaskRQ.task_description);
                        cmd.Parameters.AddWithValue("@task_category", updateTaskRQ.task_category);
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    TaskModel task = new TaskModel
                    {
                        task_id = updateTaskRQ.task_id,
                        task_title = updateTaskRQ.task_title,
                        task_description = updateTaskRQ.task_description,
                        task_category = updateTaskRQ.task_category,
                        task_created_at = createdAt,
                        user_id = user_id,
                    };

                    oUpdateTaskRS.status = "Success";
                    oUpdateTaskRS.statusCode = 1;
                    oUpdateTaskRS.statusMessage = "Task Updated successfully";
                    oUpdateTaskRS.task = task;

                }
            }
            catch (Exception ex)
            {
                oUpdateTaskRS.status = "Failed";
                oUpdateTaskRS.statusCode = 0;
                oUpdateTaskRS.statusMessage = ex.Message;
            }
            return oUpdateTaskRS;
        }

        public async Task<TaskRS> DeleteTaskAsync(Guid user_id, DeleteTaskRQ deleteTaskRQ)
        {
            var oDeleteTaskRS = new TaskRS();
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "DELETE FROM \"Tasks\" WHERE task_id = @task_id AND user_id = @user_id";
                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@task_id", deleteTaskRQ.task_id);
                        cmd.Parameters.AddWithValue("@user_id", user_id);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    GetTaskRQ getTaskRQ = new GetTaskRQ { task_id = deleteTaskRQ.task_id };
                    var deleteUserRS = await GetTaskAsync(user_id, getTaskRQ);

                    if (deleteUserRS != null)
                    {
                        oDeleteTaskRS.status = "Success";
                        oDeleteTaskRS.statusCode = 1;
                        oDeleteTaskRS.statusMessage = $"Task: {deleteUserRS.task.task_title} deleted successfully";
                        oDeleteTaskRS.task = deleteUserRS.task;
                    }
                    else
                    {
                        oDeleteTaskRS.status = "Success";
                        oDeleteTaskRS.statusCode = 1;
                        oDeleteTaskRS.statusMessage = $"Task details not fetched but deleted.";
                    }
                }
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
