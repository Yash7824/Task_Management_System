using Task_Management_System.Models;

namespace Task_Management_System.Repositories
{
    public interface IUserRepository
    {
        Task<UserCreated> CreateUserAsync(User user);
        Task<GetUsers> GetUsersAsync();
        Task<GetUser> GetUserAsync(string? user_id);
        Task<UpdateUserRS> UpdateUserAsync(string? userId, UpdateUserRQ user);
        Task<DeleteUserRS> DeleteUserAsync(string? userId);
    }
}
