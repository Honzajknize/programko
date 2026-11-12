# TodoApi

Jednoduché REST API pro správu úkolů (todo list) napsané v **C# / .NET 8** jako *minimal API*.
Data se ukládají pouze do paměti – po restartu aplikace zmizí.

Repozitář slouží jako výchozí bod pro cvičení **CI/CD s GitHub Actions**. Pipeline (workflow)
si napíšete sami.

## Struktura repozitáře

```
TodoApi.sln
src/TodoApi/              # webová aplikace (minimal API)
  Program.cs              # registrace služeb a endpointů
  AppInfo.cs              # informace o aplikaci (verze)
  TodoItem.cs             # datové typy (úkol, požadavky)
  TodoStore.cs            # úložiště úkolů v paměti
tests/TodoApi.Tests/      # testy (xUnit)
  TodoStoreTests.cs       # jednotkové testy úložiště
  TodoEndpointsTests.cs   # integrační testy HTTP endpointů
k8s/                      # manifesty pro Kubernetes
Dockerfile                # vícefázový (multi-stage) build image
.editorconfig             # pravidla formátování kódu
```

## Endpointy

| Metoda | Cesta              | Popis                                   | Odpovědi          |
|--------|--------------------|-----------------------------------------|-------------------|
| GET    | `/health`          | stav aplikace a její verze              | 200               |
| GET    | `/api/todos`       | seznam všech úkolů                      | 200               |
| GET    | `/api/todos/{id}`  | jeden úkol                              | 200, 404          |
| POST   | `/api/todos`       | vytvoření úkolu, tělo `{"title": "…"}`  | 201, 400          |
| PUT    | `/api/todos/{id}`  | změna stavu, tělo `{"isCompleted": true}` | 200, 404        |

Příklad odpovědi `GET /health`:

```json
{ "status": "ok", "version": "dev" }
```

Verze aplikace se čte z proměnné prostředí `APP_VERSION`.

## Požadavky

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://docs.docker.com/get-docker/) (pro sestavení image)

## Lokální spuštění

```bash
dotnet restore
dotnet build
dotnet test

dotnet run --project src/TodoApi
```

Aplikace poběží na `http://localhost:5000` (proměnná `APP_VERSION=dev` se nastaví
z `src/TodoApi/Properties/launchSettings.json`). V jiném terminálu:

```bash
curl http://localhost:5000/health
curl -X POST http://localhost:5000/api/todos \
     -H "Content-Type: application/json" \
     -d '{"title": "Naučit se GitHub Actions"}'
curl http://localhost:5000/api/todos
```

## Kontrola formátování

```bash
dotnet format --verify-no-changes
```

Příkaz skončí chybou, pokud kód neodpovídá pravidlům v `.editorconfig`.
Opravu lze provést automaticky příkazem `dotnet format`.

## Pokrytí kódu testy

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Výsledek (formát Cobertura) najdete v `tests/TodoApi.Tests/TestResults/<guid>/coverage.cobertura.xml`.

## Docker

```bash
docker build -t todo-api:dev .
docker run --rm -p 8080:8080 -e APP_VERSION=dev todo-api:dev

curl http://localhost:8080/health
```

> **Otázka k zamyšlení:**
> 1. Proč je `Dockerfile` vícefázový (*multi-stage*)? Co obsahuje výsledný image a co ne?
>    Porovnejte velikost image `mcr.microsoft.com/dotnet/sdk:8.0` a výsledného image.
> 2. Proč se soubor `*.csproj` kopíruje a `dotnet restore` spouští **zvlášť, ještě před**
>    `COPY . .`? Co se stane s dobou buildu při změně jednoho `.cs` souboru, kdybychom
>    kopírovali vše najednou?

## Kubernetes

Ve složce `k8s/` jsou základní manifesty (`Deployment` a `Service`). Image v deploymentu
(`todo-api:placeholder`) je zástupná hodnota, kterou nahradí vaše CD pipeline.

```bash
kubectl apply -f k8s/
```
