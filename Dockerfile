# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Install libgdiplus for GDI+ support
RUN apt-get update && \
    apt-get install -y libgdiplus libc6-dev ffmpeg && \
    rm -rf /var/lib/apt/lists/*

# Build image
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY *.sln ./
COPY ./src/TWJ.TWJApp.TWJService.Api/TWJ.TWJApp.TWJService.Api.csproj ./src/TWJ.TWJApp.TWJService.Api/
COPY ./src/TWJ.TWJApp.TWJService.Common/TWJ.TWJApp.TWJService.Common.csproj ./src/TWJ.TWJApp.TWJService.Common/
COPY ./src/TWJ.TWJApp.TWJService.Domain/TWJ.TWJApp.TWJService.Domain.csproj ./src/TWJ.TWJApp.TWJService.Domain/
COPY ./src/TWJ.TWJApp.TWJService.Application/TWJ.TWJApp.TWJService.Application.csproj ./src/TWJ.TWJApp.TWJService.Application/
COPY ./src/TWJ.TWJApp.TWJService.Persistence/TWJ.TWJApp.TWJService.Persistence.csproj ./src/TWJ.TWJApp.TWJService.Persistence/
COPY ./tests/TWJ.TWJApp.TWJService.UnitTest/TWJ.TWJApp.TWJService.UnitTest.csproj ./tests/TWJ.TWJApp.TWJService.UnitTest/
COPY ./WebScraper/WebScraper.csproj ./WebScraper/
RUN dotnet restore
COPY . .
WORKDIR ./src/TWJ.TWJApp.TWJService.Api
RUN dotnet build "TWJ.TWJApp.TWJService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TWJ.TWJApp.TWJService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TWJ.TWJApp.TWJService.Api.dll"]


#"C:\Sources\thewellnessjunction\TWJ.TWJApp.TWJService.sln"
#"C:\Sources\thewellnessjunction"
#"C:\Sources\thewellnessjunction\Dockerfile"
#"C:\Sources\thewellnessjunction\WebScraper\WebScraper.csproj"
#"C:\Sources\thewellnessjunction\src\TWJ.TWJApp.TWJService.Api\TWJ.TWJApp.TWJService.Api.csproj"
#"C:\Sources\thewellnessjunction\src\TWJ.TWJApp.TWJService.Application\TWJ.TWJApp.TWJService.Application.csproj"
#"C:\Sources\thewellnessjunction\src\TWJ.TWJApp.TWJService.Common\TWJ.TWJApp.TWJService.Common.csproj"
#"C:\Sources\thewellnessjunction\src\TWJ.TWJApp.TWJService.Domain\TWJ.TWJApp.TWJService.Domain.csproj"
#"C:\Sources\thewellnessjunction\src\TWJ.TWJApp.TWJService.Persistence\TWJ.TWJApp.TWJService.Persistence.csproj"