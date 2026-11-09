package com.example.appapilogin.Interfaz

import com.example.appapilogin.Clases.Datos
import com.example.appapilogin.Clases.DatosUsuario
import com.example.appapilogin.Clases.Persona
import com.example.appapilogin.Clases.Usuario
import retrofit2.Response
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.POST

interface Funciones {
    @GET("ListadoPersonas")//FUNCION DE LA API
    suspend fun ListadoPersonas() : Response<Datos>
    @GET("ListadoUsuario")
    suspend fun ListadoUsuario() : Response<DatosUsuario>
    @POST("InsertarPersona")
    suspend fun InsertarPersona(@Body persona: Persona): Response<Datos>
    @POST("InsertarUsuario")
    suspend fun InsertarUsuario(@Body usuario: Usuario):Response<Datos>

}