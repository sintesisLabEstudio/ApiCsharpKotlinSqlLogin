using APILogin.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Consultas
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /* CÓDIGO ANTERIOR:
               Se quitó ConfigureKestrel de aquí.
               EXPLICACIÓN: La configuración de Kestrel se gestiona ahora desde appsettings.json bajo la sección "Kestrel",
               evitando conflictos de dirección de lectura cuando se pasa una URL a app.Run().
            */

            builder.Services.AddControllers();

            /* CÓDIGO ANTERIOR:
               No se registraba el DbContext en el contenedor de dependencias.
               EXPLICACIÓN: Registrar ArchivoContext aquí permite inyectar el contexto globalmente y tomar la cadena de conexión de appsettings.json.
            */
            builder.Services.AddDbContext<ArchivoContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionArchivo")));

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
                    c.RoutePrefix = string.Empty; // Swagger en la página raíz
                });
            }

            /* CÓDIGO ANTERIOR:
               app.UseHttpsRedirection();
               EXPLICACIÓN: Se comenta temporalmente para evitar rechazos o redirecciones inesperadas durante peticiones HTTP no cifradas desde Android.
            */
            // app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            /* CÓDIGO ANTERIOR:
               app.Run("http://0.0.0.0:5017");
               EXPLICACIÓN: La URL se quitó porque ahora se configura en appsettings.json. 
               Simplemente llamar a app.Run() permite que ASP.NET Core use la URL configurada.
            */
            app.Run();
        }
}
}
