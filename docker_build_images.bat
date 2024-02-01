@echo off

FOR /F "delims=" %%v IN ('git describe --tags --abbrev^=0') DO set version=%%v

set version=%version:v=%

echo %version%

docker build -t domih/mcce-smart-office-api -f .\src\Mcce.SmartOffice.Api\Dockerfile --build-arg="BUILD_VERSION=%version%" --build-arg="COMMIT_SHA=DEBUG" .