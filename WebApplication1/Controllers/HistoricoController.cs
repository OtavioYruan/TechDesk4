using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HistoricoController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public HistoricoController(TechDeskDbContext context)
        {
            _context = context;
        }

        // POST - registrar novo histórico
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] HistoricoChamado historico)
        {
            if (historico == null || string.IsNullOrWhiteSpace(historico.Mensagem))
                return BadRequest("Dados inválidos. A mensagem é obrigatória.");

            historico.Data = DateTime.UtcNow;

            _context.HistoricoChamados.Add(historico);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListarPorChamado), new { chamadoId = historico.IdChamado }, historico);
        }

        // GET - listar históricos de um chamado
        [HttpGet("{chamadoId:int}")]
        public async Task<IActionResult> ListarPorChamado(int chamadoId)
        {
            var historicos = await _context.HistoricoChamados
                .Where(h => h.IdChamado == chamadoId)
                .OrderByDescending(h => h.Data)
                .Include(h => h.AutorUsuario)
                .Include(h => h.AutorTecnico)
                .ToListAsync();

            if (historicos == null || historicos.Count == 0)
                return NotFound("Nenhum histórico encontrado para este chamado.");

            return Ok(historicos);
        }
    }
}
