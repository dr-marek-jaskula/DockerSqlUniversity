	  USE VISUAL STUDIO CODE with Docker extension

============================
  The Docker Swarm mode is the production focus option that enables to create multiple container on multiple hosts and governs them by swarm "leader" node.
    Starting from version '3' can use docker-compose.yaml file to configure services. 
  Docker swarm includes:
    - config
    - secrets
    - rollback
    - multiple replicas (easy scaling)
    - health checks
    - automatic restarts
    - parallel execution of container across nodes with build load balancing
  
  Use Docker Swarm even with a single node if necessary
===========================

===========================
Best practises:
  Do not create containers on the "Leader" node. To achieve this aim we state the node status to "DRAIN". 
  Do same with manager "REACHABLE" nodes (possible Leaders)

===========================

  To create a multi node swarm:
1) Create a docker swarm: 
  docker swarm init
Now the host is the leader of a single node swarm.
2) We deploy stack, by docker-compose file, into the swarm
  docker stack deploy -c docker-compose.yaml my-first-stack
3) To update changes made in docker-compose.yaml just run the same command as above
x) When the stack is no longer needed:
  docker stack rm my-first-stack

===========================
Docker swarm node promote to manager (reachable) command:
  docker node promote <ip>

================================================
VI. Multi node docker swarm with services: api, db, seq:
    a) Three Dockerfiles: Dockerfile.webapi (in SakilaWebApi), Dockerfile.mssql (in DatabaseLibrary), Dockerfile.seq (in solution folder)
    b) One docker-compose.yaml in the solution folder
    c) Two environmental files "docker-mssql.env", "docker-seq.env" in the same folder as docker-compose.yaml
    d) External volumes: docker-mssql-volume, docker-seq-volume
    e) compose.cmd && compose.ps1 files to compose the solution (up or down) - project_name is specified in the docker-compose.yaml as comment. 
        Add to docker ignore: "**/compose*"
    f) Seq is included in the separete container, connected with webapi by the new network
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

======================

    compose.cmd:
======================

======================

    compose.ps1:
======================

======================

To create a named project (a group of container) we navigate to the file where the "compose.cmd" file is (where solution is) and write:
    Preferred way:
a) In the cmd, Make/Launch, vs terminal or similar:
    1) compose <up/down>             
b) In PowerShell and similar
    1) .\compose.ps1 <up/down>       
    2) "compose <up/down>" | cmd
