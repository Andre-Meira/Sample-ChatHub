FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Worker/Sample.ChatHub.Worker.API/Sample.ChatHub.Worker.API.csproj", "src/Worker/Sample.ChatHub.Worker.API/"]
COPY ["src/core/Sample.ChatHub.Bus/Sample.ChatHub.Bus.csproj", "src/core/Sample.ChatHub.Bus/"]
COPY ["src/Worker/Sample.ChatHub.Core/Sample.ChatHub.Worker.Core.csproj", "src/Worker/Sample.ChatHub.Core/"]
COPY ["src/core/Sample.ChatHub.Domain.Abstracts/Sample.ChatHub.Domain.Abstracts.csproj", "src/core/Sample.ChatHub.Domain.Abstracts/"]
COPY ["src/core/Sample.ChatHub.Domain.Contracts/Sample.ChatHub.Domain.Contracts.csproj", "src/core/Sample.ChatHub.Domain.Contracts/"]
COPY ["src/Worker/Sample.ChatHub.Infrastructure/Sample.ChatHub.Worker.Infrastructure.csproj", "src/Worker/Sample.ChatHub.Infrastructure/"]

RUN dotnet restore "./src/Worker/Sample.ChatHub.Worker.API/Sample.ChatHub.Worker.API.csproj"
COPY . .
WORKDIR "src/Worker/Sample.ChatHub.Worker.API"
RUN dotnet build "./Sample.ChatHub.Worker.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sample.ChatHub.Worker.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.ChatHub.Worker.API.dll"]