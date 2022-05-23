Multiple node docker swarm is almost the same as single node swarm.

Instruction:

===========================
Best practises:
  Do not create containers on the "Leader" node. To achieve this aim we state the node status to "DRAIN". 
  Do same with manager "REACHABLE" nodes (possible Leaders)

===========================

  To create a multi node swarm:
1) Create a docker swarm: 
  docker swarm init
and copy the join token. Now the host is the leader of a single node swarm.
2) Add two nodes by using:
docker swarm join \
  --token  SWMTKN-1-49nj1cmql0jkz5s954yi3oex3nedyz0fb0xx14ie39trti4wxv-8vxv8rssmk743ojnwacrr2e7c \
  192.168.99.100:2377
3) List nodes by:
  docker node ls
2) We deploy stack, by docker-compose file, into the swarm
  docker stack deploy -c docker-compose.yaml my-first-stack
To update changes made in docker-compose.yaml just run the same command as above
3) List services using: 
  docker service ls
4) Examine services using:
  docker service inspect --pretty <SERVICE-ID>
5) See nodes running the service:
  docker service ps <SERVICE-ID>
6) Scale the application by:
  docker service scale <SERVICE-ID>=<NUMBER-OF-TASKS>
7) Delete the service by:
  docker service rm helloworld
8) Change Leader status to drain 
  docker node update --availability drain <NODE-ID>
x) When the stack is no longer needed:
  docker stack rm my-first-stack

===========================
Docker swarm node promote to manager (reachable) command:
  docker node promote <ip>
