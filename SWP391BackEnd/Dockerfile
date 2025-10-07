# ==================== Base runtime image ====================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ==================== Build environment ====================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV BUILD_CONFIGURATION=Release

# Làm việc ở root của source code
WORKDIR /src/SWP391BackEnd

# Copy project file của backend và các project phụ thuộc
COPY ../BusinessObject/*.csproj ../BusinessObject/
COPY ../DataAccessLayer/*.csproj ../DataAccessLayer/
COPY ../Repository/*.csproj ../Repository/
COPY ../Service/*.csproj ../Service/
COPY *.csproj ./

# Restore các package
RUN dotnet restore

# Copy toàn bộ source code (bao gồm project phụ)
WORKDIR /src
COPY .. .

# Build và publish project backend
WORKDIR /src/SWP391BackEnd
RUN dotnet build -c $BUILD_CONFIGURATION /p:UseAppHost=false
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ==================== Final runtime image ====================
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SWP391BackEnd.dll"]
