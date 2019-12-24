FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["OKN.WebApp/OKN.WebApp.csproj", "OKN.WebApp/"]
COPY ["AspNet.Security.OAuth.Oldsaratov/AspNet.Security.OAuth.Oldsaratov.csproj", "AspNet.Security.OAuth.Oldsaratov/"]
COPY ["OKN.Core/OKN.Core.csproj", "OKN.Core/"]
RUN dotnet restore "OKN.WebApp/OKN.WebApp.csproj"
COPY . .
WORKDIR "/src/OKN.WebApp"
RUN dotnet build "OKN.WebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "OKN.WebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OKN.WebApp.dll"]