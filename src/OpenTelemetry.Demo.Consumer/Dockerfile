FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /sln
ONBUILD COPY *.sln  NuGet.Config ./
COPY ["src/OpenTelemetry.Demo.Consumer/OpenTelemetry.Demo.Consumer.csproj", "src/OpenTelemetry.Demo.Consumer/"]
RUN dotnet restore "src/OpenTelemetry.Demo.Consumer/OpenTelemetry.Demo.Consumer.csproj"

COPY . .
RUN dotnet build "src/OpenTelemetry.Demo.Consumer/OpenTelemetry.Demo.Consumer.csproj" -c Release

FROM build AS publish
WORKDIR "/sln/src/OpenTelemetry.Demo.Consumer"
RUN dotnet publish "OpenTelemetry.Demo.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetry.Demo.Consumer.dll"]