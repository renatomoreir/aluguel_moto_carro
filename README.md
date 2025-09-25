# Mottu  - Desafio 

Sistema de gerenciamento de aluguel de motos e entregadores desenvolvido em C# com .NET 8, PostgreSQL, RabbitMQ e API REST com documentação Swagger.

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação clara de responsabilidades:

- **Mottu.Domain**: Entidades de negócio, interfaces e regras de domínio
- **Mottu.Application**: Casos de uso, DTOs e interfaces de serviços
- **Mottu.Infrastructure**: Implementação de repositórios, banco de dados e mensageria
- **Mottu.API**: Controllers, configuração da API e documentação Swagger

## 🚀 Tecnologias Utilizadas

- **.NET 8**: Framework principal
- **ASP.NET Core**: API REST
- **Entity Framework Core**: ORM para acesso ao banco de dados
- **PostgreSQL**: Banco de dados relacional
- **RabbitMQ**: Sistema de mensageria
- **Swagger/OpenAPI**: Documentação da API
- **Docker**: Containerização dos serviços

## 📋 Funcionalidades

### Gestão de Motos
- ✅ Cadastrar nova moto
- ✅ Consultar motos (com filtro por placa)
- ✅ Consultar moto por identificador
- ✅ Modificar placa da moto
- ✅ Remover moto (apenas se não tiver locações)

### Gestão de Entregadores
- ✅ Cadastrar novo entregador
- ✅ Consultar entregador por identificador
- ✅ Enviar foto da CNH

### Gestão de Locações
- ✅ Alugar moto
- ✅ Consultar locação por identificador
- ✅ Informar data de devolução e calcular valor

### Sistema de Mensageria
- ✅ Publicação de eventos quando moto é cadastrada
- ✅ Consumo de eventos para motos do ano 2024
- ✅ Armazenamento de notificações no banco de dados

## 🛠️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started) (opcional, para PostgreSQL e RabbitMQ)
- [PostgreSQL](https://www.postgresql.org/download/) (se não usar Docker)
- [RabbitMQ](https://www.rabbitmq.com/download.html) (se não usar Docker)

## 🚀 Como Executar

### Opção 1: Com Docker (Recomendado)

1. **Clone o repositório**
```bash
git clone https://github.com/renatomoreir/aluguel_moto_carro
cd Mottu
```

2. **Suba os serviços de infraestrutura**
```bash
docker compose up -d
```

3. **Execute as migrations**
```bash
cd Mottu.API
dotnet ef database update
```

4. **Execute a aplicação**
```bash
dotnet run
```

### Opção 2: Sem Docker (Desenvolvimento)

1. **Configure o ambiente de desenvolvimento**
```bash Linux
# A aplicação usará banco em memória e RabbitMQ opcional
export ASPNETCORE_ENVIRONMENT=Development
```
```bash Windows
# A aplicação usará banco em memória e RabbitMQ opcional
setx ASPNETCORE_ENVIRONMENT "Development"
dotnet dev-certs https --trust
dotnet new tool-manifest
dotnet tool install --global dotnet-ef
dotnet tool update --global dotnet-ef
dotnet ef --version
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```
2. **Execute a aplicação**
```bash
cd Mottu.API
dotnet run
```

### Opção 3: Script Automatizado

```bash
# Para teste rápido sem dependências externas
./test-local.sh

# Para execução completa com Docker
./run-tests.sh
```

## 📖 Documentação da API

Após executar a aplicação, acesse:

- **Swagger UI**: http://localhost:5000/swagger
- **API Base URL**: http://localhost:5000

### Principais Endpoints

#### Motos
- `POST /motos` - Cadastrar moto
- `GET /motos` - Listar motos (com filtro opcional por placa)
- `GET /motos/{id}` - Consultar moto por ID
- `PUT /motos/{id}/placa` - Atualizar placa
- `DELETE /motos/{id}` - Remover moto

#### Entregadores
- `POST /entregadores` - Cadastrar entregador
- `GET /entregadores/{id}` - Consultar entregador por ID
- `POST /entregadores/{id}/cnh` - Enviar foto da CNH

#### Locações
- `POST /locacao` - Criar locação
- `GET /locacao/{id}` - Consultar locação por ID
- `PUT /locacao/{id}/devolucao` - Informar devolução

## 🗄️ Banco de Dados

### Estrutura das Tabelas

- **Motos**: Identificador, Ano, Modelo, Placa
- **Entregadores**: Identificador, Nome, CNPJ, Data Nascimento, CNH
- **Locacoes**: Identificador, Entregador, Moto, Datas, Valores
- **NotificacaoMotos**: Notificações de motos 2024

### Migrations

```bash
# Criar nova migration
dotnet ef migrations add NomeDaMigration

# Aplicar migrations
dotnet ef database update

# Reverter migration
dotnet ef database update PreviousMigrationName
```

## 🐰 RabbitMQ

### Configuração

- **Exchange**: `mottu.exchange` (Topic)
- **Queue**: `moto.2024.queue`
- **Routing Key**: `moto.cadastrada`

### Fluxo de Mensagens

1. Quando uma moto é cadastrada → Publica evento `moto.cadastrada`
2. Consumer processa eventos → Filtra motos do ano 2024
3. Salva notificação no banco → Tabela `NotificacaoMotos`

## 🧪 Testes

### Executar Testes

```bash
# Testes unitários
dotnet test

# Teste de compilação e execução
./test-local.sh

# Teste completo com infraestrutura
./run-tests.sh
```

### Configuração de Teste

O projeto suporta dois modos:

1. **Produção**: PostgreSQL + RabbitMQ
2. **Desenvolvimento**: InMemory Database + RabbitMQ Opcional

## 📁 Estrutura do Projeto

```
Mottu/
├── Mottu.API/                 # Controllers e configuração da API
├── Mottu.Application/         # Serviços e DTOs
├── Mottu.Domain/              # Entidades e interfaces
├── Mottu.Infrastructure/      # Repositórios e infraestrutura
├── docker-compose.yml         # Configuração Docker
├── run-tests.sh               # Script de teste completo
├── test-local.sh              # Script de teste local
└── README.md                  # Este arquivo
```

## 🔧 Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=mottu_db;Username=postgres;Password=postgres",
    "RabbitMQ": "localhost"
  }
}
```

### appsettings.Development.json

```json
{
  "Features": {
    "EnableRabbitMQ": false,
    "EnableDatabase": false
  }
}
```

## 🚨 Regras de Negócio

### Motos
- Placa deve ser única
- Não pode remover moto com locações ativas

### Entregadores
- CNPJ deve ser único
- Número da CNH deve ser único
- Apenas CNH categoria A ou AB podem alugar motos

### Locações
- Entregador deve ter CNH categoria A ou AB
- Não pode ter locação ativa simultânea
- Moto não pode estar alugada
- Planos disponíveis: 7, 15, 30, 45, 50 dias
- Cálculo de multas por devolução antecipada
- Taxa adicional por atraso na devolução

## 🐛 Troubleshooting

### Problemas Comuns

1. **Erro de conexão com PostgreSQL**
   - Verifique se o PostgreSQL está rodando
   - Confirme as credenciais no appsettings.json

2. **Erro de conexão com RabbitMQ**
   - Use o modo Development para desabilitar RabbitMQ
   - Verifique se o RabbitMQ está rodando na porta 5672

3. **Erro de migration**
   - Execute `dotnet ef database update`
   - Verifique a connection string

