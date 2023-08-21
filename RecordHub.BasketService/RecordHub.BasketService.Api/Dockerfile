#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RecordHub.BasketService/RecordHub.BasketService.Api/RecordHub.BasketService.Api.csproj", "RecordHub.BasketService/RecordHub.BasketService.Api/"]
COPY ["RecordHub.Shared/RecordHub.Shared.csproj", "RecordHub.Shared/"]
COPY ["RecordHub.BasketService/RecordHub.BasketService.Application/RecordHub.BasketService.Application.csproj", "RecordHub.BasketService/RecordHub.BasketService.Application/"]
COPY ["RecordHub.BasketService/RecordHub.BasketService.Domain/RecordHub.BasketService.Domain.csproj", "RecordHub.BasketService/RecordHub.BasketService.Domain/"]
COPY ["RecordHub.BasketService/RecordHub.BasketService.Infrastructure/RecordHub.BasketService.Infrastructure.csproj", "RecordHub.BasketService/RecordHub.BasketService.Infrastructure/"]
RUN dotnet restore "RecordHub.BasketService/RecordHub.BasketService.Api/RecordHub.BasketService.Api.csproj"
COPY . .
WORKDIR "/src/RecordHub.BasketService/RecordHub.BasketService.Api"
RUN dotnet build "RecordHub.BasketService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecordHub.BasketService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecordHub.BasketService.Api.dll"]