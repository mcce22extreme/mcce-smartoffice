FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

COPY ["src/Mcce.SmartOffice.WorkspaceConfigurations/.", "Mcce.SmartOffice.WorkspaceConfigurations/"]
COPY ["src/Mcce.SmartOffice.Core/.", "Mcce.SmartOffice.Core/"]

RUN dotnet build "Mcce.SmartOffice.WorkspaceConfigurations/Mcce.SmartOffice.WorkspaceConfigurations.csproj" -c Release -o /app/build

RUN dotnet publish "Mcce.SmartOffice.WorkspaceConfigurations/Mcce.SmartOffice.WorkspaceConfigurations.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

 RUN addgroup --group serviceuser --gid 2000 \
 && adduser \    
     --uid 1000 \
     --gid 2000 \
     "serviceuser"
 RUN chown serviceuser:serviceuser /app
 USER serviceuser:serviceuser

COPY --from=build-env /app/publish .

ENTRYPOINT ["dotnet", "Mcce.SmartOffice.WorkspaceConfigurations.dll"]