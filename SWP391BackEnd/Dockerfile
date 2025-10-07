# ==================== Base runtime image ====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ==================== Build environment ====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ENV BUILD_CONFIGURATION=Release

# Copy all project files from upper level
COPY ../BusinessObject/*.csproj BusinessObject/
COPY ../DataAccessLayer/*.csproj DataAccessLayer/
COPY ../Repository/*.csproj Repository/
COPY ../Service/*.csproj Service/
COPY *.csproj ./

# Restore dependencies
RUN dotnet restore

# Copy the entire source tree (from root level)
COPY .. .

# Build & publish app
RUN dotnet build -c $BUILD_CONFIGURATION /p:UseAppHost=false
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==================== Final runtime image ====================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SWP391BackEnd.dll"]
