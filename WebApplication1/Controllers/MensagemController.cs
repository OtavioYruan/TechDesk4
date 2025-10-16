using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MensagemController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public MensagemController(TechDeskDbContext context)
        {
            _context = context;
        }

        // POST /api/Mensagem/{chamadoId}
        [HttpPost("{chamadoId:int}")]
        public async Task<IActionResult> EnviarMensagem(int chamadoId, [FromBody] HistoricoChamado mensagem)
        {
            if (mensagem == null || string.IsNullOrWhiteSpace(mensagem.Mensagem))
                return BadRequest("A mensagem não pode estar vazia.");

            // Preenche dados obrigatórios
            mensagem.IdChamado = chamadoId;
            mensagem.Data = DateTime.UtcNow;
            mensagem.AutorTipo ??= "Usuario"; // padrão se não vier definido
            mensagem.Visibilidade ??= "Externo";

            _context.HistoricoChamados.Add(mensagem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ListarMensagensPorChamado), new { chamadoId = mensagem.IdChamado }, mensagem);
        }

        // PUT /api/Mensagem/{mensagemId}
        [HttpPut("{mensagemId:int}")]
        public async Task<IActionResult> EditarMensagem(int mensagemId, [FromBody] HistoricoChamado mensagemAtualizada)
        {
            var mensagem = await _context.HistoricoChamados.FindAsync(mensagemId);
            if (mensagem == null)
                return NotFound("Mensagem não encontrada.");

            if (!string.IsNullOrWhiteSpace(mensagemAtualizada.Mensagem))
                mensagem.Mensagem = mensagemAtualizada.Mensagem;

            await _context.SaveChangesAsync();
            return Ok(mensagem);
        }

        // GET /api/Mensagem/chamado/{chamadoId}
        [HttpGet("chamado/{chamadoId:int}")]
        public async Task<IActionResult> ListarMensagensPorChamado(int chamadoId)
        {
            var mensagensChamado = await _context.HistoricoChamados
                .Where(h => h.IdChamado == chamadoId)
                .Include(h => h.AutorUsuario)
                .Include(h => h.AutorTecnico)
                .OrderBy(h => h.Data)
                .ToListAsync();

            if (mensagensChamado == null || mensagensChamado.Count == 0)
                return NotFound("Nenhuma mensagem encontrada para este chamado.");

            return Ok(mensagensChamado);
        }
    }
}
