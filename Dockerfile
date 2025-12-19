# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY Domain/Domain.csproj Domain/
COPY DataAccess/DataAccess.csproj DataAccess/
COPY SocialNetworkBe/SocialNetworkBe.csproj SocialNetworkBe/

# Restore dependencies
RUN dotnet restore SocialNetworkBe/SocialNetworkBe.csproj

# Copy source code
COPY . .

# Build and publish
WORKDIR /src/SocialNetworkBe
RUN dotnet publish -c Release -o /app/publish

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser

# Copy published files
COPY --from=build /app/publish .

# Change ownership and switch to non-root user
RUN chown -R appuser:appuser /app
USER appuser

# Expose port (Railway, Render, etc. will set PORT env variable)
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Start application
ENTRYPOINT ["dotnet", "SocialNetworkBe.dll"]
