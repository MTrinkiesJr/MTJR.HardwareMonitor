FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env

WORKDIR /app
COPY . ./
RUN dotnet restore ./MTJR.HardwareMonitor/MTJR.HardwareMonitor.csproj -s "https://api.nuget.org/v3/index.json"
RUN dotnet publish -c Release -o out ./MTJR.HardwareMonitor/MTJR.HardwareMonitor.csproj --no-restore

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "MTJR.HardwareMonitor.dll"]