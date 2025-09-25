using System.ComponentModel.DataAnnotations;

namespace Mottu.Application.DTOs
{
    public class CriarLocacaoDto
    {
        [Required(ErrorMessage = "Identificador é obrigatório")]
        public string Identificador { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "ID do entregador é obrigatório")]
        public string EntregadorId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "ID da moto é obrigatório")]
        public string MotoId { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Data de início é obrigatória")]
        public DateTime DataInicio { get; set; }
        
        [Required(ErrorMessage = "Data de término é obrigatória")]
        public DateTime DataTermino { get; set; }
        
        [Required(ErrorMessage = "Data de previsão de término é obrigatória")]
        public DateTime DataPrevisaoTermino { get; set; }
        
        [Required(ErrorMessage = "Plano é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Plano deve ser um valor válido")]
        public int Plano { get; set; }
    }
    
    public class LocacaoDto
    {
        public string Identificador { get; set; } = string.Empty;
        public string EntregadorId { get; set; } = string.Empty;
        public string MotoId { get; set; } = string.Empty;
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public DateTime DataPrevisaoTermino { get; set; }
        public DateTime? DataDevolucao { get; set; }
        public int Plano { get; set; }
        public decimal ValorDiaria { get; set; }
        public decimal? ValorTotal { get; set; }
    }
    
    public class DevolucaoDto
    {
        [Required(ErrorMessage = "Data de devolução é obrigatória")]
        public DateTime DataDevolucao { get; set; }
    }
    
    public class CalculoLocacaoDto
    {
        public decimal ValorTotal { get; set; }
        public DateTime DataDevolucao { get; set; }
        public string Observacoes { get; set; } = string.Empty;
    }
}

