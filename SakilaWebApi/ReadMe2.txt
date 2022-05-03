Continuation of the ReadMe.txt

================================================
V. Containerize the WebApi and Microsoft SQL Server using docker-compose.yaml:
    a) Two Dockerfiles: Dockerfile-webapi, Dockerfile-mssql
    b) External volume: docker-mssql-volume
    c) Custom environmental variables file: docker-mssql.env
================================================

    Dockerfile-webapi:

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY ["SakilaWebApi.csproj", "."]
RUN dotnet restore 
COPY . .
RUN dotnet build "SakilaWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SakilaWebApi.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
LABEL Name="Eltin"
WORKDIR /app
EXPOSE 80 
EXPOSE 443 
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SakilaWebApi.dll"]

    Dockerfile-mssql:

FROM mcr.microsoft.com/mssql/server:2019-latest AS build

    docker-mssql.env:

SA_PASSWORD=Pas123Word2022
ACCEPT_EULA=Y
MSSQL_PID=Express

    docker-compose.yaml:

version: '3.8'

services:
  api:
    container_name: docker-webapi-container
    build:
      context: .
      dockerfile: Dockerfile-webapi
    image: docker-webapi-image
    networks:
      - db-net
    ports:
      - 8080:80
    depends_on:
      - "db"
  db:
    container_name: docker-mssql-container
    build:
      context: .
      dockerfile: Dockerfile-mssql
    image: docker-mssql-image
    volumes:
      - dbdata:/var/opt/mssql
    env_file:
      - docker-mssql.env
    ports:
      - 1433:1433
    networks:
      - db-net

networks:
  db-net:
    name: docker-webapi-network

volumes:
  dbdata:
    name: docker-mssql-volume
    external: true


