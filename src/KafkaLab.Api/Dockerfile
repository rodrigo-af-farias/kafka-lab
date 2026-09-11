FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/KafkaLab.Api/KafkaLab.Api.csproj", "src/KafkaLab.Api/"]
COPY ["src/KafkaLab.Application/KafkaLab.Application.csproj", "src/KafkaLab.Application/"]
COPY ["src/KafkaLab.Infrastructure/KafkaLab.Infrastructure.csproj", "src/KafkaLab.Infrastructure/"]

RUN dotnet restore "src/KafkaLab.Api/KafkaLab.Api.csproj"

COPY . .

WORKDIR "/src/src/KafkaLab.Api"

RUN dotnet publish "KafkaLab.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KafkaLab.Api.dll"]