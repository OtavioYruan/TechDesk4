using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeedbackController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public FeedbackController(TechDeskDbContext context)
        {
            _context = context;
        }

        // POST /api/Feedback/{chamadoId}
        [HttpPost("{chamadoId:int}")]
        public async Task<IActionResult> EnviarFeedback(int chamadoId, [FromBody] FeedbackAtendimento feedback)
        {
            if (feedback == null)
                return BadRequest("Dados inválidos.");

            if (feedback.Nota < 1 || feedback.Nota > 5)
                return BadRequest("A nota deve ser entre 1 e 5.");

            // Define os IDs e data
            feedback.IdChamado = chamadoId;
            feedback.Data = DateTime.UtcNow;

            _context.FeedbackAtendimentos.Add(feedback);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterFeedbackPorChamado), new { chamadoId = feedback.IdChamado }, feedback);
        }

        // GET /api/Feedback/{chamadoId}
        [HttpGet("{chamadoId:int}")]
        public async Task<IActionResult> ObterFeedbackPorChamado(int chamadoId)
        {
            var lista = await _context.FeedbackAtendimentos
                .Where(f => f.IdChamado == chamadoId)
                .Include(f => f.Usuario)
                .ToListAsync();

            if (lista == null || lista.Count == 0)
                return NotFound("Nenhum feedback encontrado para esse chamado.");

            return Ok(lista);
        }
    }
}
