using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Task_Management_System.Models;
using Task_Management_System.Repositories;

namespace Task_Management_System.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly ITokenRepository tokenRepository;
        private readonly IUserRepository userRepository;
        public AuthController(ITokenRepository tokenRepository, IUserRepository userRepository)
        {
            this.tokenRepository = tokenRepository;
            this.userRepository = userRepository;
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(Login login)
        {
            var client = new HttpClient();
            if (ModelState.IsValid)
            {
                var response = await tokenRepository.ValidateUser(login, userRepository, client);
                return Ok(response);
            }
            return BadRequest();
        }
    }
}
