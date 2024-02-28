using System.Text;
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
        foreach (var cardName in cardNames)
        {
            await _getCardByName.Get(cardName);
            var cardsWithSameName = await _repository.GetCardByName(cardName);

            if (cardsWithSameName.Any() is false)
            {
                await _getCardByName.Get(cardName);
                _response.AppendLine($"Card: {cardName} was not found in database");
            }
            else if (cardsWithSameName.Count > 1)
            {
                _response.AppendLine($"Card: {cardName} has {cardsWithSameName.Count} results");
                foreach (var card in cardsWithSameName)
                {
                    WriteCardSpecs(card, withStock);
                }
            }
            else
            {
                var card = cardsWithSameName.First();
                WriteCardSpecs(card, withStock);
            }
        }

        return _response;
    }

    private void WriteCardSpecs(Card card, bool withStock)
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
        
        _response.AppendLine(WriteRow(card, item));
    }
    
    private CardItem? GetCheapestCard(IEnumerable<CardItem> items)
    {
        return items.MinBy(item => item.Prices
            .Min(price => price.Value));
    }
    
    private string WriteRow(Card card, CardItem item)
    {
        var row = $@"Card: {card.GetName()} - Edition: {item.Edition.Name} "
                  + $"- Quality: {item.Quality} ";

        if (string.IsNullOrWhiteSpace(item.Extra) is false)
            row += $"- Extra: {item.Extra} ";

        if (item.Quantity > 0)
        {
            row += $"- Quantity in stock:{item.Quantity}";
        }
        
        row += $"- Price: {item.Prices.Min(price => price.Value)}";
        
        if (item.Prices.Any(x => x.HasDiscont))
            row += $" (Card With Discount! Original Price: {item.Prices.Max(x => x.Value)})";

        return row;
    }
}

public interface ICardAnalyser
{
    Task<StringBuilder> Analyse(string[] cardNames, bool withStock);
}