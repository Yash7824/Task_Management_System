using Dapper;
using Newtonsoft.Json;
using Npgsql;
using Task_Managament_System.Models;
using Task_Managament_System.Services;
using Task_Management_System.Constants;
using Task_Management_System.Models;
using Task_Management_System.Repositories;
using Task_Management_System.Services;
using NpgsqlTypes;

namespace Task_Management_System.DL
{
    public class UserDL : IUserRepository
    {
        public async Task<UserCreated> CreateUserAsync(User user, string correlationID)
        {
            var oUserCreatedRs = new UserCreated();

            try
            {
                var encrypted_password = CommonMethod.EncryptAES(user.user_password);

                var oGetAllUsersRS = await GetUsersAsync(correlationID);
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

                    if (rowsAffected > 0)
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
                oUserCreatedRs.statusMessage = $"Exception Occurred in UserDL.CreateUserAsync(): {ex.Message}";
                await DBLogger.InsertLog("UserDL.CreateUserAsync()", ex.Message, ex.StackTrace, JsonConvert.SerializeObject(user), JsonConvert.SerializeObject(oUserCreatedRs), correlationID, user.user_id);
            }

            return oUserCreatedRs;
        }

        public async Task<GetUsers> GetUsersAsync(string correlationID)
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
                await DBLogger.InsertLog("UserDL.GetUsersAsync()", ex.Message, ex.StackTrace, "", JsonConvert.SerializeObject(oGetUsersRs), correlationID, "");
            }

            return oGetUsersRs;
        }

        public async Task<GetUser> GetUserAsync(string? userID, string correlationID)
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
                await DBLogger.InsertLog("UserDL.GetUserAsync()", ex.Message, ex.StackTrace, userID, JsonConvert.SerializeObject(oGetUserRs), correlationID, userID);
            }

            return oGetUserRs;
        }

        public async Task<UpdateUserRS> UpdateUserAsync(string? userId, UpdateUserRQ userRQ, string correlationID)
        {
            var oUpdateUserRS = new UpdateUserRS();
            try
            {
                using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                {
                    var encrypted_password = CommonMethod.EncryptAES(userRQ.user_password);
                    await dbConn.OpenAsync();

                    string query = $"UPDATE USERS SET USERNAME = @USERNAME, USER_EMAIL = @USER_EMAIL, USER_PASSWORD = @USER_PASSWORD, MOBILE_NUM = @MOBILE_NUM WHERE USER_ID = @USER_ID";

                    var parameters = new
                    {
                        USERNAME = userRQ.username,
                        USER_EMAIL = userRQ.user_email,
                        USER_PASSWORD = encrypted_password,
                        MOBILE_NUM = userRQ.mobile_num,
                        USER_ID = userId
                    };

                    int rowsAffected = await dbConn.ExecuteAsync(query, parameters);
                    if (rowsAffected > 0)
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
                await DBLogger.InsertLog("UserDL.UpdateUserAsync()", ex.Message, ex.StackTrace, JsonConvert.SerializeObject(userRQ), JsonConvert.SerializeObject(oUpdateUserRS), correlationID, "");
            }

            return oUpdateUserRS;
        }

        public async Task<DeleteUserRS> DeleteUserAsync(string? userId, string correlationID)
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
                await DBLogger.InsertLog("UserDL.DeleteUserAsync()", ex.Message, ex.StackTrace, userId, JsonConvert.SerializeObject(oDeleteUserRS), correlationID, userId);
            }

            return oDeleteUserRS;
        }

        public async Task<UserDumpRS> InsertUserAsync(List<UserDump> users, string correlationID)
        {
            var oUserDumpRs = new UserDumpRS();
            int rowsAffected = 0;
            try
            {
                foreach(var user in users)
                {
                    using (var dbConn = new NpgsqlConnection(TaskConstant.PostgresDbConn))
                    {
                        await dbConn.OpenAsync();
                        string query = "INSERT INTO USER_DUMP (ID, FIRST_NAME, LAST_NAME, AGE, GENDER, USERNAME, PASSWORD, BIRTH_DATE, IMAGE, ADDRESS, COMPANY, BANK) VALUES (:ID, :FIRST_NAME, :LAST_NAME, :AGE, :GENDER, :USERNAME, :PASSWORD, :BIRTH_DATE, :IMAGE, :ADDRESS, :COMPANY, :BANK)";

                        var parameters = new
                        {
                            ID = user.id,
                            FIRST_NAME = user.first_name,
                            LAST_NAME = user.last_name,
                            AGE = user.age,
                            GENDER = user.gender,
                            USERNAME = user.username,
                            PASSWORD = user.password,
                            BIRTH_DATE = user.birth_date,
                            IMAGE = user.image,
                            ADDRESS = JsonConvert.SerializeObject(user.address),
                            COMPANY = JsonConvert.SerializeObject(user.company),
                            BANK = JsonConvert.SerializeObject(user.bank)
                        };

                        rowsAffected += await dbConn.ExecuteAsync(query, parameters);
                    }
                }

                if (rowsAffected > 0)
                {
                    oUserDumpRs.status = "Success";
                    oUserDumpRs.statusCode = 0;
                    oUserDumpRs.statusMessage = $"Users Inserted Successfully and Total Users: {rowsAffected}";
                }
                else
                {
                    oUserDumpRs.status = "Failed";
                    oUserDumpRs.statusCode = 1;
                    oUserDumpRs.statusMessage = "Unable To Insert User Dump!";
                }

            }
            catch (Exception ex)
            {
                oUserDumpRs.status = "Failed";
                oUserDumpRs.statusCode = 2;
                oUserDumpRs.statusMessage = $"Exception occurred in UserDL.InsertUserAsync(): {ex.Message}";
                await DBLogger.InsertLog("UserDL.InsertUserAsync()", ex.Message, ex.StackTrace, JsonConvert.SerializeObject(users), JsonConvert.SerializeObject(oUserDumpRs), correlationID, "");
            }

            return oUserDumpRs;
        }
    }
}
