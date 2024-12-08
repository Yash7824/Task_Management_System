using Task_Management_System.Models;

namespace Task_Management_System.Repositories
{
    public interface ITokenRepository
    {
        Task<LoginRS> ValidateUser(Login login, IUserRepository userRepository, HttpClient client);
        string GenerateToken(User user);
        string? GenerateTokenPrev(User user);
        Task<ValidateTokenRS> ValidateTokenAsync(string token);

    }
}
