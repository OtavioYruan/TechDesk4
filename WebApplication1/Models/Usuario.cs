using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string SenhaHash { get; set; } = null!;

    public string Perfil { get; set; } = null!;

    public bool Ativo { get; set; }

    public DateTime CriadoEm { get; set; }

    public DateTime? AtualizadoEm { get; set; }

    public virtual ICollection<Chamado> Chamados { get; set; } = new List<Chamado>();

    public virtual ICollection<FeedbackAtendimento> FeedbackAtendimentos { get; set; } = new List<FeedbackAtendimento>();

    public virtual ICollection<HistoricoChamado> HistoricoChamados { get; set; } = new List<HistoricoChamado>();

    public virtual PreferenciasNotificacao? PreferenciasNotificacao { get; set; }
}
