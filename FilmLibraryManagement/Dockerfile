# Base image for running the app (ASP.NET runtime image)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use an SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY FilmLibraryManagement.csproj ./FilmLibraryManagement/
WORKDIR /src/FilmLibraryManagement
RUN dotnet restore "./FilmLibraryManagement.csproj"

# Copy the entire project and build
COPY . ./
RUN dotnet publish "./FilmLibraryManagement.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: production-ready image
FROM base AS final
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Run the application
ENTRYPOINT ["dotnet", "FilmLibraryManagement.dll"]
