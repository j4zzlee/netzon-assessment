FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["./netzon-assetment.csproj", "netzon-assetment/"]
RUN dotnet restore "netzon-assetment/netzon-assetment.csproj"
COPY . "./netzon-assetment"
WORKDIR "/src/netzon-assetment"
RUN dotnet build "netzon-assetment.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "netzon-assetment.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "netzon-assetment.dll"]