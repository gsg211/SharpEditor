FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BackEnd.csproj", "./"]
RUN dotnet restore "BackEnd.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./BackEnd.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BackEnd.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# --- MODIFICĂM DOAR ETAPA FINALĂ ---
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Devenim root temporar pentru a configura folderul de date
USER root
RUN mkdir -p /app/data && chown -R $APP_UID:$APP_UID /app/data

# Ne întoarcem la utilizatorul securizat implicit al imaginii (.NET app user)
USER $APP_UID
ENTRYPOINT ["dotnet", "BackEnd.dll"]