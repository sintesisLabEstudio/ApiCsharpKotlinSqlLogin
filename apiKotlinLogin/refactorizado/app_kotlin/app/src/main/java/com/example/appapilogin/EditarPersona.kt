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
import com.example.appapilogin.Conexion.RetrofitCliente
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

// AGREGADO [2026-11-08 21:18]: Actividad para editar y eliminar registros de personas consumiendo la API C#
class EditarPersona : AppCompatActivity() {
    // AGREGADO [2026-11-08 22:33]: Caché de personas para mapeo instantáneo al seleccionar en el Spinner
    private var listaPersonasCache: List<Persona> = emptyList()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_editar_persona)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        val btnVolver = findViewById<Button>(R.id.btnVolverEditPersona)
        val btnEditar = findViewById<Button>(R.id.btnEditarPersona)
        val btnEliminar = findViewById<Button>(R.id.btnEliminarPersona)
        val btnLimpiar = findViewById<Button>(R.id.btnLimpiarEditPersona)

        val spinnerPersonas = findViewById<Spinner>(R.id.spinnerPersonasEdit)
        val txtIdPersona = findViewById<EditText>(R.id.txtIdPersonaEdit)
        val txtNombres = findViewById<EditText>(R.id.txtNombresEdit)
        val txtApellidos = findViewById<EditText>(R.id.txtApellidosEdit)
        val txtEdad = findViewById<EditText>(R.id.txtEdadEdit)

        // Cargar datos en el Spinner al iniciar la actividad
        cargarSpinnerPersonas(spinnerPersonas, txtIdPersona, txtNombres, txtApellidos, txtEdad)

        btnVolver.setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        }

        btnLimpiar.setOnClickListener {
            txtIdPersona.text.clear()
            txtNombres.text.clear()
            txtApellidos.text.clear()
            txtEdad.text.clear()
        }

        btnEditar.setOnClickListener {
            val idStr = txtIdPersona.text.toString().trim()
            val nom = txtNombres.text.toString().trim()
            val ape = txtApellidos.text.toString().trim()
            val edadStr = txtEdad.text.toString().trim()

            if (idStr.isEmpty() || nom.isEmpty() || ape.isEmpty() || edadStr.isEmpty()) {
                Toast.makeText(this, "Complete todos los campos para editar", Toast.LENGTH_SHORT).show()
            } else {
                ejecutarEditarPersona(idStr.toInt(), nom, ape, edadStr.toInt(), spinnerPersonas, txtIdPersona, txtNombres, txtApellidos, txtEdad)
            }
        }

        btnEliminar.setOnClickListener {
            val idStr = txtIdPersona.text.toString().trim()
            if (idStr.isEmpty()) {
                Toast.makeText(this, "Ingrese el ID de la persona a eliminar", Toast.LENGTH_SHORT).show()
            } else {
                ejecutarEliminarPersona(idStr.toInt(), spinnerPersonas, txtIdPersona, txtNombres, txtApellidos, txtEdad)
            }
        }
    }

    // AGREGADO [2026-11-08 22:33]: Función asíncrona para poblar el Spinner y autocompletar campos al seleccionar
    private fun cargarSpinnerPersonas(spinner: Spinner, txtId: EditText, txtNom: EditText, txtApe: EditText, txtEdad: EditText) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val llamada = RetrofitCliente.ruta.ListadoPersonas()
                if (llamada.isSuccessful) {
                    val body = llamada.body()
                    listaPersonasCache = body?.lista ?: emptyList()
                    val nombresList = listaPersonasCache.map { "${it.idPersona} - ${it.nomPersona} ${it.apePersona}" }

                    withContext(Dispatchers.Main) {
                        val adapter = ArrayAdapter(this@EditarPersona, android.R.layout.simple_spinner_item, nombresList)
                        adapter.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                        spinner.adapter = adapter

                        spinner.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
                            override fun onItemSelected(parent: AdapterView<*>, view: View?, position: Int, id: Long) {
                                if (position < listaPersonasCache.size) {
                                    val seleccionada = listaPersonasCache[position]
                                    txtId.setText(seleccionada.idPersona.toString())
                                    txtNom.setText(seleccionada.nomPersona)
                                    txtApe.setText(seleccionada.apePersona)
                                    txtEdad.setText(seleccionada.edad.toString())
                                }
                            }
                            override fun onNothingSelected(parent: AdapterView<*>) {}
                        }
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@EditarPersona, "Error al cargar personas: ${ex.message}", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    private fun ejecutarEditarPersona(id: Int, nombres: String, apellidos: String, edad: Int, spinner: Spinner, txtId: EditText, txtNom: EditText, txtApe: EditText, txtEdad: EditText) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val persona = Persona(id, nombres, apellidos, edad)
                val respuesta = RetrofitCliente.ruta.EditarPersona(persona)
                withContext(Dispatchers.Main) {
                    if (respuesta.isSuccessful) {
                        val body: Datos? = respuesta.body()
                        Toast.makeText(this@EditarPersona, "${body?.respuesta}: ${body?.mensaje}", Toast.LENGTH_LONG).show()
                        cargarSpinnerPersonas(spinner, txtId, txtNom, txtApe, txtEdad)
                    } else {
                        Toast.makeText(this@EditarPersona, "Error del servidor: ${respuesta.code()}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@EditarPersona, "Error de red: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }

    private fun ejecutarEliminarPersona(id: Int, spinner: Spinner, txtId: EditText, txtNom: EditText, txtApe: EditText, txtEdad: EditText) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val persona = Persona(id, "", "", 0)
                val respuesta = RetrofitCliente.ruta.EliminarPersona(persona)
                withContext(Dispatchers.Main) {
                    if (respuesta.isSuccessful) {
                        val body: Datos? = respuesta.body()
                        Toast.makeText(this@EditarPersona, "${body?.respuesta}: ${body?.mensaje}", Toast.LENGTH_LONG).show()
                        txtId.text.clear()
                        txtNom.text.clear()
                        txtApe.text.clear()
                        txtEdad.text.clear()
                        cargarSpinnerPersonas(spinner, txtId, txtNom, txtApe, txtEdad)
                    } else {
                        Toast.makeText(this@EditarPersona, "Error del servidor: ${respuesta.code()}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@EditarPersona, "Error de red: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }
}
