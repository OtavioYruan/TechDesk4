using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class Tecnico
{
    public int Id { get; set; }

    public string Nome { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string SenhaHash { get; set; } = null!;

    public string Perfil { get; set; } = null!;

    public string? Especialidade { get; set; }

    public string? Nivel { get; set; }

    public string? CodigoEmpresa { get; set; }

    public bool Ativo { get; set; }

    public DateTime CriadoEm { get; set; }

    public DateTime? AtualizadoEm { get; set; }

    public virtual ICollection<Chamado> Chamados { get; set; } = new List<Chamado>();

    public virtual ICollection<HistoricoChamado> HistoricoChamados { get; set; } = new List<HistoricoChamado>();

    public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();
}
