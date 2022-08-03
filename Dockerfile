# Taken from https://github.com/dotnet/dotnet-docker/blob/main/samples/aspnetapp/Dockerfile

# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY dotnet/*.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY dotnet/. ./
WORKDIR /source
RUN dotnet publish -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
ARG API_VERSION=<dockerfile-default>
ENV API_VERSION  ${API_VERSION}
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "WeatherForecastAPI.dll"]
