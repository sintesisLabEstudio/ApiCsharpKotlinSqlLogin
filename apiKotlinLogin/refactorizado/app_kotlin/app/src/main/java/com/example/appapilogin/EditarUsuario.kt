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
import com.example.appapilogin.Clases.UsuarioTrae
import com.example.appapilogin.Conexion.RetrofitCliente
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

// AGREGADO [2026-11-08 21:18]: Actividad para editar y eliminar usuarios del sistema
class EditarUsuario : AppCompatActivity() {
    private var listaPersonasCache: List<Persona> = emptyList()
    private var listaUsuariosCache: List<UsuarioTrae> = emptyList()
    private var listaUsuariosFiltrados: List<UsuarioTrae> = emptyList()
    // AGREGADO [2026-11-09 00:11]: Guarda el nombre de usuario original para enviarlo en query param
    private var usuarioOriginalSeleccionado: String = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_editar_usuario)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }

        val btnVolver = findViewById<Button>(R.id.btnVolverEditUsuario)
        val btnEditar = findViewById<Button>(R.id.btnEditarUsuario)
        val btnEliminar = findViewById<Button>(R.id.btnEliminarUsuario)
        val btnLimpiar = findViewById<Button>(R.id.btnLimpiarEditUsuario)

        val spinnerPersonas = findViewById<Spinner>(R.id.spinnerPersonasUsrEdit)
        val spinnerUsuarios = findViewById<Spinner>(R.id.spinnerUsuariosUsrEdit)

        val txtNomUsuario = findViewById<EditText>(R.id.txtNomUsuarioEdit)
        val txtClave = findViewById<EditText>(R.id.txtClaveEdit)
        val txtIdPersona = findViewById<EditText>(R.id.txtIdPersonaUsrEdit)
        val txtIdEstado = findViewById<EditText>(R.id.txtIdEstadoUsrEdit)

        cargarSpinners(spinnerPersonas, spinnerUsuarios, txtNomUsuario, txtClave, txtIdPersona, txtIdEstado)

        btnVolver.setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        }

        btnLimpiar.setOnClickListener {
            txtNomUsuario.text.clear()
            txtClave.text.clear()
            txtIdPersona.text.clear()
            txtIdEstado.text.clear()
        }

        btnEditar.setOnClickListener {
            val usr = txtNomUsuario.text.toString().trim()
            val pass = txtClave.text.toString().trim()
            val idPerStr = txtIdPersona.text.toString().trim()
            val idEstStr = txtIdEstado.text.toString().trim()

            if (usr.isEmpty() || idPerStr.isEmpty() || idEstStr.isEmpty()) {
                Toast.makeText(this, "Debe ingresar usuario, ID persona e ID estado", Toast.LENGTH_SHORT).show()
            } else {
                val oldUsr = if (usuarioOriginalSeleccionado.isEmpty()) usr else usuarioOriginalSeleccionado
                val claveFinal = if (pass.isEmpty()) "123456" else pass
                ejecutarEditarUsuario(usr, claveFinal, idPerStr.toInt(), idEstStr.toInt(), oldUsr, spinnerPersonas, spinnerUsuarios, txtNomUsuario, txtClave, txtIdPersona, txtIdEstado)
            }
        }

        btnEliminar.setOnClickListener {
            val usr = txtNomUsuario.text.toString().trim()
            if (usr.isEmpty()) {
                Toast.makeText(this, "Ingrese el nombre de usuario a eliminar", Toast.LENGTH_SHORT).show()
            } else {
                ejecutarEliminarUsuario(usr, spinnerPersonas, spinnerUsuarios, txtNomUsuario, txtClave, txtIdPersona, txtIdEstado)
            }
        }
    }

    private fun cargarSpinners(spinnerPer: Spinner, spinnerUsr: Spinner, txtUsr: EditText, txtPass: EditText, txtPer: EditText, txtEst: EditText) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val llamadaPer = RetrofitCliente.ruta.ListadoPersonas()
                val llamadaUsr = RetrofitCliente.ruta.ListadoUsuario()

                if (llamadaPer.isSuccessful && llamadaUsr.isSuccessful) {
                    listaPersonasCache = llamadaPer.body()?.lista ?: emptyList()
                    listaUsuariosCache = llamadaUsr.body()?.lista ?: emptyList()

                    val nombresPerList = listaPersonasCache.map { "${it.idPersona} - ${it.nomPersona} ${it.apePersona}" }

                    withContext(Dispatchers.Main) {
                        val adapterPer = ArrayAdapter(this@EditarUsuario, android.R.layout.simple_spinner_item, nombresPerList)
                        adapterPer.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                        spinnerPer.adapter = adapterPer

                        spinnerPer.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
                            override fun onItemSelected(parent: AdapterView<*>, view: View?, position: Int, id: Long) {
                                if (position < listaPersonasCache.size) {
                                    val personaSel = listaPersonasCache[position]
                                    txtPer.setText(personaSel.idPersona.toString())

                                    val nombreCompleto = "${personaSel.nomPersona} ${personaSel.apePersona}".trim()
                                    listaUsuariosFiltrados = listaUsuariosCache.filter { it.nombreApellido.equals(nombreCompleto, ignoreCase = true) }

                                    val nombresUsrList = listaUsuariosFiltrados.map { it.nomUsuario }

                                    val adapterUsr = ArrayAdapter(this@EditarUsuario, android.R.layout.simple_spinner_item, nombresUsrList)
                                    adapterUsr.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item)
                                    spinnerUsr.adapter = adapterUsr

                                    if (listaUsuariosFiltrados.isNotEmpty()) {
                                        spinnerUsr.setSelection(0)
                                        val usrSel = listaUsuariosFiltrados[0]
                                        usuarioOriginalSeleccionado = usrSel.nomUsuario
                                        txtUsr.setText(usrSel.nomUsuario)
                                        txtEst.setText(if (usrSel.nom_estado.equals("HABILITADO", ignoreCase = true)) "1" else "2")
                                    } else {
                                        usuarioOriginalSeleccionado = ""
                                        txtUsr.text.clear()
                                        txtPass.text.clear()
                                        txtEst.text.clear()
                                    }

                                    spinnerUsr.onItemSelectedListener = object : AdapterView.OnItemSelectedListener {
                                        override fun onItemSelected(parentUsr: AdapterView<*>, viewUsr: View?, posUsr: Int, idUsr: Long) {
                                            if (posUsr < listaUsuariosFiltrados.size) {
                                                val usuarioSel = listaUsuariosFiltrados[posUsr]
                                                usuarioOriginalSeleccionado = usuarioSel.nomUsuario
                                                txtUsr.setText(usuarioSel.nomUsuario)
                                                txtEst.setText(if (usuarioSel.nom_estado.equals("HABILITADO", ignoreCase = true)) "1" else "2")
                                            }
                                        }
                                        override fun onNothingSelected(parentUsr: AdapterView<*>) {}
                                    }
                                }
                            }
                            override fun onNothingSelected(parent: AdapterView<*>) {}
                        }
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@EditarUsuario, "Error al cargar datos: ${ex.message}", Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    private fun ejecutarEditarUsuario(usuario: String, clave: String, idPersona: Int, idEstado: Int, oldNomUsuario: String, spinnerPer: Spinner, spinnerUsr: Spinner, txtUsr: EditText, txtPass: EditText, txtPer: EditText, txtEst: EditText) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val dto = Usuario(usuario, clave, idPersona, idEstado)
                // AGREGADO [2026-11-09 00:11]: Envía oldNomUsuario para actualizar el nombre de usuario de forma limpia en la API
                val respuesta = RetrofitCliente.ruta.EditarUsuario(dto, oldNomUsuario)
                withContext(Dispatchers.Main) {
                    if (respuesta.isSuccessful) {
                        val body: Datos? = respuesta.body()
                        Toast.makeText(this@EditarUsuario, "${body?.respuesta}: ${body?.mensaje}", Toast.LENGTH_LONG).show()
                        cargarSpinners(spinnerPer, spinnerUsr, txtUsr, txtPass, txtPer, txtEst)
                    } else {
                        Toast.makeText(this@EditarUsuario, "Error del servidor: ${respuesta.code()}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@EditarUsuario, "Error de red: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }

    private fun ejecutarEliminarUsuario(usuario: String, spinnerPer: Spinner, spinnerUsr: Spinner, txtUsr: EditText, txtPass: EditText, txtPer: EditText, txtEst: EditText) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val dto = Usuario(usuario, "", 0, 0)
                val respuesta = RetrofitCliente.ruta.EliminarUsuario(dto)
                withContext(Dispatchers.Main) {
                    if (respuesta.isSuccessful) {
                        val body: Datos? = respuesta.body()
                        Toast.makeText(this@EditarUsuario, "${body?.respuesta}: ${body?.mensaje}", Toast.LENGTH_LONG).show()
                        txtUsr.text.clear()
                        txtPass.text.clear()
                        cargarSpinners(spinnerPer, spinnerUsr, txtUsr, txtPass, txtPer, txtEst)
                    } else {
                        Toast.makeText(this@EditarUsuario, "Error del servidor: ${respuesta.code()}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (ex: Exception) {
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@EditarUsuario, "Error de red: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }
}
