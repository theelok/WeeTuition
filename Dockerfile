# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .csproj and restore
COPY ["TimetableSystem/TimetableSystem.csproj", "TimetableSystem/"]
RUN dotnet restore "TimetableSystem/TimetableSystem.csproj"

# Copy the entire project
COPY . .
WORKDIR "/src/TimetableSystem"
RUN dotnet build "TimetableSystem.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TimetableSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimetableSystem.dll"]