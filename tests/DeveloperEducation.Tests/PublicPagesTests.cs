using System.Net;

namespace DeveloperEducation.Tests;

[Collection(AppCollection.Name)]
public class PublicPagesTests(AppFactory factory)
{
    [Theory]
    [InlineData("/")]
    [InlineData("/ziel")]
    [InlineData("/zulassung")]
    [InlineData("/pruefungsbereiche")]
    [InlineData("/ablauf")]
    [InlineData("/bewertung")]
    public async Task Public_page_is_reachable_without_login(string url)
    {
        var client = factory.CreateBrowser();

        var resp = await client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }
}
