#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5279
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Server/Sample.ChatHub.Server.API/Sample.ChatHub.Server.API.csproj", "src/Server/Sample.ChatHub.Server.API/"]
COPY ["src/core/Sample.ChatHub.Bus/Sample.ChatHub.Bus.csproj", "src/core/Sample.ChatHub.Bus/"]
COPY ["src/core/Sample.ChatHub.Domain.Contracts/Sample.ChatHub.Domain.Contracts.csproj", "src/core/Sample.ChatHub.Domain.Contracts/"]
COPY ["src/core/Sample.ChatHub.Domain.Abstracts/Sample.ChatHub.Domain.Abstracts.csproj", "src/core/Sample.ChatHub.Domain.Abstracts/"]
RUN dotnet restore "./src/Server/Sample.ChatHub.Server.API/Sample.ChatHub.Server.API.csproj"
COPY . .
WORKDIR "/src/src/Server/Sample.ChatHub.Server.API"
RUN dotnet build "./Sample.ChatHub.Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sample.ChatHub.Server.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.ChatHub.Server.API.dll"]