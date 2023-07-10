FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

COPY ["src/Mcce.SmartOffice.Workspaces/.", "Mcce.SmartOffice.Workspaces/"]
COPY ["src/Mcce.SmartOffice.Core/.", "Mcce.SmartOffice.Core/"]
RUN dotnet restore "Mcce.SmartOffice.Workspaces/Mcce.SmartOffice.Workspaces.csproj"
COPY . .
WORKDIR "/src/Mcce.SmartOffice.Workspaces"
RUN dotnet build "Mcce.SmartOffice.Workspaces.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Mcce.SmartOffice.Workspaces.csproj" -c Release -o /app/publish
RUN addgroup --group serviceuser --gid 2000 \
&& adduser \    
    --uid 1000 \
    --gid 2000 \
    "serviceuser"
RUN chown serviceuser:serviceuser /app
USER serviceuser:serviceuser

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mcce.SmartOffice.Workspaces.dll"]