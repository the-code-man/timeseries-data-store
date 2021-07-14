#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/DataIngestor/DataIngestor.csproj", "src/DataIngestor/"]
COPY ["core/TimeSeries.ServiceBus.Common/TimeSeries.ServiceBus.Common.csproj", "core/TimeSeries.ServiceBus.Common/"]
COPY ["core/TimeSeries.Shared.Contracts/TimeSeries.Shared.Contracts.csproj", "core/TimeSeries.Shared.Contracts/"]
COPY ["core/TimeSeries.DataStore.Raw/TimeSeries.DataStore.Raw.csproj", "core/TimeSeries.DataStore.Raw/"]
RUN dotnet restore "src/DataIngestor/DataIngestor.csproj"
COPY . .
WORKDIR "/src/src/DataIngestor"
RUN dotnet build "DataIngestor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DataIngestor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DataIngestor.dll"]
