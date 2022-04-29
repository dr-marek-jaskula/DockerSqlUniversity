	USE VISUAL STUDIO CODE with Docker extension (simply the best)

I. Containerize just the WebApi (without the sql)
	1. Write Dockerfile
	2. In command line in the directory with the Dockerfile write (where "dockerwebapi" is the name of the container):
		docker build -t dockerwebapi .
	3. Then, write:
		docker run -p 8080:80 dockersqlwebapi
		Note that the internal port (which is 80) needs to be the same the one exposed in the Dockerfile (80 o 443 are for webpage)
	4. Examine the running container:
		http://localhost:8080/actor/example

II. Containerize the WebApi and the Microsoft Sql Server 

