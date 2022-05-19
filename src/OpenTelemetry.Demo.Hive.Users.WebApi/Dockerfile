FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /sln
ONBUILD COPY *.sln  ./NuGet.Config ./

COPY ["src/OpenTelemetry.Demo.Hive.Users.WebApi/OpenTelemetry.Demo.Hive.Users.WebApi.csproj", "src/OpenTelemetry.Demo.Hive.Users.WebApi/"]
COPY ./NuGet.Config ./
RUN dotnet restore "src/OpenTelemetry.Demo.Hive.Users.WebApi/OpenTelemetry.Demo.Hive.Users.WebApi.csproj" --configfile ./NuGet.Config

COPY . .
RUN dotnet build "src/OpenTelemetry.Demo.Hive.Users.WebApi/OpenTelemetry.Demo.Hive.Users.WebApi.csproj" -c Release

FROM build AS publish
WORKDIR "/sln/src/OpenTelemetry.Demo.Hive.Users.WebApi"
RUN dotnet publish "OpenTelemetry.Demo.Hive.Users.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OpenTelemetry.Demo.Hive.Users.WebApi.dll"]