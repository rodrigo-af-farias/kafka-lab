FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/KafkaLab.Consumer/KafkaLab.Consumer.csproj", "src/KafkaLab.Consumer/"]
COPY ["src/KafkaLab.Application/KafkaLab.Application.csproj", "src/KafkaLab.Application/"]
COPY ["src/KafkaLab.Infrastructure/KafkaLab.Infrastructure.csproj", "src/KafkaLab.Infrastructure/"]

RUN dotnet restore "src/KafkaLab.Consumer/KafkaLab.Consumer.csproj"

COPY . .

WORKDIR "/src/src/KafkaLab.Consumer"

RUN dotnet publish "KafkaLab.Consumer.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "KafkaLab.Consumer.dll"]