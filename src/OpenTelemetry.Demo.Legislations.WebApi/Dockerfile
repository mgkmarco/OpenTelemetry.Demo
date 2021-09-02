FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /sln
ONBUILD COPY *.sln  NuGet.Config ./
COPY ["src/OpenTelemetry.Demo.Legislations.WebApi/OpenTelemetry.Demo.Legislations.WebApi.csproj", "src/OpenTelemetry.Demo.Legislations.WebApi/"]
RUN dotnet restore "src/OpenTelemetry.Demo.Legislations.WebApi/OpenTelemetry.Demo.Legislations.WebApi.csproj"

COPY . .
RUN dotnet build "src/OpenTelemetry.Demo.Legislations.WebApi/OpenTelemetry.Demo.Legislations.WebApi.csproj" -c Release

FROM build AS publish
WORKDIR "/sln/src/OpenTelemetry.Demo.Legislations.WebApi"
RUN dotnet publish "OpenTelemetry.Demo.Legislations.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetry.Demo.Legislations.WebApi.dll"]