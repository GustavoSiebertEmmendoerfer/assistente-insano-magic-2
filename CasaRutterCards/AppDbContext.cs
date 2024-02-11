using Microsoft.EntityFrameworkCore;

namespace CasaRutterCards;

public class AppDbContext : DbContext
{
    public AppDbContext() { }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    public DbSet<Card> Cards { get; set; }
    public DbSet<Edition> Editions { get; set; }
    public DbSet<Price> Prices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseNpgsql("Host=localhost;Port=5432;Database=Rutter;Username=postgres;Password=postgres;");
}