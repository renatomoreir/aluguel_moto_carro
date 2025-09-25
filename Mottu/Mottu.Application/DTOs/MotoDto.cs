using System.ComponentModel.DataAnnotations;

namespace Mottu.Application.DTOs
{
    public class CriarMotoDto
    {
        [Required(ErrorMessage = "Identificador é obrigatório")]
        public string Identificador { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Ano é obrigatório")]
        [Range(1900, 2100, ErrorMessage = "Ano deve estar entre 1900 e 2100")]
        public int Ano { get; set; }
        
        [Required(ErrorMessage = "Modelo é obrigatório")]
        [MaxLength(100, ErrorMessage = "Modelo deve ter no máximo 100 caracteres")]
        public string Modelo { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Placa é obrigatória")]
        [MaxLength(10, ErrorMessage = "Placa deve ter no máximo 10 caracteres")]
        public string Placa { get; set; } = string.Empty;
    }
    
    public class MotoDto
    {
        public string Identificador { get; set; } = string.Empty;
        public int Ano { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
    }
    
    public class AtualizarPlacaMotoDto
    {
        [Required(ErrorMessage = "Placa é obrigatória")]
        [MaxLength(10, ErrorMessage = "Placa deve ter no máximo 10 caracteres")]
        public string Placa { get; set; } = string.Empty;
    }
}

