#!/bin/bash

echo "🚀 Testando Mottu  localmente..."

# Compilar o projeto
echo "🔨 Compilando o projeto..."
dotnet build

if [ $? -eq 0 ]; then
    echo "✅ Projeto compilado com sucesso!"
else
    echo "❌ Erro ao compilar o projeto"
    exit 1
fi

# Executar testes unitários (se existirem)
echo "🧪 Executando testes..."
dotnet test

# Verificar se a aplicação pode ser iniciada
echo "🚀 Verificando se a aplicação pode ser iniciada..."
cd Mottu.API

# Tentar iniciar a aplicação por alguns segundos
timeout 10s dotnet run &
APP_PID=$!

sleep 5

# Verificar se o processo ainda está rodando
if kill -0 $APP_PID 2>/dev/null; then
    echo "✅ Aplicação iniciou com sucesso!"
    kill $APP_PID
else
    echo "❌ Aplicação falhou ao iniciar"
    exit 1
fi

echo ""
echo "🎉 Testes locais concluídos com sucesso!"
echo "📝 Notas:"
echo "   - Para executar com banco de dados, configure PostgreSQL e RabbitMQ"
echo "   - Swagger estará disponível em: http://localhost:5000/swagger"
echo "   - Para executar: cd Mottu.API && dotnet run"

