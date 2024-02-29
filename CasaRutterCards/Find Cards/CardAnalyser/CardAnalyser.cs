using System.Text;
using System.Xml.Linq;
using CasaRutterCards.Entities;
using CasaRutterCards.Infra;

namespace CasaRutterCards.Find_Cards.CardAnalyser;

public class CardAnalyser : ICardAnalyser
{
    private readonly ICardsRepository _repository;
    private readonly StringBuilder _response;
    private readonly IGetCardByName _getCardByName;
    
    public CardAnalyser(ICardsRepository cardsRepository, IGetCardByName getCardByName)
    {
        _repository = cardsRepository;
        _response = new StringBuilder();
        _getCardByName = getCardByName;
    }

    public async Task<StringBuilder> Analyse(string[] cardNames, bool withStock)
    {
        var dic = new Dictionary<string, int>();
        foreach (var x in cardNames)
        {
            int value = 1;
            var name = x;
            
            var indexEmpty = x.IndexOf(' ');
            if (indexEmpty > 0)
            {
                var indexNumber = x.Substring(0, indexEmpty);
                if (int.TryParse(indexNumber, out value))
                {
                    name = x.Substring(indexEmpty);
                }
            }
            
            dic.Add(name, value);
        }
        
        foreach (var keyValue in dic)
        {
            await _getCardByName.Get(keyValue.Key);
            var cardsWithSameName = await _repository.GetCardByName(keyValue.Key);

            if (cardsWithSameName.Any() is false)
            {
                await _getCardByName.Get(keyValue.Key);
                _response.AppendLine($"Card: {keyValue} was not found in database");
            }
            else if (cardsWithSameName.Count > 1)
            {
                _response.AppendLine($"Card: {keyValue} has {cardsWithSameName.Count} results");
                foreach (var card in cardsWithSameName)
                {
                    WriteCardSpecs(card, withStock, keyValue.Value);
                }
            }
            else
            {
                var card = cardsWithSameName.First();
                WriteCardSpecs(card, withStock, keyValue.Value);
            }
        }

        return _response;
    }

    private void WriteCardSpecs(Card card, bool withStock, int quantity)
    {
        var cardItems = card.CardItems.Where(x => x.Prices.Any()).ToList();
         
        if(withStock)
            cardItems = cardItems.Where(x => x.Quantity > 0).ToList();

        if (!cardItems.Any())
        {
            if (withStock)
                _response.AppendLine($"Card: {card.GetName()} - doesnt have any prices [With Stock]");
            else
                _response.AppendLine($"Card: {card.GetName()} - doesnt have any prices");
            return;
        }
        
        var item = GetCheapestCard(cardItems);
        
        _response.AppendLine(WriteRow(card, item, quantity));
    }
    
    private CardItem? GetCheapestCard(IEnumerable<CardItem> items)
    {
        return items.MinBy(item => item.Prices
            .Min(price => price.Value));
    }
    
    private string WriteRow(Card card, CardItem item, int quantity)
    {
        var row = $@"Card: {card.GetName()} | Edition: {item.Edition.Name} "
                  + $"| Quality: {item.Quality} ";

        if (string.IsNullOrWhiteSpace(item.Extra) is false)
            row += $"| Extra: {item.Extra} ";

        if (item.Quantity > 0)
        {
            row += $"| Quantity in stock:{item.Quantity} ";
        }
        
        row += $"|Price: {item.Prices.Min(price => price.Value) * quantity}";
        
        if (item.Prices.Any(x => x.HasDiscont))
            row += $"|  (Card With Discount! Original Price: {item.Prices.Max(x => x.Value)})";

        return row;
    }
}

public interface ICardAnalyser
{
    Task<StringBuilder> Analyse(string[] cardNames, bool withStock);
}
