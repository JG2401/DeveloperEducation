using System.Net;

namespace DeveloperEducation.Tests;

[Collection(AppCollection.Name)]
public class NotizenTests(AppFactory factory)
{
    [Fact]
    public async Task Notizen_are_listed_newest_first()
    {
        var client = factory.CreateBrowser();
        await FormHelper.RegisterNewUserAsync(client);
        var older = $"Aeltere-{Guid.NewGuid():N}";
        var newer = $"Neuere-{Guid.NewGuid():N}";

        foreach (var titel in new[] { older, newer })
        {
            var resp = await FormHelper.SubmitAsync(client, "/notizen", "neue-notiz", new Dictionary<string, string>
            {
                ["titel"] = titel,
                ["text"] = "Inhalt",
            });
            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        }

        var html = await (await client.GetAsync("/notizen")).Content.ReadAsStringAsync();
        var iOlder = html.IndexOf(older, StringComparison.Ordinal);
        var iNewer = html.IndexOf(newer, StringComparison.Ordinal);
        Assert.True(iOlder >= 0 && iNewer >= 0, "Both notes should be rendered");
        Assert.True(iNewer < iOlder, "Newest note must come first");
    }

    [Fact]
    public async Task Empty_titel_is_not_saved()
    {
        var client = factory.CreateBrowser();
        await FormHelper.RegisterNewUserAsync(client);
        var text = $"OhneTitel-{Guid.NewGuid():N}";

        await FormHelper.SubmitAsync(client, "/notizen", "neue-notiz", new Dictionary<string, string>
        {
            ["titel"] = "",
            ["text"] = text,
        });

        var html = await (await client.GetAsync("/notizen")).Content.ReadAsStringAsync();
        Assert.DoesNotContain(text, html);
    }
}
