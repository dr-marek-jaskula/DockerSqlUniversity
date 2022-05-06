	USE VISUAL STUDIO CODE with Docker extension (simply the best)

INFORMATION:
    docker-compose up       -> uses Compose V1 if the flag "Use Docker Compose V2" is unchecked (to examine it -> Docker Desktop -> Setting -> General)
    docker compose up       -> uses Compose V2 (sometime its not good -> for example reading the .env file can be problematic)

Current structure of the solution is same as the solution structure for this file: 
  Multiple projects, multiple dockerfiles, one docker-compose in the solution folder, one compose.cmd file to run docker-compose up with named project

Additional informations:
1. For mssql: At least 2GB of RAM (3.25 GB prior to 2017-CU2). Make sure to assign enough memory to the Docker VM if you're running on Docker for Mac or Windows.
To examine the memory maximal capacity and current usage:
    docker stats

2. To examine that the container is not running as "root" we need to:
    1. docker exec -it docker-mssql-container bash whoami
-> The result should be "mssql" not a root

================================================
VI. Containerize the WebApi and Microsoft SQL Server using docker-compose.yaml:
    a) Two Dockerfiles: Dockerfile-webapi (in SakilaWebApi), Dockerfile-mssql (in DatabaseLibrary)
    b) One docker-compose.yaml in the solution folder
    c) One environmental file "docker-mssql.env" in the same folder as docker-compose.yaml
    d) External volume: docker-mssql-volume
    e) compose.cmd file to run "docker-compose -p my-first-project up" by witting "compose" in batch (project_name specified in the docker-compose.yaml as comment)
================================================

    Dockerfile-webapi:
======================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app
COPY ["DockerSqlUniversity.sln", "."]
COPY ["DatabaseLibrary/DatabaseLibrary.csproj", "./DatabaseLibrary/"]
COPY ["SakilaWebApi/SakilaWebApi.csproj", "./SakilaWebApi/"]

WORKDIR /app/DatabaseLibrary
RUN dotnet restore
WORKDIR /app/SakilaWebApi
RUN dotnet restore

WORKDIR /app
COPY ["DatabaseLibrary/.", "./DatabaseLibrary/"]
COPY ["SakilaWebApi/.", "./SakilaWebApi/"]

WORKDIR /app/SakilaWebApi
RUN dotnet build "SakilaWebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SakilaWebApi.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
LABEL Name="Eltin"

WORKDIR /app/SakilaWebApi
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SakilaWebApi.dll"]
======================

    Dockerfile-mssql:
======================
FROM mcr.microsoft.com/mssql/server:2019-latest
======================

    docker-mssql.env:
======================
SA_PASSWORD=Pas123Word2022
ACCEPT_EULA=Y
MSSQL_PID=Express
======================

    docker-compose.yaml:
======================
#project_name: my-first-project

version: '3.8'

services:
  api:
    container_name: docker-webapi-container
    build:
      context: .
      dockerfile: SakilaWebApi/Dockerfile-webapi
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
      context: ./DatabaseLibrary
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
======================

    compose.cmd:
======================
set result=
for /f "tokens=*" %%i in ('findstr "project_name" docker-compose.yaml') do (
  set result=%%i
)
set str=%result:~15,30%
docker-compose -p %str% up
======================

To create a named project (a group of container) we navigate to the file where the "compose.cmd" file is (where solution is) and write:
    compose <project_name>

or write:
    docker-compose -p <project_name> up