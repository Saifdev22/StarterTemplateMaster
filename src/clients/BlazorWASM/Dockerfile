#Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS restore

# Expose the port the app will run on
EXPOSE 8080
EXPOSE 8081

ARG BUILD_CONFIGURATION=Release

# Set the working directory inside the container
WORKDIR /src

# Copy the project files and restore any dependencies
COPY ["./BlazorWASM/BlazorWASM.csproj", "clients/BlazorWASM/"]
# COPY ["../.././BuildingBlocks/BuildingBlocksClient/BuildingBlocksClient.csproj", "BuildingBlocks/BuildingBlocksClient/BuildingBlocksClient.csproj"]
RUN dotnet restore "./clients/BlazorWASM/BlazorWASM.csproj"

# Copy the remaining source code and build the application
FROM restore AS build
ARG BUILD_CONFIGURATION=Release
COPY . .
WORKDIR "clients/BlazorWASM/"
RUN dotnet build "./BlazorWASM.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the app to the /app/publish directory
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlazorWASM.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 2: Serve the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory for the app
WORKDIR /app

# Copy the published app from the build stage
COPY --from=publish /app/publish/wwwroot .

# Set the entry point to run the Blazor WASM app
ENTRYPOINT ["dotnet", "BlazorWASM.dll"]
