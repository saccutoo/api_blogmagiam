#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["src/Services/Templates.API/Templates.API.csproj", "src/Services/Templates.API/"]
COPY ["src/BuildingBlocks/Utils/Utils/Utils.csproj", "src/BuildingBlocks/Utils/Utils/"]
RUN dotnet restore "src/Services/Templates.API/Templates.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Templates.API"
RUN dotnet build "Templates.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Templates.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Templates.API.dll"]