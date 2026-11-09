package com.example.appapilogin.Conexion

import com.example.appapilogin.Interfaz.Funciones
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object RetrofitCliente {
    // Si estás usando cable USB o datos móviles en el celular, usamos "127.0.0.1"
    // NOTA: Requiere ejecutar en la terminal: adb reverse tcp:5017 tcp:5017
    // adb reverse tcp:5017 tcp:5017
    var localhost = "127.0.0.1"
    var puerto = "5017"

    var ruta = Retrofit.Builder()
        .baseUrl("http://$localhost:$puerto/Producto/")
        .addConverterFactory(GsonConverterFactory.create())
        .build()
        .create(Funciones::class.java)
}
