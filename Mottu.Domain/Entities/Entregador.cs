using System.ComponentModel.DataAnnotations;
using Mottu.Domain.Enums;

namespace Mottu.Domain.Entities
{
    public class Entregador
    {
        [Key]
        public string Identificador { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(14)]
        public string Cnpj { get; set; } = string.Empty;
        
        [Required]
        public DateTime DataNascimento { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string NumeroCnh { get; set; } = string.Empty;
        
        [Required]
        public TipoCnh TipoCnh { get; set; }
        
        public string? ImagemCnh { get; set; }
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}

