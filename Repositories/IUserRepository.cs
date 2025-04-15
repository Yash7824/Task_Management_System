using Task_Management_System.Models;

namespace Task_Management_System.Repositories
{
    public interface IUserRepository
    {
        Task<UserCreated> CreateUserAsync(User user, string correlationID);
        Task<GetUsers> GetUsersAsync(string correlationID);
        Task<GetUser> GetUserAsync(string? user_id, string correlationID);
        Task<UpdateUserRS> UpdateUserAsync(string? userId, UpdateUserRQ user, string correlationID);
        Task<DeleteUserRS> DeleteUserAsync(string? userId, string correlationID);
    }
}
