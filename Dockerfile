# Stage 1: Build application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /API

# Copy solution và project files
COPY BaseFarm_BackEnd.sln ./
COPY Application/*.csproj ./Application/
COPY Domain/*.csproj ./Domain/
COPY Infrastructure/*.csproj ./Infrastructure/
COPY WebAPI/*.csproj ./WebAPI/

# Restore dependencies (không cần --no-restore nữa)
RUN dotnet restore WebAPI/WebAPI.csproj

# Copy toàn bộ source code
COPY . .

# Build ở Release mode và bật các option tối ưu
RUN dotnet build WebAPI/WebAPI.csproj -c Release -o /app/build /p:TieredCompilation=true

# Stage 2: Publish
FROM build AS publish

# Publish ở Release, bật ReadyToRun và Trimmed để giảm RAM + startup nhanh
RUN dotnet publish WebAPI/WebAPI.csproj \
    -c Release \
    -o /app/publish \
    /p:PublishReadyToRun=true \
    /p:PublishTrimmed=true \
    /p:TieredCompilation=true \
    --self-contained false \
    --no-restore

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app
COPY --from=publish /app/publish .

# Expose port (Render sẽ tự map, nhưng giữ lại để rõ ràng)
EXPOSE 8080
# Hoặc nếu bạn dùng port khác trong code, đổi thành 5255 hoặc $PORT

# Render khuyến nghị dùng $PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:$PORT

ENTRYPOINT ["dotnet", "WebAPI.dll"]