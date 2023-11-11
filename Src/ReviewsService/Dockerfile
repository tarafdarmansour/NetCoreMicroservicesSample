FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Src/ReviewsService/ReviewsService.csproj", "Src/ReviewsService/"]
COPY ["Src/ReviewsService/DataModel/DataModel.csproj", "Src/ReviewsService/DataModel/"]
COPY ["Src/Shared/Core/Events/Shared.Core.Events.csproj", "Src/Shared/Core/Events/"]
COPY ["Src/Shared/Infrastructure/Shared.Infrastructure.csproj", "Src/Shared/Infrastructure/"]
RUN dotnet restore "Src/ReviewsService/ReviewsService.csproj"
COPY . .
WORKDIR "/src/Src/ReviewsService"
RUN dotnet build "ReviewsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReviewsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewsService.dll"]