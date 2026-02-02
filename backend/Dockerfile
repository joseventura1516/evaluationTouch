# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY InventarioSystem.Core/InventarioSystem.Core.csproj InventarioSystem.Core/
COPY InventarioSystem.Infrastructure/InventarioSystem.Infrastructure.csproj InventarioSystem.Infrastructure/
COPY InventarioSystem.API/InventarioSystem.API.csproj InventarioSystem.API/

RUN dotnet restore InventarioSystem.API/InventarioSystem.API.csproj

# Copy source code and build
COPY . .
RUN dotnet build InventarioSystem.API/InventarioSystem.API.csproj -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish InventarioSystem.API/InventarioSystem.API.csproj -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

COPY --from=publish /app/publish .

# Create non-root user for security
RUN useradd -m appuser && chown -R appuser:appuser /app
USER appuser

EXPOSE 5000

HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/swagger/index.html || exit 1

ENTRYPOINT ["dotnet", "InventarioSystem.API.dll"]
