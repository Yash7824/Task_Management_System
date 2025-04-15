using Newtonsoft.Json;
using Task_Managament_System.Models;
using Task_Managament_System.Services;
using Task_Management_System.DL;
using Task_Management_System.Models;
using Task_Management_System.Repositories;
using Task_Management_System.Services;

namespace Task_Management_System.BL
{
    public class UserBL
    {
        public async Task<UserCreated> UserCreation(User user, IUserRepository userRepository, HttpClient client)
        {
            var oUserCreatedRs = new UserCreated();
            string correlationID = Guid.NewGuid().ToString();

            try
            {
                oUserCreatedRs = await userRepository.CreateUserAsync(user, correlationID);
            }
            catch (Exception ex)
            {
                oUserCreatedRs.status = "Failed";
                oUserCreatedRs.statusCode = 2;
                oUserCreatedRs.statusMessage = $"Exception occurred in UserBL.UserCreation(): {ex.Message}";
                await DBLogger.InsertLog("UserBL.UserCreation()", ex.Message, ex.StackTrace, JsonConvert.SerializeObject(user), JsonConvert.SerializeObject(oUserCreatedRs), correlationID, user.user_id);
            }

            return oUserCreatedRs;

        }

        public async Task<GetUsers> GetUsers(IUserRepository userRepository, HttpClient client)
        {
            var oGetUsersRs = new GetUsers();
            string correlationID = Guid.NewGuid().ToString();

            try
            {
                oGetUsersRs = await userRepository.GetUsersAsync(correlationID);
            }
            catch (Exception ex)
            {
                oGetUsersRs.status = "Failed";
                oGetUsersRs.statusCode = 2;
                oGetUsersRs.statusMessage = $"Exception Occurred in UserBL.GetUsers(): {ex.Message}";
                await DBLogger.InsertLog("UserBL.GetUsers()", ex.Message, ex.StackTrace, "", JsonConvert.SerializeObject(oGetUsersRs), correlationID, "");
            }

            return oGetUsersRs;
        }

        public async Task<GetUser> GetUser(IUserRepository userRepository, string? userID, HttpClient client)
        {
            var oGetUserRs = new GetUser();
            string correlationID = Guid.NewGuid().ToString();

            try
            {
                oGetUserRs = await userRepository.GetUserAsync(userID, correlationID);
            }
            catch (Exception ex)
            {
                oGetUserRs.status = "Failed";
                oGetUserRs.statusCode = 2;
                oGetUserRs.statusMessage = $"Exception Occurred in UserBL.GetUser(): {ex.Message}";
                await DBLogger.InsertLog("UserBL.GetUser()", ex.Message, ex.StackTrace, userID, JsonConvert.SerializeObject(oGetUserRs), correlationID, userID);
            }

            return oGetUserRs;
        }

        public async Task<UpdateUserRS> UpdateUser(string? userId, UpdateUserRQ user, IUserRepository userRepository, HttpClient client)
        {
            var oUpdateUserRs = new UpdateUserRS();
            string correlationID = Guid.NewGuid().ToString();

            try
            {
                oUpdateUserRs = await userRepository.UpdateUserAsync(userId, user, correlationID);
            }
            catch(Exception ex)
            {
                oUpdateUserRs.status = "Failed";
                oUpdateUserRs.statusCode = 2;
                oUpdateUserRs.statusMessage = $"Exception Occurred in UserBL.UpdateUser(): {ex.Message}";
                await DBLogger.InsertLog("UserBL.UpdateUser()", ex.Message, ex.StackTrace, JsonConvert.SerializeObject(user), JsonConvert.SerializeObject(oUpdateUserRs), correlationID, "");
            }
            return oUpdateUserRs;
        }

        public async Task<DeleteUserRS> DeleteUser(string? userId, IUserRepository userRepository, HttpClient client)
        {
            var oDeleteUserRs = new DeleteUserRS();
            string correlationID = Guid.NewGuid().ToString();

            try
            {
                oDeleteUserRs = await userRepository.DeleteUserAsync(userId, correlationID);
            }
            catch (Exception ex)
            {
                oDeleteUserRs.status = "Failed";
                oDeleteUserRs.statusCode = 2;
                oDeleteUserRs.statusMessage = $"Exception Occurred in UserBL.DeleteUser(): {ex.Message}";
                await DBLogger.InsertLog("UserBL.DeleteUser()", ex.Message, ex.StackTrace, userId, JsonConvert.SerializeObject(oDeleteUserRs), correlationID, userId);
            }

            return oDeleteUserRs;
        }

        public async Task<DecryptPasswordRS> DecryptPassword(string password, HttpClient client)
        {
            var odecryptedPasswordRS = new DecryptPasswordRS();
            string correlationID = Guid.NewGuid().ToString();

            try
            {
                var decrypted_password = CommonMethod.DecryptAES(password);
                if (decrypted_password != null)
                {
                    odecryptedPasswordRS.status = "Success";
                    odecryptedPasswordRS.statusCode = 0;
                    odecryptedPasswordRS.statusMessage = string.Empty;
                    odecryptedPasswordRS.encrypted_password = password;
                    odecryptedPasswordRS.decrypyted_password = decrypted_password;
                }
                else
                {
                    odecryptedPasswordRS.status = "Failed";
                    odecryptedPasswordRS.statusCode = 1;
                    odecryptedPasswordRS.statusMessage = "Unable To Decrypt the Password!";
                    odecryptedPasswordRS.encrypted_password = password;
                    odecryptedPasswordRS.decrypyted_password = string.Empty;
                }
            }
            catch(Exception ex)
            {
                odecryptedPasswordRS.status = "Failed";
                odecryptedPasswordRS.statusCode = 2;
                odecryptedPasswordRS.statusMessage = $"Exception Occurred in UserBL.DecryptPassword(): {ex.Message}";
                odecryptedPasswordRS.encrypted_password = password;
                await DBLogger.InsertLog("UserBL.DecryptPassword()", ex.Message, ex.StackTrace, password, JsonConvert.SerializeObject(odecryptedPasswordRS), correlationID, "");
            }
            
            return odecryptedPasswordRS;
        }
    }
}
