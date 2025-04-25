# 1) Build your project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy csproj & restore to leverage layer cache
COPY ["BlazorApp1.csproj", "./"]
RUN dotnet restore "BlazorApp1.csproj"

# Copy and publish
COPY . .
RUN dotnet publish "BlazorApp1.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

# 2) Runtime image with Chromium & ChromeDriver
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Install Chromium, ChromeDriver and native libs
RUN apt-get update \
 && apt-get install -y --no-install-recommends \
      chromium \
      chromium-driver \
      ca-certificates \
      fonts-liberation \
      libatk-bridge2.0-0 \
      libatk1.0-0 \
      libcups2 \
      libdrm2 \
      libgbm1 \
      libgtk-3-0 \
      libnspr4 \
      libnss3 \
      libx11-xcb1 \
      libxcomposite1 \
      libxdamage1 \
      libxrandr2 \
      xdg-utils \
 && rm -rf /var/lib/apt/lists/*

# Create and switch to non‑root user
ARG APP_UID=1000
RUN groupadd --gid $APP_UID appgroup \
 && useradd --uid $APP_UID --gid appgroup --shell /bin/bash --create-home appuser
# Create the DataProtection key folder inside /app and give appuser ownership
RUN mkdir -p app/keys \
    && chown appuser:appgroup app/keys \
    && chmod 777 app/keys
WORKDIR /app

# Copy published app from build stage
COPY --from=build /app/publish .

# Ensure the embedded selenium‑manager is executable
RUN chmod +x /app/selenium-manager/linux/selenium-manager

# Tell ASP.NET to bind to port 5000
ENV ASPNETCORE_URLS=http://+:5000 \
    ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000

# Run as non‑root
USER appuser

# Launch the app
ENTRYPOINT ["dotnet", "BlazorApp1.dll"]
