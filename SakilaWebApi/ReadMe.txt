	USE VISUAL STUDIO CODE with Docker extension (simply the best)

I. Containerize the WebApi using manual commands
	0. Open Visual Studio Code, navigate to the folder and open Docker extension
	1. Write Dockerfile (its only for building image)

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
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SakilaWebApi.dll"]

	2. In command line in the directory with the Dockerfile write (where "dockerwebapi" is the name of the container):
		docker build -t docker-webapi .  
	3. Then, create an container named "docker-webapi-container" and run it:
		docker run --name docker-webapi-container -p 8080:80 docker-webapi
		Note that the internal port (which is 80) needs to be the same the one exposed in the Dockerfile (80 o 443 are for webpage)
	4. Examine the running container (For ConnectionString: DefaultConnection):
		http://localhost:8080/actor/example
	5. Examine in Visual Studio Code Docker extension images and containers

II. Containerize the WebApi using docker-compose.yaml file
	0. Open Visual Studio Code, navigate to the folder and open Docker extension
	1. Write Dockerfile (same as before)
	2. Write docker-compose.yaml

version: '3'

services:
  api:
    container_name: docker-webapi-by-compose-container
    build:
      context: .
      dockerfile: Dockerfile
    image: docker-webapi-by-compose
    ports:
      - 8080:80

	3. Examine if the docker-compose.yaml is the valid yaml file on "https://codebeautify.org/yaml-validator"
	4. Execute command "docker-compose up" in the working directory containing the Dockerfile and docker-compose.yaml
	5. Examine the running container:
		http://localhost:8080/actor/example
	6. Examine in Visual Studio Code Docker extension images and containers

II. Containerize Microsoft SQL Server using manual commands
	0. Open Visual Studio Code, navigate to the folder and open Docker extension
	1. Write the container in the detach mode (-d): (port 1433 is the default port for the MS Sql Server)
		docker run --name docker-mssql-container -e 'ACCEPT_EULA=Y' -e 'MSSQL_PID=Express' -e 'SA_PASSWORD=Pas123Word2022' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest
		a) Connect by Azure SQL Server
			1. Open Azure SQL Server. 
			2. Add new connection -> Connection Type = "Microsoft SQL Server", Server = "localhost,1433", Authentication type = "SQL Login", User name = "sa" (or other), Password = "Pas123Word2022", Name = DockerSqlConnection
			3. Add new query and do the sql. For instance add sakila database, with actor table same as in this api. Then, add some records:

CREATE DATABASE sakila;
use sakila;
CREATE TABLE actor(actor_id int PRIMARY KEY IDENTITY(1,1), first_name VARCHAR(50), last_name VARCHAR(50), last_update DATETIME);
INSERT INTO actor(first_name, last_name, last_update) VALUES('Marek', 'Jaskula', '2006-02-15T04:34:33');
INSERT INTO actor(first_name, last_name, last_update) VALUES('Kuba', 'Wojownik', '2006-02-15T04:34:35');
SELECT * FROM actor;
			
		b) Connect by web api from system
			1. Add new connection string: "FromComputerToDockerContainer": "Server=localhost,1433;Database=sakila;User ID=SA;Password=Pas123Word2022"
			2. In Program.cs change the connection string of the database context
	2. The progress is deleted when the container is deleted. To avoid this problem we will add volumes

! Containerize the WebApi and Microsoft SQL Server manually and connecting them is not possible without networks (docker networks). Networks are best to do with docker-compose.yaml

III. Containerize the WebApi and Microsoft SQL Server using docker-compose.yaml
	0. Open Visual Studio Code, navigate to the folder and open Docker extension
	1. Write Dockerfile (same as before)
	2. Write docker-compose.yaml

version: '3'

services:
  api:
    container_name: docker-webapi-by-compose-container
    build:
      context: .
      dockerfile: Dockerfile
    image: docker-webapi-by-compose
    networks:
      - db-net
    ports:
      - 8080:80
    depends_on:
      - "db"
  db:
    image: mcr.microsoft.com/mssql/server:2019-latest
    container_name: sql-container
    volumes:
      - dbdata:/var/lib/sql 
    environment:
      SA_PASSWORD: Pas123Word2022
      ACCEPT_EULA: Y
      MSSQL_PID: Express
    ports:
      - 1433:1433
    networks:
      - db-net

networks:
  db-net:

volumes:
  dbdata:

	3. Examine if the docker-compose.yaml is the valid yaml file on "https://codebeautify.org/yaml-validator"
		a) Connect by Azure SQL Server
			1. Open Azure SQL Server. 
			2. Add new connection -> Connection Type = "Microsoft SQL Server", Server = "localhost,1433", Authentication type = "SQL Login", User name = "sa" (or other), Password = "Pas123Word2022", Name = DockerSqlConnection
			3. Add new query and do the sql. For instance add sakila database, with actor table same as in this api. Then, add some records:

CREATE DATABASE sakila;
use sakila;
CREATE TABLE actor(actor_id int PRIMARY KEY IDENTITY(1,1), first_name VARCHAR(50), last_name VARCHAR(50), last_update DATETIME);
INSERT INTO actor(first_name, last_name, last_update) VALUES('Marek', 'Jaskula', '2006-02-15T04:34:33');
INSERT INTO actor(first_name, last_name, last_update) VALUES('Kuba', 'Wojownik', '2006-02-15T04:34:35');
SELECT * FROM actor;

		b) Connect by web api from system
			1. Add new connection string: "FromContainerToContainer": "Server=db,1433;Database=sakila;User ID=SA;Password=Pas123Word2022"
				This "db" in the above connection string is the name of the service from the "docker-compose.yaml"
			2. In Program.cs change the connection string of the database context

	5. Examine the running container:
		http://localhost:8080/actor/example
	6. Examine the container connection:
		http://localhost:8080/actor
	6. Examine in Visual Studio Code Docker extension images, containers and volumes

	