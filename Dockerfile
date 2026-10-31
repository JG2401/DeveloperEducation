FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY src/DeveloperEducation/DeveloperEducation.csproj src/DeveloperEducation/
RUN dotnet restore src/DeveloperEducation/DeveloperEducation.csproj
COPY src/ src/
RUN dotnet publish src/DeveloperEducation/DeveloperEducation.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "DeveloperEducation.dll"]
