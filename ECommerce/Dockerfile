# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . ./
RUN dotnet restore "./ECommerce.csproj"
#WORKDIR "/src/ECommerce"
RUN dotnet build "./ECommerce.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ECommerce.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./

# Create the Data directory
RUN mkdir -p /app/Data


# Create a non-root user and group
#RUN addgroup -S app && adduser -S app -G app

# Switch to the non-root user
#USER app

# Set the entry point
ENTRYPOINT ["dotnet", "ECommerce.dll"]
