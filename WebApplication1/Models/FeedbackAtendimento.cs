using System;
using System.Collections.Generic;

namespace TechDesk.Models;

public partial class FeedbackAtendimento
{
    public int Id { get; set; }

    public int IdChamado { get; set; }

    public int UsuarioId { get; set; }

    public byte Nota { get; set; }

    public string? Comentario { get; set; }

    public DateTime Data { get; set; }

    public virtual Chamado IdChamadoNavigation { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;
}
