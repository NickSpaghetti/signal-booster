FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SignalBooster/SignalBoosterCLI/SignalBoosterCLI.csproj", "SignalBoosterCLI/"]
RUN dotnet restore "SignalBoosterCLI/SignalBoosterCLI.csproj"
COPY SignalBooster/SignalBoosterCLI/. ./SignalBoosterCLI/
COPY SignalBooster/Tests/. ./Tests/
WORKDIR "/src/SignalBoosterCLI"
RUN dotnet build "./SignalBoosterCLI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SignalBoosterCLI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS production
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SignalBoosterCLI.dll"]
#CMD ["--file","PathToFile"]
