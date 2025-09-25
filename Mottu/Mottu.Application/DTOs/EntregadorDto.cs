using System.ComponentModel.DataAnnotations;
using Mottu.Domain.Enums;

namespace Mottu.Application.DTOs
{
    public class CriarEntregadorDto
    {
        [Required(ErrorMessage = "Identificador é obrigatório")]
        public string Identificador { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [MaxLength(14, ErrorMessage = "CNPJ deve ter no máximo 14 caracteres")]
        public string Cnpj { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        public DateTime DataNascimento { get; set; }
        
        [Required(ErrorMessage = "Número da CNH é obrigatório")]
        [MaxLength(20, ErrorMessage = "Número da CNH deve ter no máximo 20 caracteres")]
        public string NumeroCnh { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Tipo da CNH é obrigatório")]
        public TipoCnh TipoCnh { get; set; }
        
        public string? ImagemCnh { get; set; }
    }
    
    public class EntregadorDto
    {
        public string Identificador { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string NumeroCnh { get; set; } = string.Empty;
        public TipoCnh TipoCnh { get; set; }
        public string? ImagemCnh { get; set; }
    }
}

