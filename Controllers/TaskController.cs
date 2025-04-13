using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Net.Http;
using Task_Managament_System.BL;
using Task_Managament_System.Models;
using Task_Managament_System.Repositories;
using Task_Management_System.BL;

namespace Task_Management_System.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TaskController : BaseController
    {
        private readonly ITaskRepository taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            this.taskRepository = taskRepository;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllTasks([FromHeader] string DashboardToken)
        {
            var client = new HttpClient();

            if(!ModelState.IsValid) return BadRequest();

            if (string.IsNullOrEmpty(DashboardToken))
                return Unauthorized(new { Message = "Token is missing." });

            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);
            
            if(!oValidateTokenRS.IsValid)
                return Unauthorized(new {Message = oValidateTokenRS.errorMessage});

            var response = await new TaskBL().GetAllTasksAsync(oValidateTokenRS.user_id, taskRepository, client);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult> GetTask([FromHeader] string DashboardToken, [FromBody] GetTaskRQ getTaskRQ)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();

            if (string.IsNullOrEmpty(DashboardToken))
                return Unauthorized(new { Message = "Token is missing." });

            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);

            if (!oValidateTokenRS.IsValid)
                return Unauthorized(new { Message = oValidateTokenRS.errorMessage });

            var response = await new TaskBL().GetTaskAsync(oValidateTokenRS.user_id, getTaskRQ, taskRepository, client);
            return Ok(response);

        }

        [HttpPost]
        public async Task<ActionResult> CreateTask([FromHeader] string DashboardToken, [FromBody] TaskModel task)
        {
            var client = new HttpClient();

            if (!ModelState.IsValid) return BadRequest();
            if (string.IsNullOrEmpty(DashboardToken))
                return Unauthorized(new { Message = "Token is missing." });

            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);

            if (!oValidateTokenRS.IsValid)
                return Unauthorized(new { Message = oValidateTokenRS.errorMessage });

            var response = await new TaskBL().CreateTaskAsync(oValidateTokenRS.user_id, task, taskRepository, client);
            return Ok(response);

        }

        [HttpPut]
        public async Task<ActionResult> UpdateTask([FromHeader] string DashboardToken, [FromBody] UpdateTaskRQ updateTaskRQ)
        {
            var client = new HttpClient();
            if (!ModelState.IsValid) return BadRequest();
            if (string.IsNullOrEmpty(DashboardToken))
                return Unauthorized(new { Message = "Token is missing." });

            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);

            if (!oValidateTokenRS.IsValid)
                return Unauthorized(new { Message = oValidateTokenRS.errorMessage });

            var response = await new TaskBL().UpdateTaskAsync(oValidateTokenRS.user_id, updateTaskRQ, taskRepository, client);
            return Ok(response);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteTask([FromHeader] string DashboardToken, [FromBody] DeleteTaskRQ deleteTaskRQ)
        {
            var client = new HttpClient();
            if(!ModelState.IsValid) return BadRequest();
            if (string.IsNullOrEmpty(DashboardToken))
                return Unauthorized(new { Message = "Token is missing." });

            var oValidateTokenRS = await new LoginBL().ValidateTokenAsync(DashboardToken);

            if (!oValidateTokenRS.IsValid)
                return Unauthorized(new { Message = oValidateTokenRS.errorMessage });

            var response = await new TaskBL().DeleteTaskAsync(oValidateTokenRS.user_id, deleteTaskRQ, taskRepository, client);
            return Ok(response);
        }

    }

}

