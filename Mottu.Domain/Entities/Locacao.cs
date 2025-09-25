using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mottu.Domain.Entities
{
    public class Locacao
    {
        [Key]
        public string Identificador { get; set; } = string.Empty;
        
        [Required]
        public string EntregadorId { get; set; } = string.Empty;
        
        [Required]
        public string MotoId { get; set; } = string.Empty;
        
        [Required]
        public DateTime DataInicio { get; set; }
        
        [Required]
        public DateTime DataTermino { get; set; }
        
        [Required]
        public DateTime DataPrevisaoTermino { get; set; }
        
        public DateTime? DataDevolucao { get; set; }
        
        [Required]
        public int Plano { get; set; } // 7, 15, 30, 45, 50 dias
        
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorDiaria { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal? ValorTotal { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        [ForeignKey("EntregadorId")]
        public virtual Entregador Entregador { get; set; } = null!;
        
        [ForeignKey("MotoId")]
        public virtual Moto Moto { get; set; } = null!;
        
        // Business methods
        public decimal CalcularValorTotal()
        {
            if (DataDevolucao == null)
                return 0;

            var diasLocacao = (DataDevolucao.Value.Date - DataInicio.Date).Days;
            var diasPlano = Plano;
            var valorBase = diasPlano * ValorDiaria;

            // Se devolveu antes do prazo
            if (DataDevolucao < DataPrevisaoTermino)
            {
                var diasNaoEfetivados = (DataPrevisaoTermino.Date - DataDevolucao.Value.Date).Days;
                var valorDiariasEfetivadas = diasLocacao * ValorDiaria;
                
                decimal percentualMulta = Plano switch
                {
                    7 => 0.20m,
                    15 => 0.40m,
                    _ => 0m
                };
                
                var valorMulta = diasNaoEfetivados * ValorDiaria * percentualMulta;
                return valorDiariasEfetivadas + valorMulta;
            }
            
            // Se devolveu após o prazo
            if (DataDevolucao > DataPrevisaoTermino)
            {
                var diasAdicionais = (DataDevolucao.Value.Date - DataPrevisaoTermino.Date).Days;
                var valorAdicional = diasAdicionais * 50m; // R$ 50,00 por dia adicional
                return valorBase + valorAdicional;
            }
            
            // Devolveu no prazo
            return valorBase;
        }
        
        public static decimal ObterValorDiaria(int plano)
        {
            return plano switch
            {
                7 => 30.00m,
                15 => 28.00m,
                30 => 22.00m,
                45 => 20.00m,
                50 => 18.00m,
                _ => throw new ArgumentException("Plano inválido")
            };
        }
    }
}

