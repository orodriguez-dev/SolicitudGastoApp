using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SolicitudGastoApp.Infrastructure.Persistence;
using SolicitudGastoApp.Domain.Entities;

namespace SolicitudGastoApp.Web.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolicitudesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SolicitudesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var solicitudes = await _context.Solicitudes.ToListAsync();
            return Ok(solicitudes);
        }
    }
}
