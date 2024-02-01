FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

COPY ["src/Mcce.SmartOffice.Api/.", "Mcce.SmartOffice.Api/"]

RUN dotnet build "Mcce.SmartOffice.Api/Mcce.SmartOffice.Api.csproj" -c Release -o /app/build

FROM build AS publish

ARG BUILD_VERSION
ARG COMMIT_SHA

RUN dotnet publish "Mcce.SmartOffice.Api/Mcce.SmartOffice.Api.csproj" -c Release -o /app/publish /p:Version="${BUILD_VERSION}" /p:InformationalVersion="${BUILD_VERSION}-${COMMIT_SHA}"

FROM base AS final

WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

RUN addgroup --group serviceuser --gid 2000 \
    && adduser \    
    --uid 1000 \
    --gid 2000 \
    "serviceuser"

RUN chown serviceuser:serviceuser /app
USER serviceuser:serviceuser

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Mcce.SmartOffice.Api.dll"]