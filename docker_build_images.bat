docker build -t domih/mcce-smart-office-apigateway -f .\src\Mcce.SmartOffice.ApiGateway\Dockerfile .

docker build -t domih/mcce-smart-office-userimage-api -f .\src\Mcce.SmartOffice.UserImages\Dockerfile .

docker build -t domih/mcce-smart-office-workspace-api -f .\src\Mcce.SmartOffice.Workspaces\Dockerfile .

docker build -t domih/mcce-smart-office-workspaceconfiguration-api -f .\src\Mcce.SmartOffice.WorkspaceConfigurations\Dockerfile .

docker build -t domih/mcce-smart-office-booking-api -f .\src\Mcce.SmartOffice.Bookings\Dockerfile .

docker build -t domih/mcce-smart-office-workspacedataentry-api -f .\src\Mcce.SmartOffice.WorkspaceDataEntries\Dockerfile .