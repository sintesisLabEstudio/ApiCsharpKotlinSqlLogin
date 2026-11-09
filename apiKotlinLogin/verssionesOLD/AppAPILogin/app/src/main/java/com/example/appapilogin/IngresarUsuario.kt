package com.example.appapilogin

import android.content.Intent
import android.os.Bundle
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
import com.example.appapilogin.Clases.Usuario
import com.example.appapilogin.Conexion.RetrofitCliente
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class IngresarUsuario : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_ingresar_usuario)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        var btnVolver = findViewById<Button>(R.id.btnIUVolver)
        var cmbPersonas = findViewById<Spinner>(R.id.cmbPersonas)
        var btnGuardar = findViewById<Button>(R.id.btnInsertarUsuario)
        var txtUsuario = findViewById<EditText>(R.id.txtNomUsuario)

        btnVolver.setOnClickListener {
            startActivity(Intent(this,MainActivity::class.java))
            finish()
        }

        cargaPersonas(cmbPersonas)

        btnGuardar.setOnClickListener {
            if(txtUsuario.text.isEmpty()){
                Toast.makeText(this, "Ingrese el usuario",
                    Toast.LENGTH_SHORT).show()
            }else{
                var textoSpinner = cmbPersonas.selectedItem.toString()
                var dividir = textoSpinner.split("-")
                var id = dividir[0].toInt()
                insertarUsr(txtUsuario.text.toString(), id)
            }
        }
    }
    fun cargaPersonas(cmbPersonas : Spinner){
        CoroutineScope(Dispatchers.IO).launch {
            try {
                var llamada = RetrofitCliente.ruta.ListadoPersonas()
                if(llamada.isSuccessful){
                    var carga : Datos? = llamada.body()
                    withContext(Dispatchers.Main){
                        var mensaje = carga?.mensaje
                        var respuesta = carga?.respuesta
                        var lista = carga?.lista

                        var listadoArr = ArrayList<String>()
                        var adaptador : ArrayAdapter<*>

                        lista?.forEach { item ->
                            listadoArr.add("${item.idPersona}-${item.nomPersona} ${item.apePersona}")
                        }
                        adaptador = ArrayAdapter(this@IngresarUsuario,
                            android.R.layout.simple_spinner_dropdown_item,
                            listadoArr)
                        cmbPersonas.adapter = adaptador
                    }
                }
            }catch (ex : Exception){

            }
        }
    }

    fun insertarUsr(txtUsuario: String, id: Int){
        CoroutineScope(Dispatchers.IO).launch {
            try{
                var datitos = Usuario(txtUsuario, id, 1)
                var llamada = RetrofitCliente.ruta.InsertarUsuario(datitos)
                if(llamada.isSuccessful){
                    var carga: Datos? = llamada?.body()
                    withContext(Dispatchers.Main){
                        var respuesta = carga?.respuesta
                        var mensaje = carga?.mensaje

                        Toast.makeText(this@IngresarUsuario,
                            "$respuesta: $mensaje",
                            Toast.LENGTH_SHORT).show()
                    }
                }
            }catch (ex : Exception){

            }
        }
    }
}