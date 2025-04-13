using Dapper;
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
                var encrypted_password = CommonMethod.EncryptAES(user.user_password);

                var oGetAllUsersRS = await GetUsersAsync();
                var userEmailPresent = oGetAllUsersRS.Users.Any(x => x.user_email == user.user_email);

                if (userEmailPresent)
                {
                    oUserCreatedRs.status = "Failed";
                    oUserCreatedRs.statusCode = 1;
                    oUserCreatedRs.statusMessage = "User Email Already Exists";
                    return oUserCreatedRs;
                };


                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {

                    await dbConn.OpenAsync();

                    var query = $"INSERT INTO USERS (USERNAME, USER_EMAIL, USER_PASSWORD, MOBILE_NUM, CREATED_AT) VALUES (:USERNAME, :USER_EMAIL, :USER_PASSWORD, :MOBILE_NUM, :CREATED_AT)";

                    var parameters = new
                    {
                        USERNAME = user.username,
                        USER_EMAIL = user.user_email,
                        USER_PASSWORD = encrypted_password,
                        MOBILE_NUM = user.mobile_num,
                        CREATED_AT = DateTime.UtcNow,
                    };

                    int rowsAffected = await dbConn.ExecuteAsync(query, parameters);

                    if(rowsAffected > 0)
                    {
                        oUserCreatedRs.status = "Success";
                        oUserCreatedRs.statusCode = 0;
                        oUserCreatedRs.statusMessage = "User Created Successfully";
                        user.user_password = encrypted_password;
                        oUserCreatedRs.userDetails = user;
                    }
                    else
                    {
                        oUserCreatedRs.status = "Failed";
                        oUserCreatedRs.statusCode = 1;
                        oUserCreatedRs.statusMessage = "User Not Inserted";
                        user.user_password = encrypted_password;
                        oUserCreatedRs.userDetails = user;
                    }
                };
            }
            catch (Exception ex)
            {
                oUserCreatedRs.status = "Failed";
                oUserCreatedRs.statusCode = 2;
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
                    var query = $"SELECT * FROM USERS";
                    var result = (await dbConn.QueryAsync<User>(query)).ToList();

                    oGetUsersRs.status = "Success";
                    oGetUsersRs.statusCode = 0;
                    oGetUsersRs.statusMessage = $"Total Number of Users: {result.Count}";
                    oGetUsersRs.Users = result;
                }
            }
            catch (Exception ex)
            {
                oGetUsersRs.status = "Failed";
                oGetUsersRs.statusCode = 2;
                oGetUsersRs.statusMessage = $"Exception Occurred in UserDL.GetUsersAsync(): {ex.Message}";
            }

            return oGetUsersRs;
        }

        public async Task<GetUser> GetUserAsync(string? userID)
        {
            var oGetUserRs = new GetUser();

            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = $"SELECT * FROM USERS WHERE USER_ID = :userID LIMIT 1";
                    var parameter = new { userID };
                    var result = await dbConn.QueryFirstOrDefaultAsync<User>(query, parameter);

                    oGetUserRs.status = "Success";
                    oGetUserRs.statusCode = 0;

                    if (result != null)
                    {
                        oGetUserRs.statusMessage = "User Fetched Successfully";
                        oGetUserRs.User = result;
                    }
                    else
                    {
                        oGetUserRs.statusMessage = "Unable To Fetch the User!";
                        oGetUserRs.User = null;
                    }
                }

            }
            catch (Exception ex)
            {
                oGetUserRs.status = "Failed";
                oGetUserRs.statusCode = 2;
                oGetUserRs.statusMessage = $"Exception occurred in UserDL.GetUserAsync(): {ex.Message}";
            }

            return oGetUserRs;
        }

        public async Task<UpdateUserRS> UpdateUserAsync(string? userId, UpdateUserRQ userRQ)
        {
            var oUpdateUserRS = new UpdateUserRS();
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    var encrypted_password = CommonMethod.EncryptAES(userRQ.user_password);
                    await dbConn.OpenAsync();

                    string query = $"UPDATE USERS SET USERNAME = @USERNAME, USER_EMAIL = @USER_EMAIL, USER_PASSWORD = @USER_PASSWORD, MOBILE_NUM = @MOBILE_NUM, TASKS_ASSIGNED = @TASKS_ASSIGNED WHERE USER_ID = @USER_ID";

                    var parameters = new
                    {
                        USERNAME = userRQ.username,
                        USER_EMAIL = userRQ.user_email,
                        USER_PASSWORD = encrypted_password,
                        MOBILE_NUM = userRQ.mobile_num,
                        TASKS_ASSIGNED = userRQ.tasks_assigned,
                        USER_ID = userId
                    };

                    int rowsAffected = await dbConn.ExecuteAsync(query, parameters);
                    if(rowsAffected > 0)
                    {
                        oUpdateUserRS.status = "Success";
                        oUpdateUserRS.statusCode = 0;
                        oUpdateUserRS.statusMessage = "User Updated Successfully";
                    }
                    else
                    {
                        oUpdateUserRS.status = "Failed";
                        oUpdateUserRS.statusCode = 1;
                        oUpdateUserRS.statusMessage = "Unable to Update User!";
                    }
                }
            }
            catch (Exception ex)
            {
                oUpdateUserRS.status = "Failed";
                oUpdateUserRS.statusCode = 2;
                oUpdateUserRS.statusMessage = $"Exception occurred in UserDL.UpdateUserAsync(): {ex.Message}";
            }

            return oUpdateUserRS;
        }

        public async Task<DeleteUserRS> DeleteUserAsync(string? userId)
        {
            var oDeleteUserRS = new DeleteUserRS();
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    await dbConn.OpenAsync();
                    string query = "DELETE FROM USERS WHERE USER_ID = @userId";
                    var parameters = new { userId };
                    int rowsAffected = await dbConn.ExecuteAsync(query, parameters);

                    if (rowsAffected > 0)
                    {
                        oDeleteUserRS.status = "Success";
                        oDeleteUserRS.statusCode = 0;
                        oDeleteUserRS.statusMessage = $"User Deleted Successfully!";
                    }
                    else
                    {
                        oDeleteUserRS.status = "Failed";
                        oDeleteUserRS.statusCode = 1;
                        oDeleteUserRS.statusMessage = $"Unable To Delete User!";
                    }
                }
            }
            catch (Exception ex)
            {
                oDeleteUserRS.status = "Failed";
                oDeleteUserRS.statusCode = 2;
                oDeleteUserRS.statusMessage = $"Exception occurred in UserDL.DeleteUserAsync(): {ex.Message}";
            }

            return oDeleteUserRS;
        }

    }
}
