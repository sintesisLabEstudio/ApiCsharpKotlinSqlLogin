package com.example.appapilogin.Clases

data class Usuario (
    var nomUsuario : String,        //pk
    var clave: String = "123456",
    var idPersona : Int,            //fk
    var idEstado : Int              //fk
)
