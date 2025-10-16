using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechDesk.Data;
using TechDesk.Models;

namespace TechDesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly TechDeskDbContext _context;

        public UsuariosController(TechDeskDbContext context)
        {
            _context = context;
        }

        // ✅ POST /api/Usuarios
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Usuario req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.SenhaHash) || string.IsNullOrWhiteSpace(req.Nome))
                return BadRequest("Informe nome, e-mail e senha.");

            // Verifica se o e-mail já existe
            bool emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == req.Email);
            if (emailExiste)
                return Conflict("E-mail já cadastrado.");

            // Cria o novo usuário
            var novoUsuario = new Usuario
            {
                Nome = req.Nome.Trim(),
                Email = req.Email.Trim(),
                SenhaHash = req.SenhaHash, // ⚠️ Futuramente aplicar hash real
                Perfil = req.Perfil ?? "Usuario",
                Ativo = true,
                CriadoEm = DateTime.UtcNow
            };

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObterPorId), new { id = novoUsuario.Id }, new
            {
                novoUsuario.Id,
                novoUsuario.Nome,
                novoUsuario.Email
            });
        }

        // ✅ GET /api/Usuarios/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            return Ok(usuario);
        }

        // ✅ POST /api/Usuarios/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Email) || string.IsNullOrWhiteSpace(req.Senha))
                return BadRequest("E-mail e senha são obrigatórios.");

            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u =>
                    u.Email == req.Email.Trim() &&
                    u.SenhaHash == req.Senha.Trim());

            if (user == null)
                return Unauthorized("Credenciais inválidas.");

            var resp = new LoginResponse
            {
                UsuarioId = user.Id,
                Nome = user.Nome,
                Token = $"token-{user.Id}-{Guid.NewGuid()}"
            };

            return Ok(resp);
        }
    }
}
