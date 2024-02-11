using CasaRutterCards;
using Microsoft.EntityFrameworkCore;

class Program
{ 
    static async Task Main(string[] args)
    {
        // Set your connection string here
        string connectionString = "Host=localhost;Port=5432;Database=Rutter;Username=postgres;Password=postgres;";

        // Initialize the DbContextOptionsBuilder with the connection string
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        var context = new AppDbContext(optionsBuilder.Options);
            Console.WriteLine("Apartir de qual Id começar:");
            var start = int.Parse(Console.ReadLine());
            Console.WriteLine("Qual o Id deve acabar:");
            var end = int.Parse(Console.ReadLine());
            await SaveCards(end, start, context);
        
    }

    public static async Task SaveCards(int indexToGo, int indexToStart, AppDbContext context)
    {
        var cards = new GetCards();
        
        var allCards = await cards.Get(indexToGo, indexToStart);

        var createCards = allCards.Where(cardCreate => !context.Cards.Any(x => x.Id == cardCreate.Id)).Select(x => x);
        var updateCards = allCards.Where(cardUpdate => context.Cards.Any(x => x.Id == cardUpdate.Id)).Select(x => x);


        await context.Cards.AddRangeAsync(createCards);    
        context.Cards.UpdateRange(updateCards);
        await context.SaveChangesAsync();
    }
}