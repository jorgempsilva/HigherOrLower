# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
#COPY ["HigherOrLower/HigherOrLower.csproj", "HigherOrLower/"]
COPY HigherOrLower/HigherOrLower.csproj /app/HigherOrLower/
COPY Domain/Domain.csproj /app/Domain/
COPY Infrastructure/Infrastructure.csproj /app/Infrastructure/
COPY HigherOrLower /app/HigherOrLower
COPY Domain /app/Domain
COPY Infrastructure /app/Infrastructure
WORKDIR /app
RUN dotnet restore HigherOrLower/HigherOrLower.csproj
COPY . .
RUN dotnet build "HigherOrLower/HigherOrLower.csproj" -c $BUILD_CONFIGURATION -o /app/output

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HigherOrLower/HigherOrLower.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Scripts/apply-migrations.sh /app/Scripts/
COPY HigherOrLower /app/HigherOrLower
COPY Infrastructure /app/Infrastructure
COPY Domain /app/Domain
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"