using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Task_Management_System.Constants;
using Task_Management_System.Models;
using Task_Management_System.Repositories;
using Task_Management_System.Services;

namespace Task_Management_System.DL
{
    public class UserDL : IUserRepository
    {
        public async Task<UserCreated> CreateUserAsync(User user)
        {
            var oUserCreatedRs = new UserCreated();

            try
            {
                user.user_id = Guid.NewGuid();
                var encrypted_password = CommonMethod.EncryptAES(user.user_password);

                var oGetAllUsersRS = await GetUsersAsync();
                var userEmailPresent = oGetAllUsersRS.Users.Any(x => x.user_email == user.user_email);

                if (userEmailPresent)
                    return new UserCreated
                    {
                        status = "Failed",
                        statusCode = 0,
                        statusMessage = "User Email Already Exists"
                    };


                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {

                    await dbConn.OpenAsync();

                    var query = "INSERT INTO \"Users\" (user_id, username, user_email, user_password, mobile_num, created_at) VALUES (@user_id, @username, @user_email, @user_password, @mobile_num, @created_at)";

                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", user.user_id);
                        cmd.Parameters.AddWithValue("@username", user.username);
                        cmd.Parameters.AddWithValue("@user_email", user.user_email);
                        cmd.Parameters.AddWithValue("@user_password", encrypted_password);
                        cmd.Parameters.AddWithValue("@mobile_num", user.mobile_num);
                        cmd.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    oUserCreatedRs.status = "Success";
                    oUserCreatedRs.statusCode = 1;
                    oUserCreatedRs.statusMessage = "User Created Successfully";
                    user.user_password = encrypted_password;
                    oUserCreatedRs.userDetails = user;
                };
            }
            catch (Exception ex)
            {
                oUserCreatedRs.status = "Failed";
                oUserCreatedRs.statusCode = 0;
                oUserCreatedRs.statusMessage = ex.Message;
            }

            return oUserCreatedRs;
        }

