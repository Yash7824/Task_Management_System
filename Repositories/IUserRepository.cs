using Task_Management_System.Models;

namespace Task_Management_System.Repositories
{
    public interface IUserRepository
    {
        Task<UserCreated> CreateUserAsync(User user);
        Task<GetUsers> GetUsersAsync();
        Task<GetUser> GetUserAsync(Guid guid);
        Task<GetUser> UpdateUserAsync(Guid userId, UpdateUserRQ user);
        Task<GetUser> DeleteUserAsync(Guid userId);
    }
}
