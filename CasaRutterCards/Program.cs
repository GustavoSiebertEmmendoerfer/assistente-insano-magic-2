using CasaRutterCards;
using CasaRutterCards.Find_Cards;
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
        Console.WriteLine("Deseja Registrar Cards ou Ler");
        var i = Console.ReadLine();
        if (i == "ler")
        {
            var getCardsFromTxt = new GetCardsFromTxt(context);
            await getCardsFromTxt.GetCards();
        }
        else
        {
            Console.WriteLine("Apartir de qual Id começar:");
            var start = int.Parse(Console.ReadLine());
            Console.WriteLine("Qual o Id deve acabar:");
            var end = int.Parse(Console.ReadLine());
            var getCards = new  GetCards(context);
            await getCards.Execute(end,start);
        }
        
    }
}