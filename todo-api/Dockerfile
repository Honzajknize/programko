FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/TodoApi/*.csproj src/TodoApi/
RUN dotnet restore src/TodoApi
COPY . .
RUN dotnet publish src/TodoApi -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "TodoApi.dll"]
