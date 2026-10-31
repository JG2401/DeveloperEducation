# DeveloperEducation

Statische Blazor-Infoseite (ASP.NET, .NET 9) zum „Geprüften Berufsspezialisten für Softwareentwicklung“,
zusammengefasst aus der [SEFPrV](https://www.gesetze-im-internet.de/sefprv/BJNR1280E0024.html). Ohne Gewähr.

## Lokal starten

```sh
dotnet run --project src/DeveloperEducation
```

Dann http://localhost:5011 öffnen.

## Datenbank

Die Seite `/notizen` speichert Lernnotizen in SQLite (EF Core). Die Verbindung steht in `appsettings.json`
(`ConnectionStrings:Default`). Lokal liegt die Datei `app.db` im Arbeitsverzeichnis (`src/DeveloperEducation` bei `dotnet run --project`);
sie ist per `.gitignore` ausgeschlossen. Das Schema wird beim Start mit `EnsureCreated()` angelegt.

## Container

```sh
docker build -t developereducation .
docker run --rm -p 8080:8080 -v developereducation-data:/data developereducation
```

Dann http://localhost:8080 öffnen. Bei Push auf `main` baut `.github/workflows/cd.yml` das Image und pusht es nach GHCR.

Im Container liegt die Datenbank unter `/data/app.db`. `/data` sollte als Volume eingebunden werden, sonst gehen die Notizen beim Entfernen des Containers verloren.
