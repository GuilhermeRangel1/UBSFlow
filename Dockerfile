FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY UBSFlow.sln ./
COPY Directory.Build.props ./
COPY src/UBSFlow.Api/UBSFlow.Api.csproj src/UBSFlow.Api/
COPY src/UBSFlow.Aplicacao/UBSFlow.Aplicacao.csproj src/UBSFlow.Aplicacao/
COPY src/UBSFlow.Dominio/UBSFlow.Dominio.csproj src/UBSFlow.Dominio/
COPY src/UBSFlow.Infraestrutura/UBSFlow.Infraestrutura.csproj src/UBSFlow.Infraestrutura/
COPY tests/UBSFlow.Testes/UBSFlow.Testes.csproj tests/UBSFlow.Testes/

RUN dotnet restore

COPY . .
RUN dotnet publish src/UBSFlow.Api/UBSFlow.Api.csproj --configuration Release --output /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "UBSFlow.Api.dll"]
