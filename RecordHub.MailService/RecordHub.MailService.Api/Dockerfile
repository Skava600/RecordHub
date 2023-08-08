#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["RecordHub.MailService/RecordHub.MailService.Api/RecordHub.MailService.Api.csproj", "RecordHub.MailService/RecordHub.MailService.Api/"]
COPY ["RecordHub.MailService/RecordHub.MailService.Infrastructure/RecordHub.MailService.Infrastructure.csproj", "RecordHub.MailService/RecordHub.MailService.Infrastructure/"]
COPY ["RecordHub.MailService/RecordHub.MailService.Application/RecordHub.MailService.Application.csproj", "RecordHub.MailService/RecordHub.MailService.Application/"]
RUN dotnet restore "RecordHub.MailService/RecordHub.MailService.Api/RecordHub.MailService.Api.csproj"
COPY . .
WORKDIR "/src/RecordHub.MailService/RecordHub.MailService.Api"
RUN dotnet build "RecordHub.MailService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RecordHub.MailService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RecordHub.MailService.Api.dll"]