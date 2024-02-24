using CasaRutterCards.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CasaRutterCards.Find_Cards;

public class GetCardsFromTxt
{
    private double totalValue = 0.0;
    private int quantityOfCards = 0;
    private readonly AppDbContext _context;
    private readonly string RootPath; 
    public GetCardsFromTxt(AppDbContext context)
    {
        RootPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "/";
        _context = context;
    }
    public async Task GetCards()
    {
        var cardNames = await ReadCardsDesire();
        var cardRutterIds = await GetCardsIdsByName(cardNames);
        await WriteCardPrices(cardRutterIds);
    }

    
    
    public async Task<string[]>  ReadCardsDesire()
    {
        using (var reader = new StreamReader(RootPath + "CardsDesire.txt"))
        {
            var text = await reader.ReadToEndAsync();
            var textList = text.Split('\n');
            return textList;
        }
    }

    public async Task WriteCardPrices(int[] cardRutterIds)
    {
        using (var streamWritter = new StreamWriter(RootPath + "/Cards.txt"))
        {
            foreach (var cardRutterId in cardRutterIds)
            {
                var card = await _context.Cards
                    .Where(card => card.Id == cardRutterId)
                    .Include(x => x.CardItems)
                    .ThenInclude(x => x.Prices)
                    .Include(x => x.CardItems)
                    .ThenInclude(x => x.Edition)
                    .FirstOrDefaultAsync();

                if (card != null)
                {
                    Write(card,streamWritter);
                }
            }
            streamWritter.WriteLine($"Number of cards with prices:{quantityOfCards}");
            streamWritter.WriteLine($"Total Value:{totalValue}");

        }
    }

    private void Write(Card card, StreamWriter writer)
    {
        if (card.CardItems.Any())
        {
            var items = card.CardItems.Where(x => x.Prices != null && x.Prices.Any()).ToList();
            if (items.Any())
            {
                var item = GetCheapestCard(items);
                writer.WriteLine(WriteRow(card, item));

                totalValue += (double)item.Prices.Min(p => p.Value);
                
                quantityOfCards++;
            }
        }
        else
        {
            writer.WriteLine(WriteRowForOutOfStock(card));
        }
    }
    
    private CardItem? GetCheapestCard(IEnumerable<CardItem> items)
    {
        return items.MinBy(item => item.Prices
            .Min(price => price.Value));
    }

    private string WriteRowForOutOfStock(Card card)
    {
        return $"Card: {card.GetName()} not found in the DB";
    }
    private string WriteRow(Card card, CardItem item)
    {
        var row = $@"Card: {card.GetName()} - Edition: {item.Edition.Name} "
        + $"Quality: {item.Quality} ";

        if (string.IsNullOrWhiteSpace(item.Extra) is false)
            row += $"Extra: {item.Extra} ";

        if (item.Quantity > 1)
        {
            row += $"Quantity in stock:{item.Quantity} - ";
        }
        
        row += $"Price: {item.Prices.Min(price => price.Value)}";
        
        if (item.Prices.Any(x => x.HasDiscont))
            row += $" (Card With Discount! Original Price: {item.Prices.Max(x => x.Value)})";

        return row;
    }
    
    public async Task<int[]> GetCardsIdsByName(string[] cardsDesire)
    {
        try
        {
            var ids = new List<int>();
            
            foreach (var cardDesire in cardsDesire)
            {
                var card = await _context.Cards
                                    .Where(x => x.NameEnglish.Trim().ToLower() == cardDesire.Trim().ToLower() 
                                                || x.NamePortuguese.Trim().ToLower() == cardDesire.Trim().ToLower())
                                    .ToListAsync(); 
                 
                if(card.Any() is false)
                    Console.WriteLine($"Name:{cardDesire} Not Found");
                else if (card.Count > 1)
                {
                    
                }
                ids.AddRange(card.Select(x => x.Id));
            }

            return ids.ToArray();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}