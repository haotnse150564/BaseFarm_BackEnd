# Stage 1: Build application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /API

# Copy solution và project files
COPY BaseFarm_BackEnd.sln ./
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY WebAPI/*.csproj ./WebAPI/

# Restore dependencies
RUN dotnet restore WebAPI/WebAPI.csproj

# Copy source code
COPY . .

# Build Release (bật TieredCompilation để giảm RAM)
RUN dotnet build WebAPI/WebAPI.csproj -c Release -o /app/build /p:TieredCompilation=true

# Stage 2: Publish
FROM build AS publish

# Publish Release đơn giản, KHÔNG ReadyToRun, KHÔNG Trimmed, KHÔNG --no-restore
RUN dotnet publish WebAPI/WebAPI.csproj \
    -c Release \
    -o /app/publish

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY --from=publish /app/publish .

# Render dùng port động
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

EXPOSE 8080

ENTRYPOINT ["dotnet", "WebAPI.dll"]