FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY CVManager/*.csproj ./CVManager/
COPY CVManager.DAL/*.csproj ./CVManager.DAL/
RUN dotnet restore

# copy everything else and build app
COPY CVManager/. ./CVManager/
COPY CVManager.DAL/. ./CVManager.DAL/
WORKDIR /source/CVManager
RUN dotnet publish -c release -o /app #--no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "CVManager.dll","--environment=Docker"]
