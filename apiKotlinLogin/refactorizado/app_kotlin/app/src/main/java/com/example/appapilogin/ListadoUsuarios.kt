package com.example.appapilogin

import android.content.Intent
import android.os.Bundle
import android.widget.ArrayAdapter
import android.widget.Button
import android.widget.ListView
import android.widget.Toast
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat
import com.example.appapilogin.Clases.DatosUsuario
import com.example.appapilogin.Conexion.RetrofitCliente
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class ListadoUsuarios : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_listado_usuarios)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        val btnVolver = findViewById<Button>(R.id.btnLUVolver)
        val listview = findViewById<ListView>(R.id.lvListadoUsuarios)

        btnVolver.setOnClickListener {
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        }

        cargaDatos(listview)

        listview.setOnItemClickListener { parent, _, position, _ ->
            val elemento = parent.getItemAtPosition(position)
            Toast.makeText(this, "Elemento: $elemento", Toast.LENGTH_SHORT).show()
        }
    }

    fun cargaDatos(listview: ListView) {
        var adaptador: ArrayAdapter<*>
        val arregloListado = ArrayList<String>()

        CoroutineScope(Dispatchers.IO).launch {
            try {
                val llamada = RetrofitCliente.ruta.ListadoUsuario()
                if (llamada.isSuccessful) {
                    val respuesta: DatosUsuario? = llamada.body()
                    withContext(Dispatchers.Main) {
                        val l = respuesta?.lista

                        l?.forEach { item ->
                            val usuario = item.nomUsuario
                            val nombre = item.nombreApellido
                            val edad = item.edad
                            val estado = item.nom_estado
                            arregloListado.add("USUARIO: $usuario - Nombre: $nombre Edad: $edad \nEstado: $estado")
                        }
                        adaptador = ArrayAdapter(
                            this@ListadoUsuarios,
                            android.R.layout.simple_list_item_1,
                            arregloListado
                        )
                        listview.adapter = adaptador
                    }
                } else {
                    // AGREGADO [2026-11-08 20:50]: Manejo de respuestas no exitosas del servidor para notificar al usuario
                    withContext(Dispatchers.Main) {
                        Toast.makeText(this@ListadoUsuarios, "Error del servidor: ${llamada.code()}", Toast.LENGTH_SHORT).show()
                    }
                }
            } catch (ex: Exception) {
                /* CÓDIGO ANTERIOR:
                   } catch (ex: Exception){ var cosa = ex.toString(); var asasd = 1 }
                   EXPLICACIÓN: Se sustituye la captura pasiva por una respuesta de interfaz visual al usuario ante fallos de conexión.
                */
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@ListadoUsuarios, "Error al cargar usuarios: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }
}
