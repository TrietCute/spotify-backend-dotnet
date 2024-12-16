# Sử dụng hình ảnh SDK .NET chính thức để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy file csproj và khôi phục các dependencies
COPY backend_spotify/*.csproj ./  
RUN dotnet restore

# Copy toàn bộ mã nguồn và build ứng dụng
COPY ./backend_spotify ./  
RUN dotnet publish -c Release -o out

# Sử dụng hình ảnh runtime nhỏ hơn để chạy ứng dụng
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Mở port mà ứng dụng sẽ chạy
EXPOSE 5025

# Thiết lập entry point cho container
ENTRYPOINT ["dotnet", "backend_spotify.dll"]
