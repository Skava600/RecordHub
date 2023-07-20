#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RecordHub.OrderingService/RecordHub.OrderingService.Api/RecordHub.OrderingService.Api.csproj", "RecordHub.OrderingService/RecordHub.OrderingService.Api/"]
COPY ["RecordHub.OrderingService/RecordHub.OrderingService.Application/RecordHub.OrderingService.Application.csproj", "RecordHub.OrderingService/RecordHub.OrderingService.Application/"]
COPY ["RecordHub.OrderingService/RecordHub.OrderingService.Domain/RecordHub.OrderingService.Domain.csproj", "RecordHub.OrderingService/RecordHub.OrderingService.Domain/"]
COPY ["RecordHub.OrderingService/RecordHub.OrderingService.Infrastructure/RecordHub.OrderingService.Infrastructure.csproj", "RecordHub.OrderingService/RecordHub.OrderingService.Infrastructure/"]
COPY ["RecordHub.Shared/RecordHub.Shared.csproj", "RecordHub.Shared/"]
RUN dotnet restore "RecordHub.OrderingService/RecordHub.OrderingService.Api/RecordHub.OrderingService.Api.csproj"
COPY . .
WORKDIR "/src/RecordHub.OrderingService/RecordHub.OrderingService.Api"
RUN dotnet build "RecordHub.OrderingService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecordHub.OrderingService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecordHub.OrderingService.Api.dll"]