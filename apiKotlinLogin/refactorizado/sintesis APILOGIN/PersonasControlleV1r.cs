using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using API_Login.Models;

namespace API_Login.Controllers
{
    public class PersonasController : Controller
    {

        [HttpGet]
        [Route("ListadoPersonas")]
        public IActionResult ListadoPersonas()
        {
            List<Persona> lista = new List<Persona>();
            try
            {
                using (var bd = new ArchivoContext())
                {
                    var consulta = bd.Personas.ToList();

                    foreach (var item in consulta)
                    {
                        lista.Add(new Persona
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
                        mensaje = "OK",
                        respuesta = "Datos cargados correctamente",
                        listado = lista
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }

        [HttpPost]
        [Route("InsertarPersona")]
        public IActionResult InsertarPersona([FromBody] Persona per)
        {
            try
            {
                using (var bd = new ArchivoContext())
                {
                    Persona insertar = new Persona();
                    insertar.NomPersona = per.NomPersona;
                    insertar.ApePersona = per.ApePersona;
                    insertar.Edad = per.Edad;
                    //añadimos a la tabla
                    bd.Personas.Add(insertar);
                    //guardamos en la base de datos
                    bd.SaveChanges();
                }
                return StatusCode(StatusCodes.Status200OK,
                    new
                    {
                        mensaje = "OK",
                        respuesta = "Datos insertados correctamente"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }
        [HttpPost]
        [Route("EditarPersona")]
        public IActionResult EditarPersona([FromBody] Persona persona)
        {
            try
            {
                using (var bd = new ArchivoContext())
                {
                    var datos = bd.Personas.Where(x => x.IdPersona == persona.IdPersona);
                    if (datos.Count() == 0) //no existe la persona
                    {
                        return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Persona no se encuentra en el sistema"
                        });

                    }
                    else//si existe
                    {
                        datos.First().NomPersona = persona.NomPersona;
                        datos.First().ApePersona = persona.ApePersona;
                        datos.First().Edad = persona.Edad;

                        bd.SaveChanges();
                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               mensaje = "OK",
                               respuesta = "Persona editada correctamente"
                           });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }

        [HttpDelete]
        [Route("EliminarPersona")]
        public IActionResult EliminarPersona([FromBody] Persona persona)
        {
            try
            {
                using (var bd = new ArchivoContext())
                {
                    var datos = bd.Personas.Where(x => x.IdPersona == persona.IdPersona);
                    if (datos.Count() == 0)//no existe
                    {
                        return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Persona no se encuentra en el sistema"
                        });
                    }
                    else//si existe
                    {
                        bd.Personas.Remove(datos.First());
                        bd.SaveChanges();
                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               mensaje = "OK",
                               respuesta = "Persona eliminada correctamente"
                           });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }
        [HttpGet]
        [Route("ListadoUsuarios")]
        public IActionResult ListadoUsuarios()
        {
            try
            {
                using (var bd = new ArchivoContext())
                {
                    List<UsuarioPersona> listado = new List<UsuarioPersona>();
                    var query = from U in bd.Usuarios
                                join P in bd.Personas on U.IdPersona equals P.IdPersona
                                join E in bd.Estados on U.IdEstado equals E.IdEstado
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
                            Estado = item.NomEstado
                        });
                    }
                    return StatusCode(StatusCodes.Status200OK,
                       new
                       {
                           mensaje = "OK",
                           respuesta = "Datos cargados correctamente",
                           listado = listado
                       });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }
        [HttpPost]
        [Route("InsertarUsuarios")]
        public ActionResult InsertarUsuarios([FromBody] Usuario usuario)
        {
            try
            {
                using (var bd = new ArchivoContext())
                {
                    var dato = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    if (dato.Count() == 0)//insertar
                    {
                        var perExiste = bd.Personas.Where(x => x.IdPersona == usuario.IdPersona).Count();
                        if (perExiste == 0) //no existe la persona
                        {
                            return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "ERROR",
                                respuesta = "Persona no se encuentra en el sistema"
                            });
                        }
                        else//se puede insertar usuario
                        {
                            Usuario insertar = new Usuario();
                            insertar.NomUsuario = usuario.NomUsuario;
                            insertar.IdPersona = usuario.IdPersona;
                            insertar.IdEstado = usuario.IdEstado;

                            bd.Usuarios.Add(insertar);
                            bd.SaveChanges();

                            return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "OK",
                                respuesta = "Usuario insertado correctamente"
                            });
                        }
                    }
                    else//existe
                    {
                        return StatusCode(StatusCodes.Status200OK,
                              new {
                                mensaje = "ERROR",
                                respuesta = "Usuario ya se encuentra en el sistema"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }
        [HttpPost]
        [Route("LoginUsuarios")]
        public ActionResult LoginUsuarios([FromBody] Usuario usuario)
        {
            try
            {
                using (var bd = new ArchivoContext())
                {
                    var dato = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    if (dato.Count() == 0) //no existe
                    {
                        return StatusCode(StatusCodes.Status200OK,
                              new
                              {
                                  mensaje = "ERROR",
                                  respuesta = "Usuario no se encuentra en el sistema"
                              });
                    }
                    else
                    {
                        //para la contraseña
                        //if(dato.First().password == usuario.password)
                        if (dato.First().IdEstado == 1)
                        {
                            return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "OK",
                                respuesta = "Bienvenido: " + dato.First().NomUsuario
                            });
                        }
                        else
                        {
                            return StatusCode(StatusCodes.Status200OK,
                              new
                              {
                                  mensaje = "ERROR",
                                  respuesta = "Usuario se encuentra DESHABILITADO"
                              });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }
    }
}
