#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Craft.Api/Craft.Api.csproj", "Craft.Api/"]
COPY ["Infrastucture/Craft.Infrastucture.csproj", "Infrastucture/"]
COPY ["Craft.Persistence/Craft.Persistence.csproj", "Craft.Persistence/"]
COPY ["Craft.Application/Craft.Application.csproj", "Craft.Application/"]
COPY ["Craft.Domain/Craft.Domain.csproj", "Craft.Domain/"]
RUN dotnet restore "Craft.Api/Craft.Api.csproj"
COPY . .
WORKDIR "/src/Craft.Api"
RUN dotnet build "Craft.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Craft.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Craft.Api.dll"]