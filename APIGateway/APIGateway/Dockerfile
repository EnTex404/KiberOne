# APIGateway/APIGateway/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

RUN apt-get update && apt-get install -y curl

# Копируем файл проекта
COPY ["APIGateway/APIGateway.csproj", "APIGateway/"]
RUN dotnet restore "APIGateway/APIGateway.csproj"

# Копируем весь код
COPY . .
RUN dotnet publish "APIGateway/APIGateway.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "APIGateway.dll"]