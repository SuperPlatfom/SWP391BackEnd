# ---------- Base runtime image ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ---------- Build environment ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ENV BUILD_CONFIGURATION=Release

# Copy solution and project files
COPY SWP391BackEnd/*.csproj SWP391BackEnd/
COPY BusinessObject/*.csproj BusinessObject/
COPY Service/*.csproj Service/
COPY DataAccessLayer/*.csproj DataAccessLayer/
COPY Repository/*.csproj Repository/

# Restore dependencies
WORKDIR /src/SWP391BackEnd
RUN dotnet restore

# Copy the full source code
COPY . .

# Build the project (disable AppHost to avoid conflicts)
RUN dotnet build --no-restore -c $BUILD_CONFIGURATION /p:UseAppHost=false

# Publish the application
RUN dotnet publish --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ---------- Final runtime image ----------
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Copy extra static folder if needed (optional)
# COPY SWP391BackEnd/app/.well-known /app/app/.well-known

ENTRYPOINT ["dotnet", "SWP391BackEnd.dll"]
