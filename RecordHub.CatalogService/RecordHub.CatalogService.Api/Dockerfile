#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RecordHub.CatalogService/RecordHub.CatalogService.Api/RecordHub.CatalogService.Api.csproj", "RecordHub.CatalogService/RecordHub.CatalogService.Api/"]
COPY ["RecordHub.CatalogService/RecordHub.CatalogService.Application/RecordHub.CatalogService.Application.csproj", "RecordHub.CatalogService/RecordHub.CatalogService.Application/"]
COPY ["RecordHub.CatalogService/RecordHub.CatalogService.Domain/RecordHub.CatalogService.Domain.csproj", "RecordHub.CatalogService/RecordHub.CatalogService.Domain/"]
COPY ["RecordHub.CatalogService/RecordHub.CatalogService.Infrastructure/RecordHub.CatalogService.Infrastructure.csproj", "RecordHub.CatalogService/RecordHub.CatalogService.Infrastructure/"]
RUN dotnet restore "RecordHub.CatalogService/RecordHub.CatalogService.Api/RecordHub.CatalogService.Api.csproj"
COPY . .
WORKDIR "/src/RecordHub.CatalogService/RecordHub.CatalogService.Api"
RUN dotnet build "RecordHub.CatalogService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecordHub.CatalogService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecordHub.CatalogService.Api.dll"]