package com.example.appapilogin

import android.content.Intent
import android.os.Bundle
import android.view.View
import android.widget.AdapterView
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.EditText
import android.widget.Spinner
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.example.appapilogin.Clases.Datos
import com.example.appapilogin.Clases.Persona
import com.example.appapilogin.Clases.Usuario
import com.example.appapilogin.Conexion.RetrofitCliente
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class IngresarUsuario : AppCompatActivity() {
    // AGREGADO [2026-11-08 23:32]: Caché para poblar el Spinner de personas en el formulario de ingreso de usuarios
    private var listaPersonasCache: List<Persona> = emptyList()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_ingresar_usuario)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        val btnVolver = findViewById<Button>(R.id.btnIUVolver)
        val btnGuardar = findViewById<Button>(R.id.btnGuardarUsuario)
        val spinnerPersonas = findViewById<Spinner>(R.id.spinnerPersonasIngresarUsr)
        val txtNomUsuario = findViewById<EditText>(R.id.txtNomUsuario)
        val txtClave = findViewById<EditText>(R.id.txtClave)
        val txtIdPersona = findViewById<EditText>(R.id.txtIdPersona)
        val txtIdEstado = findViewById<EditText>(R.id.txtIdEstado)

        cargarSpinnerPersonas(spinnerPersonas, txtIdPersona)

        btnVolver.setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        }

        btnGuardar.setOnClickListener {
            val usr = txtNomUsuario.text.toString().trim()
            val pass = txtClave.text.toString().trim()
            val idPer = txtIdPersona.text.toString().trim()
            val idEst = txtIdEstado.text.toString().trim()

            if (usr.isEmpty() || idPer.isEmpty() || idEst.isEmpty()) {
                Toast.makeText(this, "Complete los campos obligatorios", Toast.LENGTH_SHORT).show()
            } else {
                guardarUsuario(usr, pass, idPer.toInt(), idEst.toInt())
            }
        }
    }

    // AGREGADO [2026-11-08 23:32]: Carga asíncrona de personas en el Spinner y asignación automática de ID Persona
    private fun cargarSpinnerPersonas(spinner: Spinner, txtIdPer: EditText) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val llamada = RetrofitCliente.ruta.ListadoPersonas()
                if (llamada.isSuccessful) {
                    val body = llamada.body()
                    listaPersonasCache = body?.lista ?: emptyList()
                    val nombresList = listaPersonasCache.map { "${it.idPersona} - ${it.nomPersona} ${it.apePersona}" }

                    withContext(Dispatchers.Main) {
                        val adapter = ArrayAdapter(this@IngresarUsuario, android.R.layout.simple_spinner_item, nombresList)
                        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                        spinner.adapter = adapter

                        spinner.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
                            override fun onItemSelected(parent: AdapterView<*>, view: View?, position: Int, id: Long) {
                                if (position < listaPersonasCache.size) {
                                    val personaSel = listaPersonasCache[position]
                                    txtIdPer.setText(personaSel.idPersona.toString())
                                }
                            }
                            override fun onNothingSelected(parent: AdapterView<*>) {}
                        }
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@IngresarUsuario, "Error al cargar personas: ${ex.message}", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    private fun guardarUsuario(usuario: String, clave: String, idPersona: Int, idEstado: Int) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val nuevoUsr = Usuario(
                    usuario,
                    if (clave.isEmpty()) "123456" else clave,
                    idPersona,
                    idEstado
                )
                val llamada = RetrofitCliente.ruta.InsertarUsuario(nuevoUsr)

                withContext(Dispatchers.Main) {
                    if (llamada.isSuccessful) {
                        val body: Datos? = llamada.body()
                        Toast.makeText(this@IngresarUsuario, "${body?.respuesta}: ${body?.mensaje}", Toast.LENGTH_LONG).show()
                    } else {
                        Toast.makeText(this@IngresarUsuario, "Error del servidor: ${llamada.code()}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@IngresarUsuario, "Error de conexión: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }
}
