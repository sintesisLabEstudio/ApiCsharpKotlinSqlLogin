using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APILogin.Models;

public partial class ArchivoContext : DbContext
{
    public ArchivoContext()
    {
    }

    public ArchivoContext(DbContextOptions<ArchivoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        /* CÓDIGO ANTERIOR:
           => optionsBuilder.UseSqlServer("Server=.\SQLEXPRESS;Database=ARCHIVO;Trusted_Connection=SSPI;MultipleActiveResultSets=true;Trust Server Certificate=true");
           EXPLICACIÓN: Se envuelve la llamada dentro de !optionsBuilder.IsConfigured para que si ya viene configurado vía inyección de dependencias en Program.cs use la configuración de appsettings.json.
        */
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=ARCHIVO;Trusted_Connection=SSPI;MultipleActiveResultSets=true;Trust Server Certificate=true");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.IdEstado);

            entity.ToTable("ESTADO");

            entity.Property(e => e.IdEstado)
                .ValueGeneratedNever()
                .HasColumnName("ID_ESTADO");
            entity.Property(e => e.NomEstado)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("NOM_ESTADO");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona);

            entity.ToTable("PERSONA");

            entity.Property(e => e.IdPersona).HasColumnName("ID_PERSONA");
            entity.Property(e => e.ApePersona)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("APE_PERSONA");
            entity.Property(e => e.Edad).HasColumnName("EDAD");
            entity.Property(e => e.NomPersona)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("NOM_PERSONA");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.NomUsuario);

            entity.ToTable("USUARIO");

            entity.Property(e => e.NomUsuario)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("NOM_USUARIO");

            /* CÓDIGO ANTERIOR:
               No existía la propiedad Clave mapeada en la entidad EF.
               EXPLICACIÓN: Mapeo de la columna CLAVE hacia la entidad Usuario.
            */
            entity.Property(e => e.Clave)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("123456")
                .HasColumnName("CLAVE");

            entity.Property(e => e.IdEstado).HasColumnName("ID_ESTADO");
            entity.Property(e => e.IdPersona).HasColumnName("ID_PERSONA");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdEstado)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USUARIO_ESTADO");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_USUARIO_PERSONA");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
