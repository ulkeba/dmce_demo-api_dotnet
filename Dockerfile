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
# <HACK: use image 'sdk' instead of 'aspnet' as hack to allow "dotnet dev-certs" below.>
FROM mcr.microsoft.com/dotnet/sdk:6.0 
ARG API_VERSION=<dockerfile-default>
ENV API_VERSION  ${API_VERSION}

# <HACK to add https endpoints with dev certs>
ENV ASPNETCORE_URLS "https://+:443;http://+:80"
ENV ASPNETCORE_HTTPS_PORT 443
RUN dotnet dev-certs https
# <HACK end>

WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "WeatherForecastAPI.dll"]
