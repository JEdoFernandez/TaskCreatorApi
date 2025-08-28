FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TaskCreatorAPI.csproj", "."]
RUN dotnet restore "TaskCreatorAPI.csproj"
COPY . .
RUN dotnet build "TaskCreatorAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskCreatorAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaskCreatorAPI.dll"]