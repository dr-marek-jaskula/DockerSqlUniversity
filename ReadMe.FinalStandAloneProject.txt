	  USE VISUAL STUDIO CODE with Docker extension

  The Stand Alone Docker mode with Docker Compose is demonstrated for the learning purpose. 
  Nevertheless, in production always use the Docker Swarm, which was designed for production: 
    1) Docker Swarm includes config
    2) Docker Swarm includes secrets
    3) Docker Swarm includes rollback, always uptime and other features (also multiple replicas)
  Use Docker Swarm even with a single node if necessary

INFORMATION:
    docker-compose up       -> uses Compose V1 if the flag "Use Docker Compose V2" is unchecked (to examine it -> Docker Desktop -> Setting -> General)
    docker compose up       -> uses Compose V2 (sometime its not good -> for example reading the .env file can be problematic)

Current structure of the solution is same as the solution structure for this file: 
  Multiple projects, multiple dockerfiles, docker-compose in the solution folder, compose.cmd, compose.ps1 to run docker-compose up with named project, seq logging implemented with a new container

Additional informations:
1. For mssql: At least 2GB of RAM (3.25 GB prior to 2017-CU2). Make sure to assign enough memory to the Docker VM if you're running on Docker for Mac or Windows.
To examine the memory maximal capacity and current usage:
    docker stats

2. To examine that the container is not running as "root" we need to:
    1. docker exec -it docker-mssql-container bash whoami
-> The result should be "mssql" not a root

3. In docker-compose "depend on":
  You can control the order of service startup and shutdown with the depends_on option

================================================
VI. Containerize the WebApi and Microsoft SQL Server using docker-compose.yaml:
    a) Three Dockerfiles: Dockerfile.webapi (in SakilaWebApi), Dockerfile.mssql (in DatabaseLibrary), Dockerfile.seq (in solution folder) !! Naming convention changed in comparision to prvious ReadMe's
    b) One docker-compose.yaml in the solution folder
    c) Two environmental files "docker-mssql.env", "docker-seq.env" in the same folder as docker-compose.yaml
    d) External volumes: docker-mssql-volume, docker-seq-volume
    e) compose.cmd && compose.ps1 files to compose the solution (up or down) - project_name is specified in the docker-compose.yaml as comment. 
        Add to docker ignore: "**/compose*"
    f) seq is included in the separete container, connected with webapi by the new network
================================================

    Dockerfile.webapi:
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

    Dockerfile.mssql:
======================
FROM mcr.microsoft.com/mssql/server:2019-latest
======================

    Dockerfile.seq:
======================
FROM FROM datalust/seq:latest
======================

    docker-mssql.env:
======================
SA_PASSWORD=Pas123Word2022
ACCEPT_EULA=Y
MSSQL_PID=Express
======================

    docker-seq.env:
======================
ACCEPT_EULA=Y
======================

    docker-compose.yaml:
======================
#project_name: final-project

version: '3.8'

services:
  api:
    container_name: docker-webapi-container
    build:
      context: .
      dockerfile: SakilaWebApi/Dockerfile.webapi
    image: docker-webapi-image
    restart: unless-stopped
    networks:
      - db-net
      - log-net
    ports:
      - 8080:80
    depends_on:
      - "db"
      - "seq"
  db:
    container_name: docker-mssql-container
    build:
      context: ./DatabaseLibrary
      dockerfile: Dockerfile.mssql
    image: docker-mssql-image
    volumes:
      - dbdata:/var/opt/mssql
    env_file:
      - docker-mssql.env
    restart: always
    ports:
      - 1433:1433
    networks:
      - db-net
  seq:
    container_name: docker-seq-container
    build:
      context: .
      dockerfile: Dockerfile.seq
    image: docker-seq-image
    ports:
      - 5341:80
    env_file:
      - docker-seq.env
    restart: unless-stopped
    volumes:
      - ./seqdata:/data
    networks:
      - log-net

networks:
  db-net:
    name: docker-db-network
  log-net:
    name: docker-log-network

volumes:
  dbdata:
    name: docker-mssql-volume
  seqdata:
    name: docker-seq-volume
======================

    compose.cmd:
======================
set result=
for /f "tokens=*" %%i in ('findstr "project_name" docker-compose.yaml') do (
  set result=%%i
)
set str=%result:~15,30%
docker-compose -p %str% %1
======================

    compose.ps1:
======================
param
(
    [Parameter(Mandatory)]$Command
)
"compose " + $Command | cmd
======================

To create a named project (a group of container) we navigate to the file where the "compose.cmd" file is (where solution is) and write:
    Preferred way:
a) In the cmd, Make/Launch, vs terminal or similar:
    1) compose <up/down>             -> to invoke docker-compose <project_name> <up/down>   
b) In PowerShell and similar
    1) .\compose.ps1 <up/down>       -> to invoke docker-compose <project_name> <up/down> 
    2) "compose <up/down>" | cmd     -> to invoke docker-compose <project_name> <up/down> 

    Other way:
docker-compose -p <project_name> up