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

## Registrierung und Anmeldung

Registrierung (`/konto/registrieren`) und Anmeldung (`/konto/anmelden`) nutzen ASP.NET Core Identity mit derselben SQLite-Datenbank. Abmelden erfolgt per POST auf `/konto/abmelden` (Button in der Navigation). `/notizen` erfordert eine Anmeldung; die Notizen bleiben für alle Benutzer gemeinsam. Es gibt keine E-Mail-Bestätigung, keine Rollen und keine Kontoverwaltung.

**Achtung – bestehende Datenbank:** Das Schema wird mit `EnsureCreated()` angelegt (keine Migrationen). Eine vorhandene `app.db` erhält die Identity-Tabellen **nicht**. Vor dem Start dieser Version muss `app.db` gelöscht werden; im Container muss das Volume `developereducation-data` neu angelegt werden (`docker volume rm developereducation-data`). Die bisherigen Notizen gehen dabei verloren.

Bekannte Einschränkung: Im Container läuft die Seite nur über HTTP (Port 8080, keine HTTPS-Weiterleitung); das Anmelde-Cookie wird daher unverschlüsselt übertragen.

## Container

```sh
docker build -t developereducation .
docker run --rm -p 8080:8080 -v developereducation-data:/data developereducation
```

Dann http://localhost:8080 öffnen. Bei Push auf `main` baut `.github/workflows/cd.yml` das Image und pusht es nach GHCR.

Im Container liegt die Datenbank unter `/data/app.db`. `/data` sollte als Volume eingebunden werden, sonst gehen die Notizen beim Entfernen des Containers verloren.
