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
import com.example.appapilogin.Clases.Datos
import com.example.appapilogin.Conexion.RetrofitCliente
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext

class ListadoPersonas : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        enableEdgeToEdge()
        setContentView(R.layout.activity_listado_personas)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        val btnVolver = findViewById<Button>(R.id.btnLPVolver)
        val listview = findViewById<ListView>(R.id.lvListadoPersonas)

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
                val llamada = RetrofitCliente.ruta.ListadoPersonas()
                if (llamada.isSuccessful) {
                    val respuesta: Datos? = llamada.body()
                    withContext(Dispatchers.Main) {
                        val l = respuesta?.lista

                        l?.forEach { item ->
                            val id = item.idPersona
                            val nombre = item.nomPersona
                            val apellido = item.apePersona
                            val edad = item.edad
                            arregloListado.add("ID:$id - Nombre:$nombre  - Apellido:$apellido\nEdad:$edad")
                        }
                        adaptador = ArrayAdapter(
                            this@ListadoPersonas,
                            android.R.layout.simple_list_item_1,
                            arregloListado
                        )
                        listview.adapter = adaptador
                    }
                }
            } catch (ex: Exception) {
                /* CÓDIGO ANTERIOR:
                   } catch (ex: Exception){ var cosa = ex.toString(); var asasd = 1 }
                   EXPLICACIÓN: Se eliminan las variables locales no utilizadas y se informa la excepción ocurrida al usuario mediante un Toast.
                */
                withContext(Dispatchers.Main) {
                    Toast.makeText(this@ListadoPersonas, "Error al cargar la lista: ${ex.message}", Toast.LENGTH_LONG).show()
                }
            }
        }
    }
}
