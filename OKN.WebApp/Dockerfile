FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY OKN.WebApp/OKN.WebApp.csproj OKN.WebApp/
COPY AspNet.Security.OAuth.Oldsaratov/AspNet.Security.OAuth.Oldsaratov.csproj AspNet.Security.OAuth.Oldsaratov/
COPY OKN.Core/OKN.Core.csproj OKN.Core/
RUN dotnet restore OKN.WebApp/OKN.WebApp.csproj
COPY . .
WORKDIR /src/OKN.WebApp
RUN dotnet build OKN.WebApp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish OKN.WebApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "OKN.WebApp.dll"]
