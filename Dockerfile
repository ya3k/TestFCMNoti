# Use the official .NET 8 SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY *.sln .
COPY Api/Api.csproj Api/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/

# Copy the rest of the files
COPY Api/. ./Api
COPY Infrastructure/. ./Infrastructure
COPY Application/. ./Application
COPY Domain/. ./Domain

# Restore dependencies
RUN dotnet restore

# Build and publish the app
RUN dotnet publish Api/Api.csproj -c Release -o /app/publish

# Use the official .NET 8 runtime image for the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "Api.dll"]
