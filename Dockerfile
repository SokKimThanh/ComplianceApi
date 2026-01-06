# Giai do?n 1: Build ?ng d?ng
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy file project và restore các dependencies [cite: 1, 4]
COPY ["ComplianceApi.csproj", "."]
RUN dotnet restore "ComplianceApi.csproj"

# Copy toàn b? code và build
COPY . .
RUN dotnet build "ComplianceApi.csproj" -c Release -o /app/build

# Giai do?n 2: Publish ?ng d?ng
FROM build AS publish
RUN dotnet publish "ComplianceApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai do?n 3: Ch?y ?ng d?ng (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

# T?o thu m?c luu tr? file n?u chua t?n t?i [cite: 53, 54]
RUN mkdir -p /app/InternalStorage/Documents

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ComplianceApi.dll"]