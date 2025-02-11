# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install make
RUN apt-get update && apt-get install -y make && rm -rf /var/lib/apt/lists/*

# Set working directory inside the container
WORKDIR /app

# Copy solution and restore dependencies
COPY ["Homespirations.Api/Homespirations.Api.csproj", "Homespirations.Api/"]
COPY ["Homespirations.Application/Homespirations.Application.csproj", "Homespirations.Application/"]
COPY ["Homespirations.Core/Homespirations.Core.csproj", "Homespirations.Core/"]
COPY ["Homespirations.Infrastructure/Homespirations.Infrastructure.csproj", "Homespirations.Infrastructure/"]

RUN dotnet restore "Homespirations.Api/Homespirations.Api.csproj"

# Copy the entire application source code
COPY . .

# Build the application
RUN dotnet publish "Homespirations.Api/Homespirations.Api.csproj" -c Release -o /app/publish

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/publish .

# Expose the port Fly.io will route traffic to
ENV ASPNETCORE_URLS=http://+:5000

# Default environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

# Command to run the application
ENTRYPOINT ["dotnet", "Homespirations.Api.dll"]
