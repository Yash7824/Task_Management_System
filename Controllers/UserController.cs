using Microsoft.AspNetCore.Mvc;
using Task_Management_System.BL;
using Task_Management_System.Models;
using Task_Management_System.Repositories;

namespace Task_Management_System.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : BaseController
    {
        private IUserRepository userRepository;
        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }
        [HttpPost]
        public async Task<ActionResult> CreateUser(User user)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            var response = await new UserBL().UserCreation(user, userRepository, client);
            return Ok(response);

        }

        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            var response = await new UserBL().GetUsers(userRepository, client);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetUser([FromBody] GetUserRQ getUserRQ)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            var currentUserId = GetCurrentUser();
            var response = await userRepository.GetUserAsync(getUserRQ.userID);
            return Ok(response);
            
        }

        [HttpGet]
        public async Task<ActionResult> GetUserId([FromHeader] string DashboardToken)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            if (string.IsNullOrEmpty(DashboardToken))
                return Unauthorized(new { Message = "Token is missing." });

            var oValidateTokenRs = await new LoginBL().ValidateTokenAsync(DashboardToken);

            if(!oValidateTokenRs.IsValid)
                return Unauthorized(new {Message = oValidateTokenRs.errorMessage});

            return Ok(oValidateTokenRs);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromHeader] string DashboardToken, [FromBody] UpdateUserRQ user)
        {
            
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            if (string.IsNullOrEmpty(DashboardToken))
                return Unauthorized(new { Message = "Token is missing" });

            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);

            if(!oValidateTokenRS.IsValid)
                return Unauthorized(new {Message = oValidateTokenRS.errorMessage});

            var oUpdateUserRS = await new UserBL().UpdateUser(oValidateTokenRS.user_id, user, userRepository, client);
            return Ok(oUpdateUserRS);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteUser([FromHeader] string DashboardToken)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            if (string.IsNullOrEmpty(DashboardToken))
                return Unauthorized(new { Message = "Token is missing" });

            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);

            if(!oValidateTokenRS.IsValid)
                return Unauthorized(new {Message = oValidateTokenRS.errorMessage});

            var oDeleteUserRS = await new UserBL().DeleteUser(oValidateTokenRS.user_id, userRepository, client);
            return Ok(oDeleteUserRS);
        }

        [HttpPost]
        public IActionResult DecryptPassword([FromBody] DecryptPasswordRQ decryptPasswordRQ)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            var response = new UserBL().DecryptPassword(decryptPasswordRQ.password, client);
            return Ok(response);
        }


    }
}
