# Base image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage using SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the .csproj file and restore dependencies
COPY ./FilmLibraryManagement.csproj ./FilmLibraryManagement/
WORKDIR /src/FilmLibraryManagement
RUN dotnet restore

# Copy the rest of the project files
COPY . ./

# Publish the app
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage to run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "FilmLibraryManagement.dll"]
