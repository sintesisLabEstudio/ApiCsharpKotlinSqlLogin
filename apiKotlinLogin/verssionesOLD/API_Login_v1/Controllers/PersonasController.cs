using API_Login.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using System.Text.Json.Serialization;

namespace API_Login.Controllers
{
    public class PersonasController : Controller
    {
        //LISTADO PERSONAS
        [HttpGet]
        [Route("ListadoPersonas")]
        public IActionResult ListadoPersonas()
        {
            List<Persona> lista = new List<Persona>();      //lista de personas
            try
            {
                using (var bd = new ArchivoContext())       //en carpeta models el archivo context de la BD    
                {
                    var consulta = bd.Personas.ToList();    //lista consulta de la tabla Persona
                    foreach (var item in consulta)          //recorre la lista
                    {
                        lista.Add(new Persona               //cada item es un registro fila
                        {
                            IdPersona = item.IdPersona,
                            NomPersona = item.NomPersona,
                            ApePersona = item.ApePersona,
                            Edad = item.Edad
                        });
                    }
                }
                //SI OK
                return StatusCode(StatusCodes.Status200OK,  //retorna mensaje, respuesta y listado
                    new
                    {
                        mensaje = "OK",
                        respuesta = "Datos cargados correctamente",
                        listado = lista                     //el listado es la lista
                    });
            }
            //SI ERR
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

        //INSERTAR PERSONA
        [HttpPost]
        [Route("InsertarPersona")]
        public IActionResult InsertarPersona([FromBody] Persona per)
        {
            try
            {
                //AGREGA REGISTRO NUEVO
                using (var bd = new ArchivoContext())           //instancia Archivo context
                {
                    Persona nuevaPersona = new Persona();       //instancia nuevaPersona es un registro
                    nuevaPersona.NomPersona = per.NomPersona;   //campo nombre
                    nuevaPersona.ApePersona = per.ApePersona;   //campo apellido
                    nuevaPersona.Edad = per.Edad;               //campo edad
                    bd.Personas.Add(nuevaPersona);              //AGREGA REGISTRO EN LA TABLA personas           
                    bd.SaveChanges();                           //guardamos en la base de datos
                }
                //RETORNA mensaje y respuesta ok
                return StatusCode(StatusCodes.Status200OK,      
                    new
                    {
                        mensaje = "OK",
                        respuesta = "Datos insertados correctamente"
                    });
            }
            //RETORNA ERROR mensaje y respuesta
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

        //EDITA PERSONA
        [HttpPost]
        [Route("EditarPersona")]
        public IActionResult EditarPersona([FromBody] Persona persona)
        {
            try
            {
                using (var bd = new ArchivoContext())
                {
                    //
                    var registro = bd.Personas.Where(x => x.IdPersona == persona.IdPersona);
                    //SI NO EXISTE persona
                    if (registro.Count() == 0) 
                    {
                        return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Persona no se encuentra en el sistema"
                        });

                    }
                    //SI EXISTE persona
                    else
                    {
                        //ENCUENTRA EL PRIMERO y lllena el regitro
                        registro.First().NomPersona = persona.NomPersona;
                        registro.First().ApePersona = persona.ApePersona;
                        registro.First().Edad = persona.Edad;
                        //GRABA
                        bd.SaveChanges();
                        //RETORNA mensaje y respuesta OK
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

        //ELIMINA PERSONA
        [HttpDelete]
        [Route("EliminarPersona")]
        public IActionResult EliminarPersona([FromBody] Persona persona)
        {
            try
            {
                using (var bd = new ArchivoContext())                                       //instancia Archivo context
                {
                    var registro = bd.Personas.Where(x => x.IdPersona == persona.IdPersona); //consulta por id a eliminar
                    //NO EXISTE
                    if (registro.Count() == 0)
                    {
                        //RETORNA ERROR con mensaje y respuesta Nok
                        return StatusCode(StatusCodes.Status200OK,                      //err 200 para que no salga del try
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Persona no se encuentra en el sistema"
                        });
                    }
                    //EXISTE
                    else
                    {
                        //ELIMINA
                        bd.Personas.Remove(registro.First());
                        bd.SaveChanges();
                        //RETORNA mensaje y respuesta OK
                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               mensaje = "OK",
                               respuesta = "Persona eliminada correctamente"
                           });
                    }
                }
            }
            //SI ERROR mensaje y respuesta
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

        //LISTA USUARIOS
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

        //INSERTA USUARIOS
        [HttpPost]
        [Route("InsertarUsuarios")]
        public ActionResult InsertarUsuarios([FromBody] Usuario usuario)
        {
            try
            {
                using (var bd = new ArchivoContext())
                {
                    //ENCUENTRA REGISTRO de usuario
                    var registroUsuario = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    if (registroUsuario.Count() == 0)
                    {
                        var personaExiste = bd.Personas.Where(x => x.IdPersona == usuario.IdPersona).Count(); //consulta si persona existe
                        //SI NO ExISTE la persona. El usuario debe estar en tabla personas
                        if (personaExiste == 0) //no existe la persona
                        {
                            //RETORNA mensaje y respuesta NOK
                            return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "ERROR",
                                respuesta = "Persona no se encuentra en el sistema"
                            });
                        }
                        //SI EXISTE persona
                        else//se puede insertar usuario
                        {
                            Usuario nuevoUsuario = new Usuario();           //instancia usuario nuevo
                            nuevoUsuario.NomUsuario = usuario.NomUsuario;   //nombre
                            nuevoUsuario.IdPersona = usuario.IdPersona;     //id persona es FK
                            nuevoUsuario.IdEstado = usuario.IdEstado;       //Id estado es FK
                            //AGREGA NUEVO USUARIO
                            bd.Usuarios.Add(nuevoUsuario);
                            bd.SaveChanges();
                            //RETORNA mensaje respuesta ok
                            return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "OK",
                                respuesta = "Usuario insertado correctamente"
                            });
                        }
                    }
                    //YA SE ENCUETRA REGISTRO usuario 
                    else
                    {
                        //RETORNA mnsaje respuesta
                        return StatusCode(StatusCodes.Status200OK,              //ERR 200 para no salir del bloque
                              new {
                                mensaje = "ERROR",
                                respuesta = "Usuario ya se encuentra en el sistema"
                        });
                    }
                }
            }
            //SI ERROR 
            catch (Exception ex)
            {
                //RETORNA mensaje rspuesta
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        respuesta = ex.Message
                    });
            }
        }

        //ELIMINA USUARIOS 
        [HttpDelete]
        [Route("EliminarUsuario")]
        public IActionResult EliminarUsuario([FromBody] Usuario usuario)
        {
            try
            {
                using (var bd = new ArchivoContext())                                       //instancia Archivo context
                {
                    var registro = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    //SI NO EXISTE nombre de usuario
                    if (registro.Count() == 0) //no existe
                    {
                        return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Usuario no se encuentra en el sistema"
                        });
                    }
                    //SI EXISTE
                    else//si existe se elimina
                    {
                        bd.Usuarios.Remove(registro.First());
                        bd.SaveChanges();
                        return StatusCode(StatusCodes.Status200OK,
                           new
                           {
                               mensaje = "OK",
                               respuesta = "Usuario eliminado correctamente"
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
                        respuesta = ex.InnerException?.Message ?? ex.Message
                    });
            }
        }

        //LOGIN
        [HttpPost]
        [Route("LoginUsuarios")]
        public ActionResult LoginUsuarios([FromBody] Usuario usuario)
        {
            try
            {
                using (var bd = new ArchivoContext())                           //instancia Archivo context                                      
                {
                    var registro = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    //SI NO EXISTE el nombre de usuario
                    if (registro.Count() == 0) //no existe
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
                        //DESARROLLAR PASW una vez modifiquemos la tabla usuarios
                        //para la contraseña
                        //if(registro.First().password == usuario.password)

                        //SI EL ESTADO es activo
                        if (registro.First().IdEstado == 1)
                        {
                            return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "OK",
                                respuesta = "Bienvenido: " + registro.First().NomUsuario
                            });
                        }
                        //SI NO ESTA ACTIVO
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
            //SI ERR
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
