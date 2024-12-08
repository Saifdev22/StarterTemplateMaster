# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["src/api/Starter.API/Starter.API.csproj", "src/api/Starter.API/"]
COPY ["src/modules/inventory/Inventory.Infrastructure/Inventory.Infrastructure.csproj", "src/modules/inventory/Inventory.Infrastructure/"]
COPY ["src/common/Common.Infrastructure/Common.Infrastructure.csproj", "src/common/Common.Infrastructure/"]
COPY ["src/common/Common.Application/Common.Application.csproj", "src/common/Common.Application/"]
COPY ["src/common/Common.Domain/Common.Domain.csproj", "src/common/Common.Domain/"]
COPY ["src/modules/system/System.Application/System.Application.csproj", "src/modules/system/System.Application/"]
COPY ["src/modules/system/System.Domain/System.Domain.csproj", "src/modules/system/System.Domain/"]
COPY ["src/modules/system/System.IntegrationEvents/System.IntegrationEvents.csproj", "src/modules/system/System.IntegrationEvents/"]
COPY ["src/modules/inventory/Inventory.Application/Inventory.Application.csproj", "src/modules/inventory/Inventory.Application/"]
COPY ["src/modules/inventory/Inventory.Domain/Inventory.Domain.csproj", "src/modules/inventory/Inventory.Domain/"]
COPY ["src/modules/inventory/Inventory.IntegrationEvents/Inventory.IntegrationEvents.csproj", "src/modules/inventory/Inventory.IntegrationEvents/"]
COPY ["src/modules/inventory/Inventory.Presentation/Inventory.Presentation.csproj", "src/modules/inventory/Inventory.Presentation/"]
COPY ["src/common/Common.Presentation/Common.Presentation.csproj", "src/common/Common.Presentation/"]
COPY ["src/modules/system/System.Infrastructure/System.Infrastructure.csproj", "src/modules/system/System.Infrastructure/"]
COPY ["src/modules/system/System.Presentation/System.Presentation.csproj", "src/modules/system/System.Presentation/"]
RUN dotnet restore "./src/api/Starter.API/Starter.API.csproj"
COPY . .
WORKDIR "/src/src/api/Starter.API"
RUN dotnet build "./Starter.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Starter.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mrdir -p /app/certificates
COPY solution-items/certificates/aspnetapp.pfx /app/certificates/

ENTRYPOINT ["dotnet", "Starter.API.dll"]