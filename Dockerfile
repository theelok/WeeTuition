# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Install Node.js for frontend build
RUN curl -fsSL https://deb.nodesource.com/setup_18.x | bash - && \
    apt-get install -y nodejs

# Copy .csproj and restore
COPY ["TimetableSystem.csproj", "./"]
RUN dotnet restore "TimetableSystem.csproj"

# Build frontend
COPY ["frontend/", "./frontend/"]
WORKDIR "/src/frontend"
RUN npm install && npm run build

# Copy the entire project
WORKDIR /src
COPY . .

# Create wwwroot and copy frontend build
RUN mkdir -p wwwroot && cp -r frontend/dist/* wwwroot/

# Build and publish .NET app
WORKDIR "/src"
RUN dotnet build "TimetableSystem.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "TimetableSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false
RUN cp -r wwwroot /app/publish/

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TimetableSystem.dll"]