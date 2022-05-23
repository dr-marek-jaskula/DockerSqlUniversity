if %1 == up (
  set result=
for /f "tokens=*" %%i in ('findstr "stack_name" docker-compose.yaml') do (
  set result=%%i
)
set str=%result:~13,30%
  docker swarm init
  docker stack deploy -c docker-compose.yaml %str%
)
if %1 == down (
  docker swarm leave --force
)