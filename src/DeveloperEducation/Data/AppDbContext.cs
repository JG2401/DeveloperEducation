using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DeveloperEducation.Data;

public class Lernnotiz
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Bitte einen Titel eingeben.")]
    [StringLength(120, ErrorMessage = "Der Titel darf höchstens 120 Zeichen lang sein.")]
    public string Titel { get; set; } = "";

    [Required(ErrorMessage = "Bitte einen Text eingeben.")]
    [StringLength(2000, ErrorMessage = "Der Text darf höchstens 2000 Zeichen lang sein.")]
    public string Text { get; set; } = "";

    public DateTimeOffset ErstelltAm { get; set; }
}

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Lernnotiz> Lernnotizen => Set<Lernnotiz>();
}
