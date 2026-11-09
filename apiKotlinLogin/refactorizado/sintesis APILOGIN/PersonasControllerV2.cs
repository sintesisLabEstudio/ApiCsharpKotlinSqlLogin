using API_Login.Models;
using Microsoft.AspNetCore.Mvc;
// ============================================================================
// GUIA DE CAMBIOS (busca la palabra "CAMBIO" con Ctrl+F para encontrarlos todos)
//   CAMBIO 1 : using innecesario comentado (Microsoft.Win32); System.Text.Json.Serialization se mantiene
//   CAMBIO 2 : los catch muestran el error real (InnerException)
//   CAMBIO 3 : validaciones de datos de entrada (largo, vacios, edad)
//   CAMBIO 4 : .ToList() para ejecutar la consulta UNA sola vez
//   CAMBIO 5 : validar que el ESTADO exista antes de insertar el usuario
//   CAMBIO 6 : no eliminar una persona que tiene usuarios asociados
//   CAMBIO 7 : comentario corregido sobre el codigo 200 vs 500
// El codigo original NO se borro: lo reemplazado queda comentado con //ANTES
// ============================================================================
// CAMBIO 1: este using lo agrega Visual Studio solo (lo confunde con "Registry") y no se usa.
//using Microsoft.Win32;

using System.Text.Json.Serialization; // [JsonIgnore] puede necesitarse en algunos casos (ej: ocultar propiedades al devolver objetos).

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
                        // CAMBIO 2: ex.Message solo dice "An error occurred while saving the entity changes..."
                        // y oculta el motivo real. El motivo real (ej: violacion de FK, texto truncado)
                        // viene en ex.InnerException. Si no hay InnerException se usa ex.Message.
                        // OJO: en un sistema real NO se muestra el detalle interno al cliente; para clases y pruebas es ideal.
                        //ANTES: respuesta = ex.Message
                        respuesta = ex.InnerException?.Message ?? ex.Message
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
                // CAMBIO 3: antes de tocar la base de datos se validan los datos recibidos.
                // Swagger manda edad = 0 por defecto, y por eso quedaron registros con edad 0.
                // ValidarPersona() esta al final de la clase; retorna null si todo esta bien.
                // Si hay error, retorna un texto con el error y se devuelve un 200 con mensaje = "ERROR"
                string? errorValidacion = ValidarPersona(per);
                if (errorValidacion != null)
                {
                    //ERR 200 con mensaje ERROR: error de negocio controlado (ver CAMBIO 7)
                    return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = errorValidacion
                        });
                }
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
                        //ANTES: respuesta = ex.Message
                        respuesta = ex.InnerException?.Message ?? ex.Message    // CAMBIO 2 (ver explicacion en ListadoPersonas)
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
                // CAMBIO 3: mismas validaciones que al insertar (nombre, apellido y edad)
                string? errorValidacion = ValidarPersona(persona);
                if (errorValidacion != null)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = errorValidacion
                        });
                }

                using (var bd = new ArchivoContext())
                {
                    // CAMBIO 4: Where() solo PREPARA la consulta (IQueryable), no la ejecuta.
                    // Sin ToList(), cada Count() y cada First() viajaba a la base de datos
                    // (aqui eran 1 Count + 3 First = 4 consultas). Con ToList() se ejecuta UNA vez
                    // y luego Count() y First() trabajan sobre la lista en memoria.
                    // Los objetos siguen "rastreados" por EF, asi que SaveChanges() sigue guardando los cambios.
                    //ANTES: var registro = bd.Personas.Where(x => x.IdPersona == persona.IdPersona);
                    var registro = bd.Personas.Where(x => x.IdPersona == persona.IdPersona).ToList();
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
                        //ENCUENTRA EL PRIMERO y llena el registro
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
                        //ANTES: respuesta = ex.Message
                        respuesta = ex.InnerException?.Message ?? ex.Message    // CAMBIO 2
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
                    // CAMBIO 4: ToList() ejecuta la consulta una sola vez (ver explicacion en EditarPersona)
                    //ANTES: var registro = bd.Personas.Where(x => x.IdPersona == persona.IdPersona); //consulta por id a eliminar
                    var registro = bd.Personas.Where(x => x.IdPersona == persona.IdPersona).ToList(); //consulta por id a eliminar
                    //NO EXISTE
                    if (registro.Count() == 0)
                    {
                        //RETORNA ERROR con mensaje y respuesta Nok
                        // CAMBIO 7: el 200 NO tiene relacion con "salir del try". La convencion de este ejercicio es:
                        //   - 200 con mensaje = "ERROR"  -> error de negocio controlado (no existe, ya existe, deshabilitado)
                        //   - 500                        -> excepcion inesperada (cae en el catch)
                        //ANTES (comentario): //err 200 para que no salga del try
                        return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "Persona no se encuentra en el sistema"
                        });
                    }
                    //EXISTE
                    else
                    {
                        // CAMBIO 6: USUARIO tiene una FK hacia PERSONA (FK_USUARIO_PERSONA).
                        // Si la persona tiene usuarios, SQL Server rechaza el DELETE y salia un 500 confuso.
                        // Ahora se avisa con un mensaje claro: primero se elimina el usuario (EliminarUsuario)
                        // y despues la persona.
                        if (bd.Usuarios.Any(u => u.IdPersona == persona.IdPersona))
                        {
                            return StatusCode(StatusCodes.Status200OK,
                            new
                            {
                                mensaje = "ERROR",
                                respuesta = "La persona tiene usuarios asociados. Elimine primero el usuario"
                            });
                        }

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
                        //ANTES: respuesta = ex.Message
                        respuesta = ex.InnerException?.Message ?? ex.Message    // CAMBIO 2
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
                        //ANTES: respuesta = ex.Message
                        respuesta = ex.InnerException?.Message ?? ex.Message    // CAMBIO 2
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
                // CAMBIO 3: NOM_USUARIO es varchar(10) en la base de datos. Un nombre mas largo
                // (ej: "juan espejo" tiene 11 caracteres) provoca "String or binary data would be truncated"
                // y terminaba en un 500. Se valida ANTES de llegar a la base de datos.
                if (usuario == null || string.IsNullOrWhiteSpace(usuario.NomUsuario) || usuario.NomUsuario.Length > 10)
                {
                    return StatusCode(StatusCodes.Status200OK,
                        new
                        {
                            mensaje = "ERROR",
                            respuesta = "El nombre de usuario es obligatorio y debe tener máximo 10 caracteres"
                        });
                }

                using (var bd = new ArchivoContext())
                {
                    //ENCUENTRA REGISTRO de usuario
                    // CAMBIO 4: ToList() ejecuta la consulta una sola vez (ver explicacion en EditarPersona)
                    //ANTES: var registroUsuario = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    var registroUsuario = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario).ToList();
                    if (registroUsuario.Count() == 0)
                    {
                        var personaExiste = bd.Personas.Where(x => x.IdPersona == usuario.IdPersona).Count(); //consulta si persona existe
                        //SI NO EXISTE la persona. El usuario debe estar en tabla personas
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
                            // CAMBIO 5: USUARIO.ID_ESTADO es FK hacia ESTADO, que solo tiene los valores 1 (HABILITADO)
                            // y 2 (DESHABILITADO). Swagger manda idEstado = 0 por defecto, y ese valor NO existe,
                            // por eso SQL Server rechazaba el INSERT (violacion de FK_USUARIO_ESTADO) con un 500.
                            // Se valida igual que ya se hacia con la persona.
                            var estadoExiste = bd.Estados.Any(x => x.IdEstado == usuario.IdEstado);
                            if (!estadoExiste)
                            {
                                return StatusCode(StatusCodes.Status200OK,
                                new
                                {
                                    mensaje = "ERROR",
                                    respuesta = "El estado no existe en el sistema"
                                });
                            }

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
                    //YA SE ENCUENTRA REGISTRO usuario 
                    else
                    {
                        //RETORNA mensaje respuesta
                        return StatusCode(StatusCodes.Status200OK,              //ERR 200: error de negocio controlado (ver CAMBIO 7)
                              new
                              {
                                  mensaje = "ERROR",
                                  respuesta = "Usuario ya se encuentra en el sistema"
                              });
                    }
                }
            }
            //SI ERROR 
            catch (Exception ex)
            {
                //RETORNA mensaje respuesta
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new
                    {
                        mensaje = "ERROR",
                        //ANTES: respuesta = ex.Message
                        respuesta = ex.InnerException?.Message ?? ex.Message    // CAMBIO 2
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
                    // CAMBIO 4: ToList() ejecuta la consulta una sola vez (ver explicacion en EditarPersona)
                    //ANTES: var registro = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    var registro = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario).ToList();
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
                // CAMBIO 2: este catch ya tenia InnerException; se mantiene igual
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
                    // CAMBIO 4: ToList() ejecuta la consulta una sola vez (ver explicacion en EditarPersona).
                    // Aqui se llamaba Count() y First() hasta 3 veces: ahora es una sola consulta.
                    //ANTES: var registro = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario);
                    var registro = bd.Usuarios.Where(x => x.NomUsuario == usuario.NomUsuario).ToList();
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
                        //para la contraseña (cuando se haga, guardarla con hash, nunca en texto plano)
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
                        //ANTES: respuesta = ex.Message
                        respuesta = ex.InnerException?.Message ?? ex.Message    // CAMBIO 2
                    });
            }
        }

        // ====================================================================
        // CAMBIO 3: METODO AUXILIAR (no es un endpoint, por eso es private y no lleva [Route]).
        // Se escribe UNA vez y lo usan InsertarPersona y EditarPersona, asi no se repite el codigo.
        // Retorna un texto con el error, o null si los datos son validos.
        // Los largos (30) salen de la tabla: NOM_PERSONA y APE_PERSONA son varchar(30).
        // El rango de edad (1 a 120) es un criterio del ejercicio; se puede ajustar.
        // ====================================================================
        private string? ValidarPersona(Persona? p)
        {
            if (p == null)
                return "Debe enviar los datos de la persona";
            if (string.IsNullOrWhiteSpace(p.NomPersona) || p.NomPersona.Length > 30)
                return "El nombre es obligatorio y debe tener máximo 30 caracteres";
            if (string.IsNullOrWhiteSpace(p.ApePersona) || p.ApePersona.Length > 30)
                return "El apellido es obligatorio y debe tener máximo 30 caracteres";
            if (p.Edad <= 0 || p.Edad > 120)
                return "La edad debe estar entre 1 y 120";
            return null;
        }
    }
}