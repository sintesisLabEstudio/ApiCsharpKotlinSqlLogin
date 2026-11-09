package com.example.appapilogin

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import android.widget.EditText
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.example.appapilogin.Clases.Datos
import com.example.appapilogin.Clases.Usuario
import com.example.appapilogin.Conexion.RetrofitCliente
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class Login : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_login)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        /* CÓDIGO ANTERIOR:
           El archivo Login.kt sólo contenía la plantilla inicial con la llamada a onCreate.
           EXPLICACIÓN: Se implementa la vinculación de componentes gráficos, validaciones y la petición a la API C# a través de la función de login.
        */
        val txtUsuario = findViewById<EditText>(R.id.txtNomUsuarioLogin)
        val txtClave = findViewById<EditText>(R.id.txtClaveLogin)
        val btnIngresar = findViewById<Button>(R.id.btnIngresarLogin)
        val btnVolver = findViewById<Button>(R.id.btnVolverLogin)

        btnVolver?.setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        }

        btnIngresar?.setOnClickListener {
            val user = txtUsuario?.text.toString().trim()
            val pass = txtClave?.text.toString().trim()

            if (user.isEmpty() || pass.isEmpty()) {
                Toast.makeText(this, "Debe ingresar usuario y contraseña", Toast.LENGTH_SHORT).show()
            } else {
                ejecutarLogin(user, pass)
            }
        }
    }

    private fun ejecutarLogin(usuario: String, clave: String) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val dtoUsuario = Usuario(usuario, clave, 0, 1)
                val llamada = RetrofitCliente.ruta.Login(dtoUsuario)

                withContext(Dispatchers.Main) {
                    if (llamada.isSuccessful) {
                        val body: Datos? = llamada.body()
                        if (body?.respuesta == "OK") {
                            Toast.makeText(this@Login, "Bienvenido: ${body.mensaje}", Toast.LENGTH_SHORT).show()
                            startActivity(Intent(this@Login, MainActivity::class.java))
                            finish()
                        } else {
                            Toast.makeText(this@Login, body?.mensaje ?: "Credenciales inválidas", Toast.LENGTH_LONG).show()
                        }
                    } else {
                        Toast.makeText(this@Login, "Error en el servidor: ${llamada.code()}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@Login, "Error de red: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }
}
