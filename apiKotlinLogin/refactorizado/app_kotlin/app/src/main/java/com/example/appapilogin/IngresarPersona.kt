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
import com.example.appapilogin.Clases.Persona
import com.example.appapilogin.Conexion.RetrofitCliente
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class IngresarPersona : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_ingresar_persona)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        val btnVolver = findViewById<Button>(R.id.btnIPVolver)
        val btnLimpiar = findViewById<Button>(R.id.btnLimpiarPersona)
        val btnIngresar = findViewById<Button>(R.id.btnGuardarPersona)
        val txtNombres = findViewById<EditText>(R.id.txtNombres)
        val txtApellidos = findViewById<EditText>(R.id.txtApellidos)
        val txtEdad = findViewById<EditText>(R.id.txtEdad)

        btnVolver.setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        }
        btnLimpiar.setOnClickListener {
            txtNombres.text.clear()
            txtApellidos.text.clear()
            txtEdad.text.clear()
        }
        btnIngresar.setOnClickListener {
            var mensaje = ""
            if (txtNombres.text.isEmpty()) {
                mensaje += "Debe ingresar los nombres\n"
            }
            if (txtApellidos.text.isEmpty()) {
                mensaje += "Debe ingresar los apellidos\n"
            }
            if (txtEdad.text.isEmpty()) {
                mensaje += "Debe ingresar la edad\n"
            }

            /* CÓDIGO ANTERIOR:
               if (mensaje != "") { } else { ... }
               EXPLICACIÓN: Si había errores de validación, la primera rama quedaba vacía sin mostrar avisos al usuario. Se añade un Toast con los mensajes acumulados.
            */
            if (mensaje != "") {
                Toast.makeText(this, mensaje, Toast.LENGTH_LONG).show()
            } else {
                insertarPersona(
                    txtNombres.text.toString(),
                    txtApellidos.text.toString(),
                    txtEdad.text.toString().toInt()
                )
            }
        }
    }

    fun insertarPersona(txtNombres: String, txtApellidos: String, txtEdad: Int) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val datitos = Persona(
                    0,
                    txtNombres,
                    txtApellidos,
                    txtEdad
                )
                val llamada = RetrofitCliente.ruta.InsertarPersona(datitos)
                if (llamada.isSuccessful) {
                    val carga: Datos? = llamada.body()
                    withContext(Dispatchers.Main) {
                        val respuesta = carga?.respuesta
                        val mensaje = carga?.mensaje
                        Toast.makeText(
                            this@IngresarPersona,
                            "$respuesta: $mensaje",
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                } else {
                    withContext(Dispatchers.Main) {
                        Toast.makeText(this@IngresarPersona, "Error en el servidor: ${llamada.code()}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (ex: Exception) {
                /* CÓDIGO ANTERIOR:
                   } catch (ex: Exception) { }
                   EXPLICACIÓN: El bloque catch estaba vacío y ocultaba las fallas de conexión. Se añade alerta al usuario en el hilo principal.
                */
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@IngresarPersona, "Error de red: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }
}
