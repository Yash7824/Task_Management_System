using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task_Management_System.BL;
using Task_Management_System.Models;
using Task_Management_System.Repositories;
using Task_Management_System.Services;

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
            var response = await new UserBL().GetUser(userRepository, getUserRQ.userID, client);
            return Ok(response);
            
        }

        [HttpGet]
        public async Task<ActionResult> GetUserId([FromHeader] string DashboardToken)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            var oValidateTokenRs = await new LoginBL().ValidateTokenAsync(DashboardToken);
            if(oValidateTokenRs.statusCode != 0) return Unauthorized(oValidateTokenRs);
            return Ok(oValidateTokenRs);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser([FromHeader] string DashboardToken, [FromBody] UpdateUserRQ user)
        {
            
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);
            if (oValidateTokenRS.statusCode != 0) return Unauthorized(oValidateTokenRS);
            var oUpdateUserRS = await new UserBL().UpdateUser(oValidateTokenRS.user_id, user, userRepository, client);
            return Ok(oUpdateUserRS);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteUser([FromHeader] string DashboardToken)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);
            if (oValidateTokenRS.statusCode != 0) return Unauthorized(oValidateTokenRS);
            var oDeleteUserRS = await new UserBL().DeleteUser(oValidateTokenRS.user_id, userRepository, client);
            return Ok(oDeleteUserRS);
        }

        [HttpPost]
        public async Task<ActionResult> DecryptPassword([FromBody] DecryptPasswordRQ decryptPasswordRQ)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            var response = await new UserBL().DecryptPassword(decryptPasswordRQ.password, client);
            return Ok(response);
        }
    }
}
