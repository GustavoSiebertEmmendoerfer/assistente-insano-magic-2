using CasaRutterCards.Entities;
using CasaRutterCards.Infra;
using CasaRutterCards.Utils;

namespace CasaRutterCards;

public class GetCardByName : IGetCardByName
{
    private readonly ICardsRepository _cardsRepository;
    private readonly IGetCardsFromRutter _getCardsFromRutter;

    public GetCardByName(ICardsRepository cardsRepository, IGetCardsFromRutter getCardsFromRutter)
    {
        _cardsRepository = cardsRepository;
        _getCardsFromRutter = getCardsFromRutter;
    }
    
    public async Task<List<Card>> Get(string name)
    {
        Console.WriteLine($"Trying to get lastest version of Card:{name}");
        
        var url = $" https://www.casaderuter.com.br/?view=ecom%2Fitens&busca={name.Replace(' ', '+')}&btnEnviar=1";

        var html = await HtmlDocumentUtils.GetHtmlDoc(url);

        if (html.DocumentNode.SelectNodes("//div[@class='cards']") != null && html.DocumentNode.SelectNodes("//div[@class='cards']").Any())
        {
            var items = html.DocumentNode.SelectNodes("//div[@class='card-item']");
            foreach (var item in items)
            {
                var linkCard = item.Descendants().Select(x => x.GetAttributeValue("href", "")).FirstOrDefault(link => link.Contains("card="));
                int lastIndex = linkCard.LastIndexOf('=');
                var code = int.Parse(linkCard.Substring(lastIndex + 1));
                await _getCardsFromRutter.SaveCardByRutterCode(code);
            }
        }
        else if(html.DocumentNode.SelectNodes("//div[@class='alertaErro']").Count() < 2)
        {
            var linkCard = html.DocumentNode.SelectNodes("//meta[@property='og:url']").Select(x => x.GetAttributeValue("content", "")).FirstOrDefault();
            int lastIndex = linkCard.LastIndexOf('=');
            var code = int.Parse(linkCard.Substring(lastIndex + 1));
            await _getCardsFromRutter.SaveCardByRutterCode(code);
        }
        else
        {
            Console.WriteLine($"Card Not found: {name}");
        }

        return await _cardsRepository.GetCardByName(name);
    }
}

public interface IGetCardByName
{
    Task<List<Card>> Get(string name);
}