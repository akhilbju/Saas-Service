FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

ARG GITHUB_USERNAME
ARG GITHUB_TOKEN

RUN dotnet nuget add source \
    https://nuget.pkg.github.com/akhilbju/index.json \
    -n github \
    -u $GITHUB_USERNAME \
    -p $GITHUB_TOKEN \
    --store-password-in-clear-text

COPY . .
RUN dotnet restore

RUN dotnet publish Saas-Auth-Service/Saas-Auth-Service.csproj \
    -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Saas-Auth-Service.dll"]
