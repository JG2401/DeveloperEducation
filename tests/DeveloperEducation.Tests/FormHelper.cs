using System.Net;
using System.Text.RegularExpressions;

namespace DeveloperEducation.Tests;

/// <summary>
/// Posts a static-SSR form the way a browser would: GET the page, copy every hidden field
/// (antiforgery token, _handler) out of the HTML, add the user's values, POST it back.
/// </summary>
public static class FormHelper
{
    private static readonly Regex FormRx = new(@"<form\b[^>]*>.*?</form>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
    private static readonly Regex FieldRx = new(@"<(?:input|textarea)\b[^>]*>", RegexOptions.IgnoreCase);
    private static readonly Regex AttrRx = new("([\\w-]+)\\s*=\\s*\"([^\"]*)\"");

    private static Dictionary<string, string> Attrs(string tag) =>
        AttrRx.Matches(tag).ToDictionary(m => m.Groups[1].Value.ToLowerInvariant(), m => WebUtility.HtmlDecode(m.Groups[2].Value), StringComparer.Ordinal);

    /// <summary>
    /// Finds the form whose action or hidden _handler equals <paramref name="formKey"/>, and builds the
    /// POST body: all hidden inputs plus <paramref name="valuesById"/> mapped to the real field names.
    /// </summary>
    public static Dictionary<string, string> BuildBody(string html, string formKey, IDictionary<string, string> valuesById)
    {
        foreach (Match form in FormRx.Matches(html))
        {
            var openTag = form.Value[..(form.Value.IndexOf('>') + 1)];
            var fields = FieldRx.Matches(form.Value).Select(m => Attrs(m.Value)).ToList();
            var hidden = fields
                .Where(a => a.GetValueOrDefault("type") == "hidden" && a.ContainsKey("name"))
                .ToDictionary(a => a["name"], a => a.GetValueOrDefault("value") ?? "");

            var matches = Attrs(openTag).GetValueOrDefault("action") == formKey
                || hidden.GetValueOrDefault("_handler") == formKey;
            if (!matches) continue;

            var body = new Dictionary<string, string>(hidden);
            foreach (var (id, value) in valuesById)
            {
                var field = fields.FirstOrDefault(a => a.GetValueOrDefault("id") == id)
                    ?? throw new InvalidOperationException($"Form '{formKey}' has no field with id '{id}'.");
                body[field["name"]] = value;
            }
            return body;
        }
        throw new InvalidOperationException($"No form '{formKey}' found in page:\n{html}");
    }

    /// <summary>GET <paramref name="pageUrl"/>, then POST the form <paramref name="formKey"/> back to <paramref name="postUrl"/> (default: same URL).</summary>
    public static async Task<HttpResponseMessage> SubmitAsync(
        HttpClient client, string pageUrl, string formKey, IDictionary<string, string> valuesById, string? postUrl = null)
    {
        var page = await client.GetAsync(pageUrl);
        var html = await page.Content.ReadAsStringAsync();
        Assert.True(page.IsSuccessStatusCode, $"GET {pageUrl} returned {(int)page.StatusCode}");

        var body = BuildBody(html, formKey, valuesById);
        return await client.PostAsync(postUrl ?? pageUrl, new FormUrlEncodedContent(body));
    }

    public static Task<HttpResponseMessage> RegisterAsync(HttpClient client, string email, string password) =>
        SubmitAsync(client, "/konto/registrieren", "registrieren", new Dictionary<string, string>
        {
            ["email"] = email,
            ["passwort"] = password,
            ["bestaetigen"] = password,
        });

    public static Task<HttpResponseMessage> LoginAsync(HttpClient client, string email, string password, string url = "/konto/anmelden") =>
        SubmitAsync(client, url, "anmelden", new Dictionary<string, string>
        {
            ["email"] = email,
            ["passwort"] = password,
        });

    public static string NewEmail() => $"qa-{Guid.NewGuid():N}@example.com";

    public const string ValidPassword = "Passwort1";

    /// <summary>Registers a fresh user on <paramref name="client"/> and asserts that it worked (302 to /).</summary>
    public static async Task<string> RegisterNewUserAsync(HttpClient client)
    {
        var email = NewEmail();
        var resp = await RegisterAsync(client, email, ValidPassword);
        Assert.True(resp.StatusCode == HttpStatusCode.Redirect || resp.StatusCode == HttpStatusCode.SeeOther,
            $"Registration returned {(int)resp.StatusCode}:\n{await resp.Content.ReadAsStringAsync()}");
        return email;
    }
}
