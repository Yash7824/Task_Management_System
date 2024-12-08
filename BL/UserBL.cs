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
                oUserCreatedRs.statusCode = 0;
                oUserCreatedRs.statusMessage = ex.Message;
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
                oGetUsersRs.statusCode = 0;
                oGetUsersRs.statusMessage = ex.Message;
            }

            return oGetUsersRs;
        }

        public async Task<GetUser> GetUser(IUserRepository userRepository, Guid userID, HttpClient client)
        {
            var oGetUserRs = new GetUser();
            try
            {
                oGetUserRs = await userRepository.GetUserAsync(userID);
            }
            catch (Exception ex)
            {
                oGetUserRs.status = "Failed";
                oGetUserRs.statusCode = 0;
                oGetUserRs.statusMessage = ex.Message;
            }

            return oGetUserRs;
        }

        public async Task<GetUser> UpdateUser(Guid userId, UpdateUserRQ user, IUserRepository userRepository, HttpClient client)
        {
            var oUpdateUserRs = new GetUser();
            try
            {
                oUpdateUserRs = await userRepository.UpdateUserAsync(userId, user);
            }
            catch(Exception ex)
            {
                oUpdateUserRs.status = "Failed";
                oUpdateUserRs.statusCode = 0;
                oUpdateUserRs.statusMessage = ex.Message;
            }

            return oUpdateUserRs;
        }

        public async Task<GetUser> DeleteUser(Guid userId, IUserRepository userRepository, HttpClient client)
        {
            var oDeleteUserRs = new GetUser();
            try
            {
                oDeleteUserRs = await userRepository.DeleteUserAsync(userId);
            }
            catch (Exception ex)
            {
                oDeleteUserRs.status = "Failed";
                oDeleteUserRs.statusCode = 0;
                oDeleteUserRs.statusMessage = ex.Message;
            }

            return oDeleteUserRs;
        }

        public DecryptPasswordRS DecryptPassword(string password, HttpClient client)
        {
            var odecryptedPasswordRS = new DecryptPasswordRS();
            var decrypted_password = CommonMethod.DecryptAES(password);

            if (decrypted_password != null)
            {
                odecryptedPasswordRS.status = "Success";
                odecryptedPasswordRS.encrypted_password = password;
                odecryptedPasswordRS.decrypyted_password = decrypted_password;
            }

            return odecryptedPasswordRS;
        }
    }
}
