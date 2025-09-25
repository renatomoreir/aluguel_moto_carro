using System.ComponentModel.DataAnnotations;

namespace Mottu.Domain.Entities
{
    public class NotificacaoMoto
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string MotoId { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        public string Modelo { get; set; } = string.Empty;
        
        [Required]
        public int Ano { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string Placa { get; set; } = string.Empty;
        
        [Required]
        public DateTime DataNotificacao { get; set; } = DateTime.UtcNow;
        
        [MaxLength(500)]
        public string? Mensagem { get; set; }
    }
}

