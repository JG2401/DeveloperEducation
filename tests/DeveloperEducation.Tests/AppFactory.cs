using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;

namespace DeveloperEducation.Tests;

/// <summary>
/// Hosts the real app against a throwaway SQLite file. The connection string is passed as an
/// environment variable because Program.cs reads it eagerly while building the host.
/// One instance is shared by all integration tests (see <see cref="AppCollection"/>).
/// </summary>
public sealed class AppFactory : WebApplicationFactory<Program>
{
    private const string EnvName = "ConnectionStrings__Default";

    private readonly string _dbPath = Path.Combine(Path.GetTempPath(), $"de-tests-{Guid.NewGuid():N}.db");
    private readonly string? _previousEnv;

    public AppFactory()
    {
        _previousEnv = Environment.GetEnvironmentVariable(EnvName);
        Environment.SetEnvironmentVariable(EnvName, $"Data Source={_dbPath}");
    }

    /// <summary>
    /// DEFECT WORKAROUND: Program.cs calls app.UseAuthorization() but never registers
    /// services.AddAuthorization(), so the host throws InvalidOperationException on startup
    /// (seen in CI). Registering it here lets the rest of the suite run without changing app code.
    /// Remove once the app registers authorization itself.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
        => builder.ConfigureServices(services => services.AddAuthorization());

    /// <summary>A browser-like client: keeps cookies, does not follow redirects.</summary>
    public HttpClient CreateBrowser() => CreateClient(new WebApplicationFactoryClientOptions
    {
        AllowAutoRedirect = false,
        HandleCookies = true,
    });

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing) return;

        Environment.SetEnvironmentVariable(EnvName, _previousEnv);
        SqliteConnection.ClearAllPools();
        foreach (var f in new[] { _dbPath, _dbPath + "-wal", _dbPath + "-shm" })
        {
            try { File.Delete(f); } catch (IOException) { }
        }
    }
}

[CollectionDefinition(Name)]
public sealed class AppCollection : ICollectionFixture<AppFactory>
{
    public const string Name = "app";
}
