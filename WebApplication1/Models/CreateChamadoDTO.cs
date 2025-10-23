namespace TechDesk.Models
{
    public class CreateChamadoDTO
    {
        public string Titulo { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public string Prioridade { get; set; } = "Baixa";
        public int IdUsuario { get; set; }
        public int IdCategoria { get; set; }
        public int? IdTecnico { get; set; }
        public string Nivel { get; set; } = "N1";
    }
}

