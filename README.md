# DeveloperEducation

Statische Blazor-Infoseite (ASP.NET, .NET 9) zum „Geprüften Berufsspezialisten für Softwareentwicklung“,
zusammengefasst aus der [SEFPrV](https://www.gesetze-im-internet.de/sefprv/BJNR1280E0024.html). Ohne Gewähr.

## Lokal starten

```sh
dotnet run --project src/DeveloperEducation
```

Dann http://localhost:5011 öffnen.

## Container

```sh
docker build -t developereducation .
docker run --rm -p 8080:8080 developereducation
```

Dann http://localhost:8080 öffnen. Bei Push auf `main` baut `.github/workflows/cd.yml` das Image und pusht es nach GHCR.
