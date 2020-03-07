#
#multi-stage target: dev
#
FROM mcr.microsoft.com/dotnet/core/sdk:2.1 as dev

ENV ASPNETCORE_URLS=http://*:5000 \
    ASPNETCORE_ENVIRONMENT=DEVELOPMENT

COPY . /app
WORKDIR /app/src/Foundry.Communications
RUN dotnet publish -o /app/dist
CMD [ "dotnet", "run" ]

#
#multi-stage target: prod
#
FROM mcr.microsoft.com/dotnet/core/aspnet:2.1 as prod
ARG commit
ENV COMMIT=$commit
COPY --from=dev /app/dist /app
WORKDIR /app
EXPOSE 80
ENV ASPNETCORE_URLS=http://*:80
CMD [ "dotnet", "Foundry.Communications.dll" ]
