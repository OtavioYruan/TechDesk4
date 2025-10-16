using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class Chamado
{
    public int IdChamado { get; set; }

    public string Titulo { get; set; } = null!;

    public string Descricao { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string Prioridade { get; set; } = null!;

    public DateTime DataInicio { get; set; }

    public DateTime? DataFinal { get; set; }

    public int IdUsuario { get; set; }

    public int? IdTecnico { get; set; }

    public int IdCategoria { get; set; }

    public string? Nivel { get; set; }

    public virtual ICollection<FeedbackAtendimento> FeedbackAtendimentos { get; set; } = new List<FeedbackAtendimento>();

    public virtual ICollection<HistoricoChamado> HistoricoChamados { get; set; } = new List<HistoricoChamado>();

    public virtual Categoria IdCategoriaNavigation { get; set; } = null!;

    public virtual Tecnico? IdTecnicoNavigation { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<SolucoesSugerida> SolucoesSugerida { get; set; } = new List<SolucoesSugerida>();
}
