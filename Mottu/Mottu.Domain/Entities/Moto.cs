using System.ComponentModel.DataAnnotations;

namespace Mottu.Domain.Entities
{
    public class Moto
    {
        [Key]
        public string Identificador { get; set; } = string.Empty;
        
        [Required]
        public int Ano { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Modelo { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(10)]
        public string Placa { get; set; } = string.Empty;
        
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}

