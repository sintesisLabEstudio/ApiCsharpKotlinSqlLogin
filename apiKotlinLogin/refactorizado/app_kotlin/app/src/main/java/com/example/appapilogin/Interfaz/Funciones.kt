package com.example.appapilogin.Interfaz

import com.example.appapilogin.Clases.Datos
import com.example.appapilogin.Clases.DatosUsuario
import com.example.appapilogin.Clases.Persona
import com.example.appapilogin.Clases.Usuario
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.HTTP
import retrofit2.http.POST
import retrofit2.http.Query

interface Funciones {
    @GET("ListadoPersonas")
    suspend fun ListadoPersonas() : Response<Datos>

    @GET("ListadoUsuario")
    suspend fun ListadoUsuario() : Response<DatosUsuario>

    @POST("InsertarPersona")
    suspend fun InsertarPersona(@Body persona: Persona): Response<Datos>

    @POST("InsertarUsuario")
    suspend fun InsertarUsuario(@Body usuario: Usuario): Response<Datos>

    /* CÓDIGO ANTERIOR:
       No existía el método Login.
       EXPLICACIÓN: Definición del endpoint de inicio de sesión para el consumo desde Login.kt.
    */
    @POST("Login")
    suspend fun Login(@Body usuario: Usuario): Response<Datos>

    // AGREGADO [2026-11-08 21:18]: Firma para editar persona por POST
    @POST("EditarPersona")
    suspend fun EditarPersona(@Body persona: Persona): Response<Datos>

    // AGREGADO [2026-11-08 21:18]: Firma para eliminar persona enviando body en HTTP DELETE
    @HTTP(method = "DELETE", path = "EliminarPersona", hasBody = true)
    suspend fun EliminarPersona(@Body persona: Persona): Response<Datos>

    // AGREGADO [2026-11-09 00:11]: Firma para editar usuario enviando oldNomUsuario opcional para actualizar clave primaria
    @POST("EditarUsuario")
    suspend fun EditarUsuario(@Body usuario: Usuario, @Query("oldNomUsuario") oldNomUsuario: String? = null): Response<Datos>

    // AGREGADO [2026-11-08 21:18]: Firma para eliminar usuario
    @POST("EliminarUsuario")
    suspend fun EliminarUsuario(@Body usuario: Usuario): Response<Datos>
}
