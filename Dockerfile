#COMPILAÇÃO
#imagem do .NET Core 6 SDK para Ubuntu 20.04
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
#define o diretório de trabalho
WORKDIR /source
#copia os arquivos da raiz do projeto para o diretório de trabalho
COPY . .
#restaura as dependências
RUN dotnet restore "ProjetoFinal.csproj" --disable-parallel
#instala a ferramenta de migração do EF Core
RUN dotnet tool install --global dotnet-ef
#adiciona o diretório de ferramentas ao PATH
ENV PATH="${PATH}:/root/.dotnet/tools"
#cria uma migração do banco de dados
RUN dotnet ef migrations add final --project "ProjetoFinal.csproj"
#aplica a migração do banco de dados
RUN dotnet ef database update --project "ProjetoFinal.csproj"
#publica o aplicativo em modo release no diretório /app
RUN dotnet publish "ProjetoFinal.csproj" -c Release -o /app --no-restore

#PRODUÇÃO
#imagem do .NET Core 6 Runtime para Ubuntu 20.04
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS runtime
#define o diretório de trabalho
WORKDIR /app
#copia os arquivos do diretório /app da imagem de compilação
#para o diretório de trabalho da imagem de produção
COPY --from=build /app ./
#copia o arquivo do banco de dados para o diretório de trabalho
COPY --from=build /source/dados.db ./
#expõe a porta 5000 para o aplicativo
# EXPOSE 5000
#executa o aplicativo
ENTRYPOINT ["dotnet", "ProjetoFinal.dll"]

#para iniciar o serviço do docker
# sudo dockerd

#para buildar a imagem removendo imagens existentes
#com a tag maroquio/mvcwebapp:latest, sendo que maroquio
#é o usuário do docker hub
# docker build --rm -t maroquio/mvcwebapp:latest .

#para executar a imagem mapeando as portas internas 5000 e 5001 para as portas externas 5000 e 5001
#e setando as variáveis de ambiente ASPNETCORE_URLS e ASPNETCORE_HTTP_PORT
# docker run --rm -p 5000:5000 -p 5001:5001 -e ASPNETCORE_HTTP_PORT=https://+:5001 -e ASPNETCORE_URLS=http://+:5000 maroquio/mvcwebapp:latest
# docker run --rm -p 5010:5000 -p 5011:5001 -e ASPNETCORE_HTTP_PORT=https://+:5001 -e ASPNETCORE_URLS=http://+:5000 maroquio/mvcwebapp:latest

#para ver as imagens rodando
# docker ps

#para encerrar a imagem
# docker stop <container_id>

#para fazer login no docker hub
# docker login -u maroquio -p senha docker.io

#para enviar a imagem para o docker hub
# docker push maroquio/mvcwebapp:latest

#mais imagens do .NET em
# https://hub.docker.com/_/microsoft-dotnet-sdk/