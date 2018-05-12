FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app

# copy everything and build the project
COPY . ./
RUN dotnet restore OKN.WebApp/*.csproj

FROM build AS publish
RUN dotnet publish OKN.WebApp/*.csproj -c Release -o out

# build runtime image
FROM base AS production
WORKDIR /app
COPY --from=publish /app/OKN.WebApp/out ./
ENTRYPOINT ["dotnet",  "OKN.WebApp.dll"]