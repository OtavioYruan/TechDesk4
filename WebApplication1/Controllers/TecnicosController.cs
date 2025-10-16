using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TecnicosController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public TecnicosController(TechDeskDbContext context)
        {
            _context = context;
        }

        // ✅ GET /api/Tecnicos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tecnico>>> GetAll()
        {
            var tecnicos = await _context.Tecnicos.ToListAsync();
            return Ok(tecnicos);
        }

        // ✅ GET /api/Tecnicos/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Tecnico>> GetById(int id)
        {
            var tecnico = await _context.Tecnicos.FindAsync(id);
            if (tecnico == null)
                return NotFound(new { mensagem = "Técnico não encontrado" });

            return Ok(tecnico);
        }

        // ✅ POST /api/Tecnicos
        [HttpPost]
        public async Task<ActionResult<Tecnico>> Create([FromBody] Tecnico novoTecnico)
        {
            if (novoTecnico == null)
                return BadRequest("Dados inválidos.");

            novoTecnico.CriadoEm = DateTime.UtcNow;
            novoTecnico.Ativo = true;
            novoTecnico.Perfil ??= "Técnico";

            _context.Tecnicos.Add(novoTecnico);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = novoTecnico.Id }, novoTecnico);
        }

        // ✅ PUT /api/Tecnicos/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Tecnico>> Update(int id, [FromBody] Tecnico tecnicoAtualizado)
        {
            var tecnico = await _context.Tecnicos.FindAsync(id);
            if (tecnico == null)
                return NotFound(new { mensagem = "Técnico não encontrado" });

            tecnico.Nome = tecnicoAtualizado.Nome;
            tecnico.Email = tecnicoAtualizado.Email;
            tecnico.Especialidade = tecnicoAtualizado.Especialidade;
            tecnico.Nivel = tecnicoAtualizado.Nivel;
            tecnico.Ativo = tecnicoAtualizado.Ativo;
            tecnico.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(tecnico);
        }

        // ✅ DELETE /api/Tecnicos/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var tecnico = await _context.Tecnicos.FindAsync(id);
            if (tecnico == null)
                return NotFound(new { mensagem = "Técnico não encontrado" });

            _context.Tecnicos.Remove(tecnico);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
