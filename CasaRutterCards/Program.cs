using CasaRutterCards;
using CasaRutterCards.Find_Cards;
using CasaRutterCards.Find_Cards.CardAnalyser;
using CasaRutterCards.Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Spectre.Console;

class Program
{
    private readonly AppDbContext _context;
    private readonly IGetCardsFromText _getCardsFromText;
    private readonly IGetCardByName _getCardByName;
    private readonly IGetCardsFromRutter _getCardsFromRutter;

    public Program(AppDbContext context, IGetCardsFromText getCardsFromText, 
        IGetCardByName getCardByName,IGetCardsFromRutter getCardsFromRutter)
    {
        _context = context;
        _getCardsFromText = getCardsFromText;
        _getCardByName = getCardByName;
        _getCardsFromRutter = getCardsFromRutter;
    }
    
    static async Task Main(string[] args)
    {
        // Set up dependency injection container
        var services = new ServiceCollection();
        ConfigureServices(services);
        
        var serviceProvider = services.BuildServiceProvider();
        
        // Resolve and run the program
        var program = serviceProvider.GetRequiredService<Program>();
        await program.RunAsync();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Set your connection string here
        string connectionString = "Host=localhost;Port=5432;Database=Rutter;Username=postgres;Password=postgres;";

        // Register DbContext
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ICardsRepository, CardsRepository>();
        services.AddScoped<ICardAnalyser, CardAnalyser>();
        services.AddScoped<IGetCardsFromText, GetCardsFromText>();
        services.AddScoped<IGetCardByName, GetCardByName>();
        services.AddScoped<IGetCardsFromRutter, GetCardsFromRutter>();
        
        // Register the program itself
        services.AddTransient<Program>();
    }
    
    private async Task RunAsync()
    {
        string choise = " ";
        while (choise != "sair")
        {
            
            choise =  AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Card Analyser")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to reveal the options)[/]")
                    .AddChoices(new[] {
                        "Ler cartas", "Atualizar banco com cartas da rutter", "Pegar carta por nome","sair"
                    }));

            switch (choise)
            {
                case "Ler cartas":
                    Console.WriteLine("Somente cartas com estoque na loja? (S/N)");
                    var withStock = Console.ReadLine().ToLower() == "s";            
                    await _getCardsFromText.WriteCardPrices(withStock);
                    ContinueOperation();
                    break;
                case "Atualizar banco com cartas da rutter":
                    Console.WriteLine("Apartir de qual Id começar:");
                    var start = int.Parse(Console.ReadLine());
                    Console.WriteLine("Qual o Id deve acabar:");
                    var end = int.Parse(Console.ReadLine());
                    await _getCardsFromRutter.Execute(start, end);
                    ContinueOperation();
                    break;
                case "Pegar carta por nome":
                    Console.WriteLine("Card Name:");
                    var i = Console.ReadLine();
                    var cards = await _getCardByName.Get(i);
                    Console.WriteLine(JsonConvert.SerializeObject(cards, Formatting.Indented));
                    ContinueOperation();
                    break;
                default:
                    Console.WriteLine("Opção não possui resultado");
                    break;
            }
            Console.Clear();
        }
    }

    private static void ContinueOperation()
    {
        Console.WriteLine("Digite qualquer coisa para continuar.");
        Console.ReadLine();
    }
}