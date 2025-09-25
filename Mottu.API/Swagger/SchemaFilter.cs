using Microsoft.OpenApi.Models;
using Mottu.Application.DTOs;
using Mottu.Domain.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Mottu.API.Swagger
{
    public class SchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(CriarMotoDto))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["identificador"] = new Microsoft.OpenApi.Any.OpenApiString("moto-001"),
                    ["ano"] = new Microsoft.OpenApi.Any.OpenApiInteger(2024),
                    ["modelo"] = new Microsoft.OpenApi.Any.OpenApiString("Honda CG 160"),
                    ["placa"] = new Microsoft.OpenApi.Any.OpenApiString("ABC1234")
                };
            }
            else if (context.Type == typeof(MotoDto))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["identificador"] = new Microsoft.OpenApi.Any.OpenApiString("moto-001"),
                    ["ano"] = new Microsoft.OpenApi.Any.OpenApiInteger(2024),
                    ["modelo"] = new Microsoft.OpenApi.Any.OpenApiString("Honda CG 160"),
                    ["placa"] = new Microsoft.OpenApi.Any.OpenApiString("ABC1234")
                };
            }
            else if (context.Type == typeof(CriarEntregadorDto))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["identificador"] = new Microsoft.OpenApi.Any.OpenApiString("entregador-001"),
                    ["nome"] = new Microsoft.OpenApi.Any.OpenApiString("João Silva"),
                    ["cnpj"] = new Microsoft.OpenApi.Any.OpenApiString("12345678000199"),
                    ["dataNascimento"] = new Microsoft.OpenApi.Any.OpenApiString("1990-05-15"),
                    ["numeroCnh"] = new Microsoft.OpenApi.Any.OpenApiString("12345678901"),
                    ["tipoCnh"] = new Microsoft.OpenApi.Any.OpenApiInteger((int)TipoCnh.A),
                    ["imagemCnh"] = new Microsoft.OpenApi.Any.OpenApiString("path/to/cnh/image.png")
                };
            }
            else if (context.Type == typeof(CriarLocacaoDto))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["identificador"] = new Microsoft.OpenApi.Any.OpenApiString("locacao-001"),
                    ["entregadorId"] = new Microsoft.OpenApi.Any.OpenApiString("entregador-001"),
                    ["motoId"] = new Microsoft.OpenApi.Any.OpenApiString("moto-001"),
                    ["dataInicio"] = new Microsoft.OpenApi.Any.OpenApiString("2024-01-15"),
                    ["dataTermino"] = new Microsoft.OpenApi.Any.OpenApiString("2024-01-22"),
                    ["dataPrevisaoTermino"] = new Microsoft.OpenApi.Any.OpenApiString("2024-01-22"),
                    ["plano"] = new Microsoft.OpenApi.Any.OpenApiInteger(7)
                };
            }
            else if (context.Type == typeof(DevolucaoDto))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["dataDevolucao"] = new Microsoft.OpenApi.Any.OpenApiString("2024-01-20")
                };
            }
            else if (context.Type == typeof(AtualizarPlacaMotoDto))
            {
                schema.Example = new Microsoft.OpenApi.Any.OpenApiObject
                {
                    ["placa"] = new Microsoft.OpenApi.Any.OpenApiString("XYZ9876")
                };
            }
        }
    }
}

