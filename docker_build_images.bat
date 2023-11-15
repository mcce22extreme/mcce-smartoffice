@echo off

FOR /F "delims=" %%v IN ('git describe --tags --abbrev^=0') DO set version=%%v

set version=%version:v=%

echo %version%

docker build -t domih/mcce-smart-office-apigateway -f .\src\Mcce.SmartOffice.ApiGateway\Dockerfile --build-arg="BUILD_VERSION=%version%" --build-arg="COMMIT_SHA=DEBUG" .

docker build -t domih/mcce-smart-office-userimage-api -f .\src\Mcce.SmartOffice.UserImages\Dockerfile --build-arg="BUILD_VERSION=%version%" --build-arg="COMMIT_SHA=DEBUG" .

docker build -t domih/mcce-smart-office-workspace-api -f .\src\Mcce.SmartOffice.Workspaces\Dockerfile --build-arg="BUILD_VERSION=%version%" --build-arg="COMMIT_SHA=DEBUG" .

docker build -t domih/mcce-smart-office-workspaceconfiguration-api -f .\src\Mcce.SmartOffice.WorkspaceConfigurations\Dockerfile --build-arg="BUILD_VERSION=%version%" --build-arg="COMMIT_SHA=DEBUG" .

docker build -t domih/mcce-smart-office-booking-api -f .\src\Mcce.SmartOffice.Bookings\Dockerfile --build-arg="BUILD_VERSION=%version%" --build-arg="COMMIT_SHA=DEBUG" .

docker build -t domih/mcce-smart-office-workspacedataentry-api -f .\src\Mcce.SmartOffice.WorkspaceDataEntries\Dockerfile --build-arg="BUILD_VERSION=%version%" --build-arg="COMMIT_SHA=DEBUG" .

docker build -t domih/mcce-smart-office-account-api -f .\src\Mcce.SmartOffice.Accounts\Dockerfile --build-arg="BUILD_VERSION=%version%" --build-arg="COMMIT_SHA=DEBUG" .