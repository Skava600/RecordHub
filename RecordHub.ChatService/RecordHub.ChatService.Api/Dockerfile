#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RecordHub.ChatService/RecordHub.ChatService.Api/RecordHub.ChatService.Api.csproj", "RecordHub.ChatService/RecordHub.ChatService.Api/"]
COPY ["RecordHub.BasketService/RecordHub.BasketService.Applicatation/RecordHub.BasketService.Applicatation.csproj", "RecordHub.BasketService/RecordHub.BasketService.Applicatation/"]
COPY ["RecordHub.Shared/RecordHub.Shared.csproj", "RecordHub.Shared/"]
COPY ["RecordHub.BasketService/RecordHub.BasketService.Domain/RecordHub.BasketService.Domain.csproj", "RecordHub.BasketService/RecordHub.BasketService.Domain/"]
COPY ["RecordHub.BasketService/RecordHub.BasketService.Infrastructure/RecordHub.BasketService.Infrastructure.csproj", "RecordHub.BasketService/RecordHub.BasketService.Infrastructure/"]
COPY ["RecordHub.ChatService/RecordHub.ChatService.Infrastructure/RecordHub.ChatService.Infrastructure.csproj", "RecordHub.ChatService/RecordHub.ChatService.Infrastructure/"]
RUN dotnet restore "RecordHub.ChatService/RecordHub.ChatService.Api/RecordHub.ChatService.Api.csproj"
COPY . .
WORKDIR "/src/RecordHub.ChatService/RecordHub.ChatService.Api"
RUN dotnet build "RecordHub.ChatService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecordHub.ChatService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecordHub.ChatService.Api.dll"]