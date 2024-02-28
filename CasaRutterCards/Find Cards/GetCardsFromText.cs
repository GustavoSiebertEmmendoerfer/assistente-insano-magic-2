using CasaRutterCards.Find_Cards.FileReader;
using CasaRutterCards.Find_Cards.FileWriter;
using CasaRutterCards.Find_Cards.CardAnalyser;
using CasaRutterCards.Infra;

namespace CasaRutterCards.Find_Cards;

public class GetCardsFromText : IGetCardsFromText
{
    private readonly string _rootPath;
    private readonly ReadCardsFromText _reader;
    private readonly WriteCardPrices _writer;
    private readonly ICardAnalyser _analyser;

    public GetCardsFromText(ICardAnalyser cardAnalyser)
    {
        _rootPath = Directory.GetParent(Directory.GetCurrentDirectory())?.Parent?.Parent?.FullName + "/";
        _reader= new ReadCardsFromText();
        _writer = new WriteCardPrices();
        _analyser = cardAnalyser;
    }
    
    public async Task WriteCardPrices(bool withStock)
    {
        var cards  = await _reader.Read(_rootPath);
        var result = await _analyser.Analyse(cards, withStock);
        await _writer.Write(_rootPath, result);
        Console.WriteLine(result);
    }
}

public interface IGetCardsFromText
{
    Task WriteCardPrices(bool withStock);
}