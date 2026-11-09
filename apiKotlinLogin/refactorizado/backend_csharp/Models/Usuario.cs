using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APILogin.Models;

public partial class Usuario
{
    public string NomUsuario { get; set; } = null!;

    /* CÓDIGO ANTERIOR:
       No tenía la propiedad Clave.
       EXPLICACIÓN: Se agrega la propiedad Clave para poder recibir y procesar las credenciales en la API.
    */
    public string Clave { get; set; } = null!;

    public int IdPersona { get; set; }

    public int IdEstado { get; set; }

    [JsonIgnore]
    public virtual Estado? IdEstadoNavigation { get; set; }

    [JsonIgnore]
    public virtual Persona? IdPersonaNavigation { get; set; }
}
