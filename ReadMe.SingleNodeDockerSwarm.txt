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
  To create a single node swarm:
1) Create a docker swarm: 
  docker swarm init
Now the host is the leader of a single node swarm.
2) We deploy stack, by docker-compose file, into the swarm
  docker stack deploy -c docker-compose.yaml single-node-swarm
3) To update changes made in docker-compose.yaml just run the same command as above
4) To leave the swarm use:
  docker swarm leave --force 
  (the force argument is used because leaving the swarm in the single node swarm will result in dropping the whole swarm. 
  Therefore, the additonal serurity leayer is present)
================================================
By default the volume folder are stored in:
\\wsl$\docker-desktop-data\version-pack-data\community\docker\volumes\

We can easly change the location of the data volumes and store data in the folder related to the given context (example for seq):
replace "seqdata:/data" into "./seqdata:/data"
================================================
VI. Single node docker swarm with services: api, db, seq, agent, portainer:
    a) Three Dockerfiles: Dockerfile.webapi (in SakilaWebApi), Dockerfile.mssql (in DatabaseLibrary), Dockerfile.seq (in solution folder)
    b) One docker-compose.yaml in the solution folder
    c) Two environmental files "docker-mssql.env", "docker-seq.env" in the same folder as docker-compose.yaml
    d) External volumes: docker-mssql-volume, docker-seq-volume, docker-portainer-volume
    e) compose.cmd && compose.ps1 files to compose the solution (up or down) - project_name is specified in the docker-compose.yaml as comment. 
        Add to docker ignore: "**/compose*"
    f) Seq is included in the separete container, connected with webapi by the new network
    g) Portainer agent and Portiner to vizualize the swarm
    h) ConnectionString.json file in which we can safetly store sensitive data
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
#stack_name: one-node-swarm

version: '3.8'

services:
  api:
    build:
      context: .
      dockerfile: SakilaWebApi/Dockerfile.webapi
    image: webapi-image
    restart: unless-stopped
    networks:
      - db-net
      - log-net
    ports:
      - 8080:80
    depends_on:
      - "db"
      - "seq"
    deploy:
      mode: replicated
      replicas: 1
      labels:
        - name=API
      placement:
        constraints: 
          - node.role == manager
  db:
    build:
      context: ./DatabaseLibrary
      dockerfile: Dockerfile.mssql
    image: mssql-image
    volumes:
      - dbdata:/var/opt/mssql
    env_file:
      - docker-mssql.env
    restart: unless-stopped
    ports:
      - 1433:1433
    networks:
      - db-net
    deploy:
      mode: replicated
      replicas: 1
      labels:
        - name=DB
      resources:
        # Hard limit - Docker does not allow to allocate more
        limits:
          memory: 4G
      placement:
        constraints: 
          - node.role == manager
      restart_policy:
        condition: on-failure
        delay: 5s
        max_attempts: 3
        window: 120s
      update_config:
        parallelism: 1
        delay: 10s
        failure_action: continue
        monitor: 60s
        max_failure_ratio: 0.3
  seq:
    build:
      context: .
      dockerfile: Dockerfile.seq
    image: seq-image
    ports:
      - 5341:80
    env_file:
      - docker-seq.env
    restart: unless-stopped
    volumes:
      - seqdata:/data
    networks:
      - log-net
    deploy:
      mode: replicated
      replicas: 1
      labels: 
        - name=SEQ
      resources:
        # Hard limit - Docker does not allow to allocate more
        limits:
          cpus: '0.25'
          memory: 512M
        # Soft limit - Docker makes best effort to return to it
        reservations:
          cpus: '0.25'
          memory: 256M
      restart_policy:
        condition: on-failure
        delay: 5s
        max_attempts: 3
        window: 120s
      update_config:
        parallelism: 1
        delay: 10s
        failure_action: continue
        monitor: 60s
        max_failure_ratio: 0.3
      placement:
        constraints: 
          - node.role == manager
  agent:
    image: portainer/agent:latest
    environment:
      AGENT_CLUSTER_ADDR: tasks.agent
    volumes:
      - /var/run/docker.sock:/var/run/docker.sock
      - /var/lib/docker/volumes:/var/lib/docker/volumes
    ports:
      - target: 9001
        published: 9001
        protocol: tcp
        mode: host
    networks:
      - portainer-net
    deploy:
      mode: global
      placement:
        constraints: 
          - node.platform.os == linux
  portainer:
    image: portainer/portainer-ce:latest
    command: -H tcp://tasks.agent:9001 --tlsskipverify
    restart: unless-stopped
    security_opt:
      - no-new-privileges:true
    networks:
      - portainer-net
    ports:
      - 9000:9000
    depends_on:
      - "agent"
    volumes:
      - portainerdata:/data
    deploy:
      placement:
        constraints:
          - node.role == manager

networks:
  db-net:
    name: db-network
  log-net:
    name: log-network
  portainer-net:
    name: portainer-network
    driver: overlay
    attachable: true

volumes:
  dbdata:
    name: mssql-volume
  seqdata:
    name: seq-volume
  portainerdata:
    name: portainer-volume
======================

    compose.cmd:
======================
set result=
for /f "tokens=*" %%i in ('findstr "stack_name" docker-compose.yaml') do (
  set result=%%i
)
set str=%result:~13,30%
if %1 == up (
  docker swarm init
  docker stack deploy -c docker-compose.yaml %str%
)
if %1 == down (
  docker swarm leave --force
)
======================

    compose.ps1:
======================
set result=
for /f "tokens=*" %%i in ('findstr "stack_name" docker-compose.yaml') do (
  set result=%%i
)
set str=%result:~13,30%
if %1 == up (
  docker swarm init
  docker stack deploy -c docker-compose.yaml %str%
)
if %1 == down (
  docker swarm leave --force
)
======================

To create a named project (a group of container) we navigate to the file where the "compose.cmd" file is (where solution is) and write:
    Preferred way:
a) In the cmd, Make/Launch, vs terminal or similar:
    1) swarm <up/down>             
b) In PowerShell and similar
    1) .\swarm.ps1 <up/down>       
    2) "swarm <up/down>" | cmd