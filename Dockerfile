FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/TaskManager.API/TaskManager.API.csproj", "src/TaskManager.API/"]
COPY ["src/TaskManager.Application/TaskManager.Application.csproj", "src/TaskManager.Application/"]
COPY ["src/TaskManager.Infrastructure/TaskManager.Infrastructure.csproj", "src/TaskManager.Infrastructure/"]
COPY ["src/TaskManager.Domain/TaskManager.Domain.csproj", "src/TaskManager.Domain/"]

RUN dotnet restore "src/TaskManager.API/TaskManager.API.csproj"

COPY . .
WORKDIR "/src/src/TaskManager.API"
RUN dotnet build "TaskManager.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaskManager.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "TaskManager.API.dll"]
