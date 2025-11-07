# ---- build ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# copie apenas o csproj primeiro para aproveitar cache
COPY BurgerApi/*.csproj BurgerApi/
RUN dotnet restore BurgerApi/BurgerApi.csproj

# copie todo o código e publique
COPY . .
RUN dotnet publish BurgerApi/BurgerApi.csproj -c Release -o /app/publish

# ---- runtime ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
# script de start (para honrar a $PORT do Railway)
COPY start.sh .
RUN chmod +x ./start.sh
EXPOSE 8080
CMD ["./start.sh"]
