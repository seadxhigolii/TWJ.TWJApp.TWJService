#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/TWJ.TWJApp.TWJService.Api/TWJ.TWJApp.TWJService.Api.csproj", "src/TWJ.TWJApp.TWJService.Api/"]
COPY ["src/TWJ.TWJApp.TWJService.Common/TWJ.TWJApp.TWJService.Common.csproj", "src/TWJ.TWJApp.TWJService.Common/"]
COPY ["src/TWJ.TWJApp.TWJService.Domain/TWJ.TWJApp.TWJService.Domain.csproj", "src/TWJ.TWJApp.TWJService.Domain/"]
COPY ["src/TWJ.TWJApp.TWJService.Application/TWJ.TWJApp.TWJService.Application.csproj", "src/TWJ.TWJApp.TWJService.Application/"]
COPY ["src/TWJ.TWJApp.TWJService.Persistence/TWJ.TWJApp.TWJService.Persistence.csproj", "src/TWJ.TWJApp.TWJService.Persistence/"]
RUN dotnet restore "src/TWJ.TWJApp.TWJService.Api/TWJ.TWJApp.TWJService.Api.csproj"
COPY . .
WORKDIR "/src/src/TWJ.TWJApp.TWJService.Api"
RUN dotnet build "TWJ.TWJApp.TWJService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TWJ.TWJApp.TWJService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TWJ.TWJApp.TWJService.Api.dll"]