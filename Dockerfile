# Stage 1: Build application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /API

# Copy solution and project files
COPY BaseFarm_BackEnd.sln ./
COPY Application/*.csproj Application/
COPY Domain/*.csproj Domain/
COPY Infrastructure/*.csproj Infrastructure/
COPY WebAPI/*.csproj WebAPI/
COPY UnitTest/BaseFarm_BackEnd.Test/*.csproj UnitTest/BaseFarm_BackEnd.Test/

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build in Debug mode for development
RUN dotnet build BaseFarm_BackEnd.sln -c Debug -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish WebAPI/WebAPI.csproj -c Debug -o /app/publish --no-restore

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5255
ENTRYPOINT ["dotnet", "WebAPI.dll"]