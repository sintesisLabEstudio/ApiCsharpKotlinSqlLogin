package com.example.appapilogin.Clases

data class Datos(
    var respuesta : String,
    var mensaje : String,

    /* CÓDIGO ANTERIOR:
       var lista: List<Persona>
       EXPLICACIÓN: Se hace nula opcional (List<Persona>? = null). Peticiones como InsertarPersona o InsertarUsuario devuelven JSON sin la propiedad 'lista', provocando una excepción de deserialización en Gson.
    */
    var lista: List<Persona>? = null
)
