package com.example.appapilogin.Clases

/* CÓDIGO ANTERIOR:
import com.google.gson.annotations.SerializedName

data class DatosUsuario(
    @SerializedName("respuesta") var respuesta : String,
    @SerializedName("mensaje") var mensaje : String,
    @SerializedName("lista") var lista: List<UsuarioTrae>? = null
)

data class UsuarioTrae(
    @SerializedName("nomUsuario") var nomUsuario : String,
    @SerializedName("nombreApellido") var nombreApellido : String,
    @SerializedName("edad") var edad : Int,
    @SerializedName("nom_estado") var nom_estado : String
)
EXPLICACIÓN: Se remueven las anotaciones @SerializedName para restaurar la estructura original limpia.
*/

// AGREGADO [2026-11-08 20:30]: Restauración del modelo DatosUsuario sin anotaciones @SerializedName a solicitud del usuario
data class DatosUsuario(
    var respuesta : String,
    var mensaje : String,
    var lista: List<UsuarioTrae>? = null
)

data class UsuarioTrae(
    var nomUsuario : String,
    var nombreApellido : String,
    var edad : Int,
    var nom_estado : String
)
