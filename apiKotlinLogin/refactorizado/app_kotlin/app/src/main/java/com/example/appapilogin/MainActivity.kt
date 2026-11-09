package com.example.appapilogin

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import androidx.activity.enableEdgeToEdge
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.app.AppCompatDelegate
import androidx.core.view.ViewCompat
import androidx.core.view.WindowInsetsCompat

class MainActivity : AppCompatActivity() {
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        // AGREGADO [2026-11-08 20:55]: Activar modo oscuro con gris atenuado para confort y vista delicada
        AppCompatDelegate.setDefaultNightMode(AppCompatDelegate.MODE_NIGHT_YES)
        enableEdgeToEdge()
        setContentView(R.layout.activity_main)
        ViewCompat.setOnApplyWindowInsetsListener(findViewById(R.id.main)) { v, insets ->
            val systemBars = insets.getInsets(WindowInsetsCompat.Type.systemBars())
            v.setPadding(systemBars.left, systemBars.top, systemBars.right, systemBars.bottom)
            insets
        }
        val btnListadoPersonas = findViewById<Button>(R.id.btnPersonas)
        val btnListadoUsuarios = findViewById<Button>(R.id.btnUsuarios)
        val btnInsertarPersonas = findViewById<Button>(R.id.btnIngresarPersona)
        val btnInsertarUsuarios = findViewById<Button>(R.id.btnIngresarUsuario)
        val btnEditarPersonaMain = findViewById<Button>(R.id.btnEditarPersonaMain)
        val btnEditarUsuarioMain = findViewById<Button>(R.id.btnEditarUsuarioMain)
        val btnLogin = findViewById<Button>(R.id.btnLogin)

        btnListadoPersonas.setOnClickListener {
            startActivity(Intent(this, ListadoPersonas::class.java))
            finish()
        }
        btnListadoUsuarios.setOnClickListener {
            startActivity(Intent(this, ListadoUsuarios::class.java))
            finish()
        }
        btnInsertarPersonas.setOnClickListener {
            startActivity(Intent(this, IngresarPersona::class.java))
            finish()
        }
        btnInsertarUsuarios.setOnClickListener {
            startActivity(Intent(this, IngresarUsuario::class.java))
            finish()
        }
        // AGREGADO [2026-11-08 21:18]: Navegación hacia las actividades Editar/Eliminar Persona y Usuario
        btnEditarPersonaMain?.setOnClickListener {
            startActivity(Intent(this, EditarPersona::class.java))
            finish()
        }
        btnEditarUsuarioMain?.setOnClickListener {
            startActivity(Intent(this, EditarUsuario::class.java))
            finish()
        }
        btnLogin.setOnClickListener {
            startActivity(Intent(this, Login::class.java))
            finish()
        }
    }
}
