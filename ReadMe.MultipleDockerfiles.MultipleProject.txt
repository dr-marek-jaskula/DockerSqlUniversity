	USE VISUAL STUDIO CODE with Docker extension (simply the best)

INFORMATION:
    docker-compose up       -> uses Compose V1 if the flag "Use Docker Compose V2" is unchecked (to examine it -> Docker Desktop -> Setting -> General)
    docker compose up       -> uses Compose V2 (sometime its not good -> for example reading the .env file can be problematic)

Currect structure of the solution is: 
  multiple projects, multiple dockerfiles and one docker-compose in the solution folder

================================================
VI. Containerize the WebApi and Microsoft SQL Server using docker-compose.yaml:
    a) Two Dockerfiles: Dockerfile-webapi, Dockerfile-mssql
    b) External volume: docker-mssql-volume
================================================

