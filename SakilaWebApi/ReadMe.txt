	USE VISUAL STUDIO CODE with Docker extension (simply the best)

I. Containerize the WebApi using manual commands
	0. Open Visual Studio Code and navigate to the folder
	1. Write Dockerfile
	2. In command line in the directory with the Dockerfile write (where "dockerwebapi" is the name of the container):
		docker build -t dockerwebapi .
	3. Then, write:
		docker run -p 8080:80 dockersqlwebapi
		Note that the internal port (which is 80) needs to be the same the one exposed in the Dockerfile (80 o 443 are for webpage)
	4. Examine the running container:
		http://localhost:8080/actor/example
	5. Examine in Visual Studio Code Docker extension images and containers

II. Containerize the WebApi using docker-compose.yaml file
	0. Open Visual Studio Code and navigate to the folder
	1. Write Dockerfile
	2. Wrote docker-compose.yaml 
	3. Examine if the docker-compose.yaml is the valid yaml file on ""

II. Containerize the WebApi and the Microsoft Sql Server 