        public async Task<GetUsers> GetUsersAsync()
        {
            var oGetUsersRs = new GetUsers();
            List<User> users = new List<User>();

            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    var query = "SELECT * FROM \"Users\"";

                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                User user = new User
                                {
                                    user_id = reader.GetFieldValue<Guid>(reader.GetOrdinal("user_id")),
                                    username = reader.GetString(reader.GetOrdinal("username")),
                                    user_email = reader.GetString(reader.GetOrdinal("user_email")),
                                    user_password = reader.GetString(reader.GetOrdinal("user_password")),
                                    mobile_num = reader.GetString(reader.GetOrdinal("mobile_num")),
                                    tasks_assigned = reader.IsDBNull(reader.GetOrdinal("tasks_assigned"))
                                         ? Array.Empty<string>()
                                         : reader.GetFieldValue<string[]>(reader.GetOrdinal("tasks_assigned")),
                                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at"))
                                };

                                users.Add(user);
                            }
                        }
                    }
                }

                oGetUsersRs.status = "Success";
                oGetUsersRs.statusCode = 1;
                oGetUsersRs.statusMessage = $"Total Number of Users: {users.Count}";
                oGetUsersRs.Users = users;
            }
            catch (Exception ex)
            {
                oGetUsersRs.status = "Failed";
                oGetUsersRs.statusCode = 0;
                oGetUsersRs.statusMessage = ex.Message;
            }

            return oGetUsersRs;
        }

        public async Task<GetUser> GetUserAsync(Guid guid)
        {
            var oGetUserRs = new GetUser();

            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "SELECT * FROM \"Users\" WHERE user_id = :userID";

                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("userID", guid);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                User user = new User
                                {
                                    user_id = reader.GetFieldValue<Guid>(reader.GetOrdinal("user_id")),
                                    username = reader.GetString(reader.GetOrdinal("username")),
                                    user_email = reader.GetString(reader.GetOrdinal("user_email")),
                                    user_password = reader.GetString(reader.GetOrdinal("user_password")),
                                    mobile_num = reader.GetString(reader.GetOrdinal("mobile_num")),
                                    tasks_assigned = reader.IsDBNull(reader.GetOrdinal("tasks_assigned")) ? 
                                        Array.Empty<string>() : reader.GetFieldValue<string[]>(reader.GetOrdinal("tasks_assigned")),
                                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at"))
                                };

                                oGetUserRs.status = "Success";
                                oGetUserRs.statusCode = 1;
                                oGetUserRs.statusMessage = "User Fetched Successfully";
                                oGetUserRs.User = user;
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                oGetUserRs.status = "Failed";
                oGetUserRs.statusCode = 0;
                oGetUserRs.statusMessage = $"Unable to fetch the user, exception: {ex.Message}";
            }

            return oGetUserRs;
        }

        public async Task<GetUser> UpdateUserAsync(Guid userId, UpdateUserRQ userRQ)
        {
            var oUpdateUserRS = new GetUser();
            DateTime createdAt = DateTime.Now;
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    var encrypted_password = CommonMethod.EncryptAES(userRQ.user_password);
                    await dbConn.OpenAsync();

                    string get_CreatedTime_Query = "SELECT created_at FROM \"Users\" where user_id = @user_id";

                    string query = "UPDATE \"Users\" SET username = @username, user_email = @user_email, user_password = @user_password, mobile_num = @mobile_num, tasks_assigned = @tasks_assigned WHERE user_id = @user_id";

                    using (var cmd = new NpgsqlCommand(get_CreatedTime_Query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if(await reader.ReadAsync())
                            {
                                createdAt = reader.GetDateTime(reader.GetOrdinal("created_at"));
                            }
                        }
                    }

                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@username", userRQ.username);
                        cmd.Parameters.AddWithValue("@user_email", userRQ.user_email);
                        cmd.Parameters.AddWithValue("@user_password", encrypted_password);
                        cmd.Parameters.AddWithValue("@mobile_num", userRQ.mobile_num);
                        cmd.Parameters.AddWithValue("@tasks_assigned", userRQ.tasks_assigned);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    User user = new User
                    {
                        user_id = userId,
                        username = userRQ.username,
                        user_email = userRQ.user_email,
                        user_password = encrypted_password,
                        mobile_num = userRQ.mobile_num,
                        created_at = createdAt,
                        tasks_assigned = userRQ.tasks_assigned,
                    };

                    oUpdateUserRS.status = "Success";
                    oUpdateUserRS.statusCode = 1;
                    oUpdateUserRS.statusMessage = "User Updated Successfully";
                    oUpdateUserRS.User = user;
                }
            }
            catch (Exception ex)
            {
                oUpdateUserRS.status = "Failed";
                oUpdateUserRS.statusCode = 0;
                oUpdateUserRS.statusMessage = $"Unable to update the user, exception: {ex.Message}";
            }

            return oUpdateUserRS;
        }

        public async Task<GetUser> DeleteUserAsync(Guid userId)
        {
            var oDeleteUserRS = new GetUser();
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "DELETE FROM \"Users\" WHERE user_id = @user_id";
                    using (var cmd = new NpgsqlCommand(query, dbConn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        await cmd.ExecuteNonQueryAsync();
                    }

                    var oGetUserRS = await GetUserAsync(userId);

                    if (oGetUserRS != null)
                    {
                        oDeleteUserRS.status = "Success";
                        oDeleteUserRS.statusCode = 1;
                        oDeleteUserRS.statusMessage = $"User: {oGetUserRS.User.username} deleted successfully";
                        oDeleteUserRS.User = oGetUserRS.User;
                    }
                    else
                    {
                        oDeleteUserRS.status = "Success";
                        oDeleteUserRS.statusCode = 1;
                        oDeleteUserRS.statusMessage = $"User details not fetched but deleted.";
                    }
                }
            }
            catch (Exception ex)
            {
                oDeleteUserRS.status = "Failed";
                oDeleteUserRS.statusCode = 0;
                oDeleteUserRS.statusMessage = $"Unable to delete the user, exception: {ex.Message}";
            }

            return oDeleteUserRS;
        }

    }
}
