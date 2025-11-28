# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .csproj and restore
COPY ["WeeTuition/WeeTuition.csproj", "WeeTuition/"]
RUN dotnet restore "WeeTuition/WeeTuition.csproj"

# Copy the entire project
COPY . .
WORKDIR "/src/WeeTuition"
RUN dotnet build "WeeTuition.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "WeeTuition.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeeTuition.dll"]
