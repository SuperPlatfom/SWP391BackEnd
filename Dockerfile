# ==================== Base runtime ====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ==================== Build environment ====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
ENV BUILD_CONFIGURATION=Release

# Copy all project files
COPY SWP391BackEnd/*.csproj SWP391BackEnd/
COPY BusinessObject/*.csproj BusinessObject/
COPY DataAccessLayer/*.csproj DataAccessLayer/
COPY Repository/*.csproj Repository/
COPY Service/*.csproj Service/

# Restore
WORKDIR /src/SWP391BackEnd
RUN dotnet restore

# Copy all source code
COPY . .

# Build & publish
RUN dotnet build -c $BUILD_CONFIGURATION /p:UseAppHost=false
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==================== Final stage ====================
FROM base AS final
WORKDIR /app
RUN apt-get update && apt-get install -y \
    libgdiplus \
    fontconfig \
    wkhtmltopdf 
COPY Service/Libs /app/Service/Libs
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SWP391BackEnd.dll"]
