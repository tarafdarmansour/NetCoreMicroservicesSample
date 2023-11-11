FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Src/UsersService/UsersService.csproj", "Src/UsersService/"]
COPY ["Src/UsersService/DataModel/DataModel.csproj", "Src/UsersService/DataModel/"]
COPY ["Src/Shared/Core/Events/Shared.Core.Events.csproj", "Src/Shared/Core/Events/"]
COPY ["Src/Shared/Infrastructure/Shared.Infrastructure.csproj", "Src/Shared/Infrastructure/"]
RUN dotnet restore "Src/UsersService/UsersService.csproj"
COPY . .
WORKDIR "/src/Src/UsersService"
RUN dotnet build "UsersService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UsersService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.dll"]