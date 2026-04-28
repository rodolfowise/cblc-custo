# Multi-stage Dockerfile for MsCustoCblc microservice

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["ms-custo-cblc.sln", "."]
COPY ["src/MsCustoCblc.Domain/MsCustoCblc.Domain.csproj", "src/MsCustoCblc.Domain/"]
COPY ["src/MsCustoCblc.Application/MsCustoCblc.Application.csproj", "src/MsCustoCblc.Application/"]
COPY ["src/MsCustoCblc.Infrastructure/MsCustoCblc.Infrastructure.csproj", "src/MsCustoCblc.Infrastructure/"]
COPY ["src/MsCustoCblc.Presentation/MsCustoCblc.Presentation.csproj", "src/MsCustoCblc.Presentation/"]

# Restore dependencies
RUN dotnet restore "ms-custo-cblc.sln"

# Copy source code
COPY . .

# Build in Release mode
RUN dotnet build "ms-custo-cblc.sln" -c Release -o /app/build

# Publish to /app/publish
RUN dotnet publish "src/MsCustoCblc.Presentation/MsCustoCblc.Presentation.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published artifacts from build stage
COPY --from=build /app/publish .

# Expose port 5000
EXPOSE 5000

# Set environment to Production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5000

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "MsCustoCblc.Presentation.dll"]
