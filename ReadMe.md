# Docker Learning Solution

In this solution the simple ASP.NET webapi, connected to the Microsoft SQL Server by Entity Framework Core, is presented.
The Portainer is included to govern the swarm. Moreover, the Seq is used as a logging server.

Use "swarm <up/down>" in cmd or ".\swarm.ps1" in shell to create the swarm and deploy services.

**Five services are deployed into the one-node swarm.**

## Technologies

* C#
* Docker
* ASP.NET Core 
* Entity Framework Core

## Services

* Webapi
* SQL Server
* Seq
* Portainer
* Portainer agent


## Main features

* Dockerfiles
* Composing services
* Docker networking 
* Docker volumes
* Docker secrets
* Docker Swarm
* Seq
* Portainer