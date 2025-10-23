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

        // ✅ DTO para Cadastro
        public class UsuarioCadastroDto
        {
            public string Nome { get; set; }
            public string Email { get; set; }
            public string SenhaHash { get; set; }
            public string? Perfil { get; set; }
        }

        // ✅ DTO para Login
        public class LoginRequest
        {
            public string Email { get; set; }
            public string Senha { get; set; }
        }

        // ✅ DTO para resposta do login
        public class LoginResponse
        {
            public int UsuarioId { get; set; }
            public string Nome { get; set; }
            public string Token { get; set; }
        }

        // ✅ POST /api/Usuarios
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] UsuarioCadastroDto req)
        {
            if (req == null ||
                string.IsNullOrWhiteSpace(req.Email) ||
                string.IsNullOrWhiteSpace(req.SenhaHash) ||
                string.IsNullOrWhiteSpace(req.Nome))
            {
                return BadRequest("Informe nome, e-mail e senha.");
            }

            // Verifica se o e-mail já existe
            bool emailExiste = await _context.Usuarios.AnyAsync(u => u.Email == req.Email);
            if (emailExiste)
                return Conflict("E-mail já cadastrado.");

            // Cria o novo usuário
            var novoUsuario = new Usuario
            {
                Nome = req.Nome.Trim(),
                Email = req.Email.Trim(),
                SenhaHash = req.SenhaHash.Trim(),
                Perfil = string.IsNullOrEmpty(req.Perfil) ? "Usuario" : req.Perfil,
                Ativo = true,
                CriadoEm = DateTime.UtcNow
            };

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Usuário cadastrado com sucesso!",
                usuario = new
                {
                    novoUsuario.Id,
                    novoUsuario.Nome,
                    novoUsuario.Email,
                    novoUsuario.Perfil
                }
            });
        }

        // ✅ GET /api/Usuarios/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nome,
                    u.Email,
                    u.Perfil,
                    u.CriadoEm
                })
                .FirstOrDefaultAsync(u => u.Id == id);

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

        // ✅ PUT /api/Usuarios/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] UsuarioCadastroDto req)
        {
            if (req == null)
                return BadRequest("Dados inválidos.");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            // Atualiza os campos permitidos
            usuario.Nome = string.IsNullOrWhiteSpace(req.Nome) ? usuario.Nome : req.Nome.Trim();
            usuario.Email = string.IsNullOrWhiteSpace(req.Email) ? usuario.Email : req.Email.Trim();
            usuario.SenhaHash = string.IsNullOrWhiteSpace(req.SenhaHash) ? usuario.SenhaHash : req.SenhaHash.Trim();
            usuario.Perfil = string.IsNullOrWhiteSpace(req.Perfil) ? usuario.Perfil : req.Perfil;
            usuario.Ativo = true;
            usuario.AtualizadoEm = DateTime.UtcNow;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Usuário atualizado com sucesso!",
                usuario = new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    usuario.Perfil,
                    usuario.Ativo,
                    usuario.AtualizadoEm
                }
            });
        }
    }
}