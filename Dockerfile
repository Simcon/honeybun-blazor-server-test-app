FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["HoneybunBlazorServerExample/HoneybunBlazorServerExample.csproj", "HoneybunBlazorServerExample/"]
RUN dotnet restore "HoneybunBlazorServerExample/HoneybunBlazorServerExample.csproj"
COPY . .
WORKDIR "/src/HoneybunBlazorServerExample"
RUN dotnet build "HoneybunBlazorServerExample.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HoneybunBlazorServerExample.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HoneybunBlazorServerExample.dll"]