#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-extra AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CommentMap.Mvc.csproj", "libman.json", "CommentMap.Mvc/"]
RUN dotnet restore "./CommentMap.Mvc/CommentMap.Mvc.csproj"
WORKDIR "/src/CommentMap.Mvc"
COPY . .
RUN dotnet build --no-restore "./CommentMap.Mvc.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CommentMap.Mvc.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM node:20.14-alpine3.19 AS build-static
ENV NPM_CONFIG_UPDATE_NOTIFIER=false
ENV NPM_CONFIG_FUND=false
WORKDIR /src
COPY ["package.json", "package-lock.json", "tsconfig.json", "./"]
RUN npm ci --omit=dev
COPY "build" "./build"
COPY "Scripts" "./Scripts"
RUN npm run build

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build-static /src/wwwroot ./wwwroot
ENTRYPOINT ["dotnet", "CommentMap.Mvc.dll"]