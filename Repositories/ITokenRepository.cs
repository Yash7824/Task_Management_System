using Task_Management_System.Models;

namespace Task_Management_System.Repositories
{
    public interface ITokenRepository
    {
        Task<LoginRS> ValidateUser(Login login, IUserRepository userRepository, HttpClient client);
        Task<string> GenerateToken(User user);
        Task<ValidateTokenRS> ValidateTokenAsync(string token);

    }
}
