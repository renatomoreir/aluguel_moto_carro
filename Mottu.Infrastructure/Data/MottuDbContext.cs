using Microsoft.EntityFrameworkCore;
using Mottu.Domain.Entities;

namespace Mottu.Infrastructure.Data
{
    public class MottuDbContext : DbContext
    {
        public MottuDbContext(DbContextOptions<MottuDbContext> options) : base(options)
        {
        }

        public DbSet<Moto> Motos { get; set; }
        public DbSet<Entregador> Entregadores { get; set; }
        public DbSet<Locacao> Locacoes { get; set; }
        public DbSet<NotificacaoMoto> NotificacaoMotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Moto
            modelBuilder.Entity<Moto>(entity =>
            {
                entity.HasKey(e => e.Identificador);
                entity.Property(e => e.Identificador).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Ano).IsRequired();
                entity.Property(e => e.Modelo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Placa).IsRequired().HasMaxLength(10);
                entity.Property(e => e.DataCriacao).IsRequired();
                
                // Índice único para placa
                entity.HasIndex(e => e.Placa).IsUnique();
            });

            // Configuração da entidade Entregador
            modelBuilder.Entity<Entregador>(entity =>
            {
                entity.HasKey(e => e.Identificador);
                entity.Property(e => e.Identificador).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(14);
                entity.Property(e => e.DataNascimento).IsRequired();
                entity.Property(e => e.NumeroCnh).IsRequired().HasMaxLength(20);
                entity.Property(e => e.TipoCnh).IsRequired();
                entity.Property(e => e.ImagemCnh).HasMaxLength(500);
                entity.Property(e => e.DataCriacao).IsRequired();
                
                // Índices únicos
                entity.HasIndex(e => e.Cnpj).IsUnique();
                entity.HasIndex(e => e.NumeroCnh).IsUnique();
            });

            // Configuração da entidade Locacao
            modelBuilder.Entity<Locacao>(entity =>
            {
                entity.HasKey(e => e.Identificador);
                entity.Property(e => e.Identificador).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EntregadorId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.MotoId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DataInicio).IsRequired();
                entity.Property(e => e.DataTermino).IsRequired();
                entity.Property(e => e.DataPrevisaoTermino).IsRequired();
                entity.Property(e => e.Plano).IsRequired();
                entity.Property(e => e.ValorDiaria).IsRequired().HasColumnType("decimal(10,2)");
                entity.Property(e => e.ValorTotal).HasColumnType("decimal(10,2)");
                entity.Property(e => e.DataCriacao).IsRequired();

                // Relacionamentos
                entity.HasOne(e => e.Entregador)
                    .WithMany(e => e.Locacoes)
                    .HasForeignKey(e => e.EntregadorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Moto)
                    .WithMany(m => m.Locacoes)
                    .HasForeignKey(e => e.MotoId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuração da entidade NotificacaoMoto
            modelBuilder.Entity<NotificacaoMoto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.MotoId).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Modelo).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Ano).IsRequired();
                entity.Property(e => e.Placa).IsRequired().HasMaxLength(10);
                entity.Property(e => e.DataNotificacao).IsRequired();
                entity.Property(e => e.Mensagem).HasMaxLength(500);
            });
        }
    }
}

