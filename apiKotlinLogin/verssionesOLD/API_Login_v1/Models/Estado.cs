using System;
using System.Collections.Generic;

namespace API_Login.Models;

public partial class Estado
{
    public int IdEstado { get; set; }

    public string NomEstado { get; set; } = null!;

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
