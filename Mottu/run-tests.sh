#!/bin/bash

echo "🚀 Iniciando testes do Mottu ..."

# Verificar se o Docker está rodando
if ! docker info > /dev/null 2>&1; then
    echo "❌ Docker não está rodando. Por favor, inicie o Docker primeiro."
    exit 1
fi

# Subir os serviços de infraestrutura
echo "📦 Subindo PostgreSQL e RabbitMQ..."
docker-compose up -d

# Aguardar os serviços ficarem prontos
echo "⏳ Aguardando serviços ficarem prontos..."
sleep 10

# Verificar se o PostgreSQL está pronto
until docker exec mottu_postgres pg_isready -U postgres > /dev/null 2>&1; do
    echo "⏳ Aguardando PostgreSQL..."
    sleep 2
done

echo "✅ PostgreSQL está pronto!"

# Verificar se o RabbitMQ está pronto
until docker exec mottu_rabbitmq rabbitmqctl status > /dev/null 2>&1; do
    echo "⏳ Aguardando RabbitMQ..."
    sleep 2
done

echo "✅ RabbitMQ está pronto!"

# Executar migrations
echo "🗄️ Executando migrations..."
cd Mottu.API
dotnet ef database update

if [ $? -eq 0 ]; then
    echo "✅ Migrations executadas com sucesso!"
else
    echo "❌ Erro ao executar migrations"
    exit 1
fi

# Compilar o projeto
echo "🔨 Compilando o projeto..."
cd ..
dotnet build

if [ $? -eq 0 ]; then
    echo "✅ Projeto compilado com sucesso!"
else
    echo "❌ Erro ao compilar o projeto"
    exit 1
fi

# Executar a aplicação em background
echo "🚀 Iniciando a aplicação..."
cd Mottu.API
dotnet run &
APP_PID=$!

# Aguardar a aplicação iniciar
echo "⏳ Aguardando aplicação iniciar..."
sleep 10

# Testar se a aplicação está respondendo
echo "🧪 Testando se a aplicação está respondendo..."
if curl -f http://localhost:5000/swagger > /dev/null 2>&1; then
    echo "✅ Aplicação está respondendo!"
    echo "📖 Swagger disponível em: http://localhost:5000/swagger"
    echo "🐰 RabbitMQ Management disponível em: http://localhost:15672 (guest/guest)"
    echo "🗄️ PostgreSQL disponível em: localhost:5432 (postgres/postgres)"
else
    echo "❌ Aplicação não está respondendo"
    kill $APP_PID
    exit 1
fi

echo ""
echo "🎉 Todos os testes passaram!"
echo "💡 Para parar a aplicação, pressione Ctrl+C"
echo "💡 Para parar os serviços: docker-compose down"

# Manter o script rodando
wait $APP_PID

