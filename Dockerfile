## Multi-stage build for ASP.NET Core server
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj and restore as distinct layers (paths relative to build context)
COPY GarageInventoryServer.csproj ./
RUN dotnet restore GarageInventoryServer.csproj

# copy everything else and build
COPY . .
WORKDIR /src
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "GarageInventoryServer.dll"]


