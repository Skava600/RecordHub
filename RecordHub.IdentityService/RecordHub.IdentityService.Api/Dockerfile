#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RecordHub.IdentityService/RecordHub.IdentityService.Api/RecordHub.IdentityService.Api.csproj", "RecordHub.IdentityService/RecordHub.IdentityService.Api/"]
COPY ["RecordHub.IdentityService/RecordHub.IdentityService.Core/RecordHub.IdentityService.Core.csproj", "RecordHub.IdentityService/RecordHub.IdentityService.Core/"]
COPY ["RecordHub.IdentityService/RecordHub.IdentityService.Domain/RecordHub.IdentityService.Domain.csproj", "RecordHub.IdentityService/RecordHub.IdentityService.Domain/"]
COPY ["RecordHub.IdentityService/RecordHub.IdentityService.Infrastructure/RecordHub.IdentityService.Infrastructure.csproj", "RecordHub.IdentityService/RecordHub.IdentityService.Infrastructure/"]
COPY ["RecordHub.IdentityService/RecordHub.IdentityService.Persistence/RecordHub.IdentityService.Persistence.csproj", "RecordHub.IdentityService/RecordHub.IdentityService.Persistence/"]
RUN dotnet restore "RecordHub.IdentityService/RecordHub.IdentityService.Api/RecordHub.IdentityService.Api.csproj"
COPY . .
WORKDIR "/src/RecordHub.IdentityService/RecordHub.IdentityService.Api"
RUN dotnet build "RecordHub.IdentityService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecordHub.IdentityService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecordHub.IdentityService.Api.dll"]