using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SolicitudGastoApp.Application.DTOs;
using SolicitudGastoApp.Application.Services;
using System.Security.Claims;

namespace SolicitudGastoApp.Web.Controllers.Api
{
    [ApiController]
    [Route("api/expenses")]
    //[Authorize] // ya estás protegido con Identity + 2FA
    public class ExpenseRequestsController : ControllerBase
    {
        private readonly IExpenseRequestService _service;

        public ExpenseRequestsController(IExpenseRequestService service)
        {
            _service = service;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [AllowAnonymous]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("pong");
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateExpenseRequestDto dto)
        {
            var result = await _service.CreateAsync(UserId, dto);
            return Ok(result);
        }

        [HttpPost("{id:guid}/submit")]
        public async Task<IActionResult> Submit(Guid id)
        {
            var result = await _service.SubmitAsync(UserId, id);
            return Ok(result);
        }

        [HttpPost("{id:guid}/decide")]
        public async Task<IActionResult> Decide(Guid id, DecideExpenseDto dto)
        {
            var result = await _service.DecideAsync(UserId, id, dto);
            return Ok(result);
        }

        [HttpGet("pending")]
        public async Task<IActionResult> PendingForApprover()
        {
            var result = await _service.GetPendingForApproverAsync(UserId);
            return Ok(result);
        }
    }
}
