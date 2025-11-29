# STAGE 1 — Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["RecebeRequisicaoWebhook/RecebeRequisicaoWebhook.csproj", "./"]
RUN dotnet restore "./RecebeRequisicaoWebhook.csproj"

COPY RecebeRequisicaoWebhook/ .
RUN dotnet publish RecebeRequisicaoWebhook.csproj -c Release -o /app/publish --no-restore

# STAGE 2 — Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 80

ENTRYPOINT ["dotnet", "RecebeRequisicaoWebhook.dll"]
