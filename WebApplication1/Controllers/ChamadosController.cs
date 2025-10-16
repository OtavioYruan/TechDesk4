using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChamadosController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public ChamadosController(TechDeskDbContext context)
        {
            _context = context;
        }

        // GET: api/Chamados
        [HttpGet]
        public async Task<IActionResult> GetChamados()
        {
            var chamados = await _context.Chamados
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.IdTecnicoNavigation)
                .ToListAsync();

            return Ok(chamados);
        }

        // GET: api/Chamados/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChamadoPorId(int id)
        {
            var chamado = await _context.Chamados
                .Include(c => c.IdCategoriaNavigation)
                .Include(c => c.IdUsuarioNavigation)
                .Include(c => c.IdTecnicoNavigation)
                .FirstOrDefaultAsync(c => c.IdChamado == id);

            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            return Ok(chamado);
        }

        // POST: api/Chamados
        [HttpPost]
        public async Task<IActionResult> CriarChamado([FromBody] Chamado chamado)
        {
            if (chamado == null)
                return BadRequest("Dados inválidos.");

            chamado.DataInicio = DateTime.UtcNow;
            chamado.Status = "Aberto";

            _context.Chamados.Add(chamado);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChamadoPorId), new { id = chamado.IdChamado }, chamado);
        }

        // PUT: api/Chamados/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarChamado(int id, [FromBody] Chamado chamadoAtualizado)
        {
            var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.IdChamado == id);
            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            chamado.Titulo = chamadoAtualizado.Titulo;
            chamado.Descricao = chamadoAtualizado.Descricao;
            chamado.Status = chamadoAtualizado.Status;
            chamado.Prioridade = chamadoAtualizado.Prioridade;
            chamado.IdTecnico = chamadoAtualizado.IdTecnico;
            chamado.IdCategoria = chamadoAtualizado.IdCategoria;
            chamado.Nivel = chamadoAtualizado.Nivel;
            chamado.DataFinal = chamadoAtualizado.DataFinal;

            await _context.SaveChangesAsync();
            return Ok(chamado);
        }

        // DELETE: api/Chamados/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletarChamado(int id)
        {
            var chamado = await _context.Chamados.FirstOrDefaultAsync(c => c.IdChamado == id);
            if (chamado == null)
                return NotFound("Chamado não encontrado.");

            _context.Chamados.Remove(chamado);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // GET: api/Chamados/{id}/historico
        [HttpGet("{id}/historico")]
        public async Task<IActionResult> GetHistoricoPorChamado(int id)
        {
            var historico = await _context.HistoricoChamados
                .Where(h => h.IdChamado == id)
                .OrderByDescending(h => h.Data)
                .ToListAsync();

            return Ok(historico);
        }
    }
}
