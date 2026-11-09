using APILogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Consultas.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductoController : ControllerBase
    {
        [HttpGet]
        [Route("ListadoPersonas")]
        public IActionResult ListadoPersonas()
        {
            List<Persona> listado = new List<Persona>();
            try
            {
                using (var db = new ArchivoContext())
                {
                    var consulta = db.Personas.ToList();

                    foreach (var item in consulta)
                    {
                        listado.Add(new Persona()
                        {
                            IdPersona = item.IdPersona,
                            NomPersona = item.NomPersona,
                            ApePersona = item.ApePersona,
                            Edad = item.Edad
                        });
                    }
                }
                return StatusCode(StatusCodes.Status200OK,
                    new
                    {
                        respuesta = "OK",
                        mensaje = "Cargado correctamente",
                        lista = listado
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        respuesta = "ERROR",
                        mensaje = ex.Message,
                        lista = listado
                    });
            }
        }

        [HttpPost]
        [Route("InsertarPersona")]
        public IActionResult InsertarPersona([FromBody] Persona persona)
        {
            try
            {
                using (var db = new ArchivoContext())
                {
                    Persona insertar = new Persona();
                    insertar.NomPersona = persona.NomPersona;
                    insertar.ApePersona = persona.ApePersona;
                    insertar.Edad = persona.Edad;

                    db.Personas.Add(insertar);
                    db.SaveChanges();
                }

                return StatusCode(StatusCodes.Status200OK,
                    new
                    {
                        respuesta = "OK",
                        mensaje = "Insertado correctamente"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        respuesta = "ERROR",
                        mensaje = ex.Message
                    });
            }
        }

        [HttpPost]
        [Route("EditarPersona")]
        public IActionResult EditarPersona([FromBody] Persona persona)
        {
            try
            {
                using (var db = new ArchivoContext())
                {
                    var contador = db.Personas.Where(x => x.IdPersona == persona.IdPersona);
                    if (!contador.Any())
                    {
                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               respuesta = "ERROR",
                               mensaje = "Persona no se encuentra"
                           });
                    }
                    else
                    {
                        var item = contador.First();
                        item.NomPersona = persona.NomPersona;
                        item.ApePersona = persona.ApePersona;
                        item.Edad = persona.Edad;

                        db.SaveChanges();
                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               respuesta = "OK",
                               mensaje = "Editado correctamente"
                           });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        respuesta = "ERROR",
                        mensaje = ex.Message
                    });
            }
        }

        [HttpDelete]
        [Route("EliminarPersona")]
        public IActionResult EliminarPersona([FromBody] Persona persona)
        {
            try
            {
                using (var db = new ArchivoContext())
                {
                    var contador = db.Personas.Where(x => x.IdPersona == persona.IdPersona);
                    if (!contador.Any())
                    {
                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               respuesta = "ERROR",
                               mensaje = "Persona no se encuentra"
                           });
                    }
                    else
                    {
                        db.Personas.Remove(contador.First());
                        db.SaveChanges();
                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               respuesta = "OK",
                               mensaje = "Persona eliminada correctamente"
                           });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        respuesta = "ERROR",
                        mensaje = ex.Message
                    });
            }
        }

        [HttpPost]
        [Route("InsertarUsuario")]
        public IActionResult InsertarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                using (var db = new ArchivoContext())
                {
                    var existeUsr = db.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    if (!existeUsr.Any())
                    {
                        var existePer = db.Personas.Where(x => x.IdPersona == usuario.IdPersona);

                        if (!existePer.Any())
                        {
                            return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                respuesta = "ERROR",
                                mensaje = "Persona no se encuentra en el sistema"
                            });
                        }
                        else
                        {
                            Usuario insertar = new Usuario();
                            insertar.NomUsuario = usuario.NomUsuario;
                            insertar.Clave = string.IsNullOrEmpty(usuario.Clave) ? "123456" : usuario.Clave;
                            insertar.IdPersona = usuario.IdPersona;
                            insertar.IdEstado = usuario.IdEstado;

                            db.Usuarios.Add(insertar);
                            db.SaveChanges();

                            return StatusCode(StatusCodes.Status200OK,
                               new
                               {
                                   respuesta = "OK",
                                   mensaje = "Usuario insertado correctamente"
                               });
                        }
                    }
                    else
                    {
                        return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            respuesta = "ERROR",
                            mensaje = "Usuario ya se encuentra en el sistema"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        respuesta = "ERROR",
                        mensaje = ex.Message
                    });
            }
        }

        // AGREGADO [2026-11-09 00:23]: Validación previa de Persona y Estado para prevenir violaciones FK y responder siempre en formato JSON limpio
        [HttpPost]
        [Route("EditarUsuario")]
        public IActionResult EditarUsuario([FromBody] Usuario usuario, [FromQuery] string? oldNomUsuario = null)
        {
            try
            {
                using (var db = new ArchivoContext())
                {
                    string targetName = !string.IsNullOrWhiteSpace(oldNomUsuario) ? oldNomUsuario : usuario.NomUsuario;
                    var usr = db.Usuarios.FirstOrDefault(x => x.NomUsuario == targetName);
                    if (usr == null)
                    {
                        usr = db.Usuarios.FirstOrDefault(x => x.NomUsuario == usuario.NomUsuario);
                    }

                    if (usr == null)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { respuesta = "ERROR", mensaje = "Usuario no encontrado" });
                    }

                    // Validar que la persona asociada exista en la BD
                    var existePer = db.Personas.Any(x => x.IdPersona == usuario.IdPersona);
                    if (!existePer)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { respuesta = "ERROR", mensaje = "La persona asociada (ID: " + usuario.IdPersona + ") no existe" });
                    }

                    // Validar que el estado exista en la BD
                    var existeEst = db.Estados.Any(x => x.IdEstado == usuario.IdEstado);
                    if (!existeEst)
                    {
                        usuario.IdEstado = 1; // Estado activo por defecto
                    }

                    string claveFinal = string.IsNullOrEmpty(usuario.Clave) || usuario.Clave == "123456" ? usr.Clave : usuario.Clave;

                    if (usr.NomUsuario != usuario.NomUsuario)
                    {
                        var existeNuevo = db.Usuarios.Any(x => x.NomUsuario == usuario.NomUsuario);
                        if (existeNuevo)
                        {
                            return StatusCode(StatusCodes.Status200OK, new { respuesta = "ERROR", mensaje = "El nuevo nombre de usuario ya existe" });
                        }

                        db.Database.ExecuteSqlRaw(
                            "UPDATE USUARIO SET NOM_USUARIO = {0}, CLAVE = {1}, ID_PERSONA = {2}, ID_ESTADO = {3} WHERE NOM_USUARIO = {4}",
                            usuario.NomUsuario, claveFinal, usuario.IdPersona, usuario.IdEstado, usr.NomUsuario);
                    }
                    else
                    {
                        usr.Clave = claveFinal;
                        usr.IdPersona = usuario.IdPersona;
                        usr.IdEstado = usuario.IdEstado;
                        db.SaveChanges();
                    }

                    return StatusCode(StatusCodes.Status200OK, new { respuesta = "OK", mensaje = "Usuario editado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new { respuesta = "ERROR", mensaje = "Error interno: " + ex.Message });
            }
        }

        [HttpPost]
        [Route("EliminarUsuario")]
        public IActionResult EliminarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                using (var db = new ArchivoContext())
                {
                    var usr = db.Usuarios.FirstOrDefault(x => x.NomUsuario == usuario.NomUsuario);
                    if (usr == null)
                    {
                        return StatusCode(StatusCodes.Status200OK, new { respuesta = "ERROR", mensaje = "Usuario no encontrado" });
                    }
                    db.Usuarios.Remove(usr);
                    db.SaveChanges();
                    return StatusCode(StatusCodes.Status200OK, new { respuesta = "OK", mensaje = "Usuario eliminado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status200OK, new {
                    respuesta = "ERROR",
                    mensaje = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("ListadoUsuario")]
        public IActionResult ListadoUsuarios()
        {
            try
            {
                List<UsuarioPersona> listado = new List<UsuarioPersona>();
                using (var db = new ArchivoContext())
                {
                    var query = from U in db.Usuarios
                                join P in db.Personas on U.IdPersona equals P.IdPersona
                                join E in db.Estados on U.IdEstado equals E.IdEstado
                                select new
                                {
                                    U.NomUsuario,
                                    P.NomPersona,
                                    P.ApePersona,
                                    P.Edad,
                                    E.NomEstado
                                };
                    foreach (var item in query)
                    {
                        listado.Add(new UsuarioPersona
                        {
                            NomUsuario = item.NomUsuario,
                            NombreApellido = item.NomPersona + " " + item.ApePersona,
                            Edad = item.Edad,
                            Nom_estado = item.NomEstado
                        });
                    }
                }
                return StatusCode(StatusCodes.Status200OK,
                   new
                   {
                       respuesta = "OK",
                       mensaje = "Cargado correctamente",
                       lista = listado
                   });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        respuesta = "ERROR",
                        mensaje = ex.Message
                    });
            }
        }

        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] Usuario usuario)
        {
            try
            {
                using (var db = new ArchivoContext())
                {
                    var usr = db.Usuarios.FirstOrDefault(x => x.NomUsuario == usuario.NomUsuario && x.Clave == usuario.Clave);
                    if (usr != null)
                    {
                        if (usr.IdEstado != 1)
                        {
                            return StatusCode(StatusCodes.Status200OK, new
                            {
                                respuesta = "ERROR",
                                mensaje = "El usuario se encuentra deshabilitado"
                            });
                        }

                        return StatusCode(StatusCodes.Status200OK, new
                        {
                            respuesta = "OK",
                            mensaje = "Inicio de sesión exitoso"
                        });
                    }

                    return StatusCode(StatusCodes.Status200OK, new
                    {
                        respuesta = "ERROR",
                        mensaje = "Usuario o contraseña incorrectos"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    respuesta = "ERROR",
                    mensaje = ex.Message
                });
            }
        }
    }
}
