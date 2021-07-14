#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/TimeSeries.Calculator.Min/TimeSeries.Calculator.Min.csproj", "src/TimeSeries.Calculator.Min/"]
COPY ["core/TimeSeries.gRPC.Server/TimeSeries.gRPC.Server.csproj", "core/TimeSeries.gRPC.Server/"]
COPY ["core/TimeSeries.Shared.Contracts/TimeSeries.Shared.Contracts.csproj", "core/TimeSeries.Shared.Contracts/"]
COPY ["core/TimeSeries.ServiceBus.Common/TimeSeries.ServiceBus.Common.csproj", "core/TimeSeries.ServiceBus.Common/"]
COPY ["core/TimeSeries.DataStore.Aggr/TimeSeries.DataStore.Aggr.csproj", "core/TimeSeries.DataStore.Aggr/"]
RUN dotnet restore "src/TimeSeries.Calculator.Min/TimeSeries.Calculator.Min.csproj"
COPY . .
WORKDIR "/src/src/TimeSeries.Calculator.Min"
RUN dotnet build "TimeSeries.Calculator.Min.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeSeries.Calculator.Min.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeSeries.Calculator.Min.dll"]
