using System.ComponentModel.DataAnnotations;
using DeveloperEducation.Data;

namespace DeveloperEducation.Tests;

public class LernnotizValidationTests
{
    private static List<ValidationResult> Validate(Lernnotiz n)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(n, new ValidationContext(n), results, validateAllProperties: true);
        return results;
    }

    private static bool HasErrorFor(List<ValidationResult> results, string member) =>
        results.Any(r => r.MemberNames.Contains(member));

    [Fact]
    public void Valid_note_has_no_errors() =>
        Assert.Empty(Validate(new Lernnotiz { Titel = "Titel", Text = "Text" }));

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Titel_is_required(string? titel) =>
        Assert.True(HasErrorFor(Validate(new Lernnotiz { Titel = titel!, Text = "Text" }), nameof(Lernnotiz.Titel)));

    [Theory]
    [InlineData(120, false)]
    [InlineData(121, true)]
    public void Titel_is_limited_to_120_chars(int length, bool expectError) =>
        Assert.Equal(expectError, HasErrorFor(Validate(new Lernnotiz { Titel = new string('a', length), Text = "Text" }), nameof(Lernnotiz.Titel)));

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Text_is_required(string? text) =>
        Assert.True(HasErrorFor(Validate(new Lernnotiz { Titel = "Titel", Text = text! }), nameof(Lernnotiz.Text)));

    [Theory]
    [InlineData(2000, false)]
    [InlineData(2001, true)]
    public void Text_is_limited_to_2000_chars(int length, bool expectError) =>
        Assert.Equal(expectError, HasErrorFor(Validate(new Lernnotiz { Titel = "Titel", Text = new string('a', length) }), nameof(Lernnotiz.Text)));
}
