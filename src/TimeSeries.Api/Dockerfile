#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/TimeSeries.Api/TimeSeries.Api.csproj", "src/TimeSeries.Api/"]
COPY ["core/TimeSeries.ServiceBus.Common/TimeSeries.ServiceBus.Common.csproj", "core/TimeSeries.ServiceBus.Common/"]
COPY ["core/TimeSeries.Shared.Contracts/TimeSeries.Shared.Contracts.csproj", "core/TimeSeries.Shared.Contracts/"]
COPY ["core/TimeSeries.gRPC.Client/TimeSeries.gRPC.Client.csproj", "core/TimeSeries.gRPC.Client/"]
COPY ["core/TimeSeries.DataStore.Raw/TimeSeries.DataStore.Raw.csproj", "core/TimeSeries.DataStore.Raw/"]
RUN dotnet restore "src/TimeSeries.Api/TimeSeries.Api.csproj"
COPY . .
WORKDIR "/src/src/TimeSeries.Api"
RUN dotnet build "TimeSeries.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TimeSeries.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimeSeries.Api.dll"]
