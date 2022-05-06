set result=
for /f "tokens=*" %%i in ('findstr "project_name" docker-compose.yaml') do (
  set result=%%i
)
set str=%result:~15,30%
docker-compose -p %str% up
