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

            try
            {
                oUserCreatedRs = await userRepository.CreateUserAsync(user);
            }
            catch (Exception ex)
            {
                oUserCreatedRs.status = "Failed";
                oUserCreatedRs.statusCode = 2;
                oUserCreatedRs.statusMessage = $"Exception occurred in UserBL.UserCreation(): {ex.Message}";
            }

            return oUserCreatedRs;

        }

        public async Task<GetUsers> GetUsers(IUserRepository userRepository, HttpClient client)
        {
            var oGetUsersRs = new GetUsers();
            try
            {
                oGetUsersRs = await userRepository.GetUsersAsync();
            }
            catch (Exception ex)
            {
                oGetUsersRs.status = "Failed";
                oGetUsersRs.statusCode = 2;
                oGetUsersRs.statusMessage = $"Exception Occurred in UserBL.GetUsers(): {ex.Message}";
            }

            return oGetUsersRs;
        }

        public async Task<GetUser> GetUser(IUserRepository userRepository, string userID, HttpClient client)
        {
            var oGetUserRs = new GetUser();
            try
            {
                oGetUserRs = await userRepository.GetUserAsync(userID);
            }
            catch (Exception ex)
            {
                oGetUserRs.status = "Failed";
                oGetUserRs.statusCode = 2;
                oGetUserRs.statusMessage = $"Exception Occurred in UserBL.GetUser(): {ex.Message}";
            }

            return oGetUserRs;
        }

        public async Task<UpdateUserRS> UpdateUser(string? userId, UpdateUserRQ user, IUserRepository userRepository, HttpClient client)
        {
            var oUpdateUserRs = new UpdateUserRS();
            try
            {
                oUpdateUserRs = await userRepository.UpdateUserAsync(userId, user);
            }
            catch(Exception ex)
            {
                oUpdateUserRs.status = "Failed";
                oUpdateUserRs.statusCode = 2;
                oUpdateUserRs.statusMessage = $"Exception Occurred in UserBL.UpdateUser(): {ex.Message}";
            }

            return oUpdateUserRs;
        }

        public async Task<DeleteUserRS> DeleteUser(string? userId, IUserRepository userRepository, HttpClient client)
        {
            var oDeleteUserRs = new DeleteUserRS();
            try
            {
                oDeleteUserRs = await userRepository.DeleteUserAsync(userId);
            }
            catch (Exception ex)
            {
                oDeleteUserRs.status = "Failed";
                oDeleteUserRs.statusCode = 2;
                oDeleteUserRs.statusMessage = $"Exception Occurred in UserBL.DeleteUser(): {ex.Message}";
            }

            return oDeleteUserRs;
        }

        public DecryptPasswordRS DecryptPassword(string password, HttpClient client)
        {
            var odecryptedPasswordRS = new DecryptPasswordRS();
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
            }
            
            return odecryptedPasswordRS;
        }
    }
}
