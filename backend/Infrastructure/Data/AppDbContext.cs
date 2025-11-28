using Microsoft.EntityFrameworkCore;
using ProjetoApiPT.Domain.Entities;
using System.Collections.Generic;
using System.Linq; 

namespace ProjetoApiPT.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cliente>? Clientes { get; set; }
        public DbSet<Proposta>? Propostas { get; set; }
        public DbSet<Modelo>? Modelos { get; set; }
        public DbSet<EnvioFormulario>? EnviosFormularios { get; set; }
        public DbSet<ProcessoModelo>? ProcessosModelos { get; set; }
        public DbSet<Usuario>? Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Mensagem).IsRequired().HasMaxLength(1000);

                entity.Property(e => e.Telefone).HasMaxLength(50).HasColumnType("TEXT");
                entity.Property(e => e.PdfGerado);
                entity.Property(e => e.DataCadastro).HasDefaultValueSql("UTC_TIMESTAMP()"); 
            });

            modelBuilder.Entity<Proposta>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NomeCliente).IsRequired().HasMaxLength(255);
                entity.Property(e => e.EmailCliente).IsRequired().HasMaxLength(255);
                entity.Property(e => e.StatusValidacao).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Valor).HasPrecision(18, 2);
                
                entity.Property(e => e.MensagemEquipe).HasMaxLength(1000);
                entity.Property(e => e.Responsavel).HasMaxLength(255);
                entity.Property(e => e.DataCriacao).HasDefaultValueSql("UTC_TIMESTAMP()"); 

                entity.Property(e => e.Slides).HasColumnType("TEXT");
                entity.Property(e => e.PdfUrl).HasMaxLength(1000).HasColumnType("TEXT");

                // Relacionamento com Cliente
                entity.HasOne<ProjetoApiPT.Domain.Entities.Cliente>()
                      .WithMany(c => c.Propostas)
                      .HasForeignKey("ClienteId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NomeUsuario).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.HashSenha).IsRequired();
                entity.Property(e => e.PerfilAcesso).IsRequired().HasMaxLength(50).HasDefaultValue("User");

                entity.HasIndex(e => e.NomeUsuario).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.DataCriacao).HasDefaultValueSql("UTC_TIMESTAMP()"); 
            });


            modelBuilder.Entity<Modelo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Titulo).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Descricao).HasMaxLength(1000);
                entity.Property(e => e.Plano).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DataCriacao).HasDefaultValueSql("UTC_TIMESTAMP()");

        
                entity.HasMany(m => m.EnviosFormularios)
                      .WithOne(e => e.Modelo!)          
                      .HasForeignKey(e => e.IdModelo)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade);


                entity.HasMany(m => m.ProcessosModelos)
                      .WithOne(p => p.Modelo)           
                      .HasForeignKey(p => p.IdModelo)
                      .IsRequired()
                      .OnDelete(DeleteBehavior.Cascade); // Excluir filhos quando o pai for excluído
            });
            
 
            modelBuilder.Entity<EnvioFormulario>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NomeLead).IsRequired().HasMaxLength(255);
                entity.Property(e => e.EmailContato).IsRequired().HasMaxLength(255);
                entity.Property(e => e.StatusEnvio).IsRequired().HasMaxLength(50).HasDefaultValue("Pendente");
                entity.Property(e => e.DadosFormularioJson).IsRequired();
                entity.Property(e => e.DataEnvio).HasDefaultValueSql("UTC_TIMESTAMP()");
            });

            // ProcessoModelo
            modelBuilder.Entity<ProcessoModelo>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DescricaoProcesso).IsRequired().HasMaxLength(500);
            });
        }
    }
}