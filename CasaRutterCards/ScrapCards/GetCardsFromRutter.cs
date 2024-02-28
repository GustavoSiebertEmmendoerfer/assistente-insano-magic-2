using System.Net;
using CasaRutterCards.Entities;
using CasaRutterCards.Infra;
using CasaRutterCards.Utils;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CasaRutterCards;

public class GetCardsFromRutter : IGetCardsFromRutter
{
    private readonly AppDbContext _context;
    private readonly ICardsRepository _cardsRepository;
    public GetCardsFromRutter(AppDbContext context, ICardsRepository cardsRepository)
    {
        _context = context;
        _cardsRepository = cardsRepository;
    }
    
    private string[] columns = { "Edição", "Idioma", "Qualidade", "Extras", "Estoque"};

    public async Task Execute(int startValue, int endValue)
    { 
        int totalNumbers = endValue - startValue + 1; // Total de números no intervalo
        double percentIncrement = 100.0 / totalNumbers; // Porcentagem de incremento para cada número

        for(int rutterCode = startValue; rutterCode <= endValue; rutterCode++)
        {
            ConsoleProgress(rutterCode, percentIncrement);

            await SaveCardByRutterCode(rutterCode);
        }
        
        void ConsoleProgress(int i, double percentIncrement)
        {
            Console.Clear();
            var currentPercent = Math.Round(((i - startValue + 1) * percentIncrement));
            Console.WriteLine($"Card Progress of: %{currentPercent}");
            Console.WriteLine($"Cards range from - {i} to {endValue}");
        }

    }

    private async Task GetExistingCardValues(string[] values, Card card)
    {
        try
        {
            var dic = dicionaryWithValues(values, out var pricesValues);
            
            var editionName = dic.GetValue((int)CardColumnEnum.Edition);
            
            var edition = await GetEditionAsync(editionName);
            
            var item = card.CardItems.FirstOrDefault(x => x.Edition.Name == edition.Name);
            
            if (item == null)
            {
                var quantity = dic.GetValue((int)CardColumnEnum.Quantity);

                var quality = dic.GetValue((int)CardColumnEnum.Quality);

                var extra = dic.GetValue((int)CardColumnEnum.Extra);
                
                item = new CardItem(edition.Id, int.Parse(quantity.Replace("unid.", "")), quality, extra);
                
                SetPrices(pricesValues, item);
                
                card.AddCardItem(item); 
                _context.Cards.Update(card);
                await _context.SaveChangesAsync();
            }
            
            SetPrices(pricesValues, item);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private async Task GetNewCardValues(string [] values, Card card)
    {
        try
        {
            var dic = dicionaryWithValues(values, out var pricesValues);

            var editionName = dic.GetValue((int)CardColumnEnum.Edition);

            var edition = await GetEditionAsync(editionName);
            
            var quantity = dic.GetValue((int)CardColumnEnum.Quantity);

            var quality = dic.GetValue((int)CardColumnEnum.Quality);

            var extra = dic.GetValue((int)CardColumnEnum.Extra);
            
            var item = new CardItem(edition.Id, int.Parse(quantity.Replace("unid.", "")), quality, extra);
            
            SetPrices(pricesValues, item);
            
            card.AddCardItem(item);
            _context.Cards.Update(card);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private Dictionary<string, string> dicionaryWithValues(string[] values, out List<string> pricesValues)
    {
        var dic = new Dictionary<string, string>();
        var realValues = values.Where(x => !columns.Contains(x)).ToArray();
        pricesValues = realValues.Where(x => x.StartsWith("R$")).ToList();
        foreach (var value in realValues.Except(pricesValues))
        {
            var column = values[Array.IndexOf(values, value) - 1];
            if(columns.Contains(column))
                dic.Add(column,value);
        }

        return dic;
    }

    private static void SetPrices(List<string> pricesValues, CardItem item)
    {
        try
        {
            var values = pricesValues.Select(x => double.Parse(x.Replace("R$", ""))).ToList();
            item.Prices.Clear();
            foreach (var price in values)
            {
                var isDiscount = pricesValues.Count() > 2 && values.Last() == price;
                var newPrice = new Price(item.Id, price, isDiscount);
                item.AddPrice(newPrice);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task SaveCardByRutterCode(int rutterCode)
    {
        var url = $"https://www.casaderuter.com.br/?view=ecom/item&tcg=1&card={rutterCode}";
        var htmlDoc = await HtmlDocumentUtils.GetHtmlDoc(url);
        try
        {
            if (htmlDoc.DocumentNode.SelectNodes("//div[@class='alertaErro']").Count() < 2)
            {
                var cardDescriptions = htmlDoc.DocumentNode.SelectNodes("//div[@class='table-cards-row']");
                
                var names = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='nomes_cards']").Descendants();
                
                var pop = names
                    .Where(x => x.InnerText != @"\n" || x.InnerText != "" || !string.IsNullOrWhiteSpace(x.InnerText))
                    .Select(x => x.InnerText.Trim()).Distinct().ToList();

                pop.RemoveAll(str => str == "");
                
                var card = await _cardsRepository.GetCardByRutterCode(rutterCode);
                if (card != null)
                {
                    foreach (var description in cardDescriptions)
                    {
                        string[] lines = description.InnerText
                            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                        await GetExistingCardValues(lines, card);
                    }
                }
                else
                {
                    card = pop.Count() > 1 ? new Card(rutterCode, pop.ElementAt(0), pop.ElementAt(1)) : new Card(rutterCode, pop.ElementAt(0), "");
                    await _context.AddAsync(card);
                    await _context.SaveChangesAsync();
                    
                    foreach (var description in cardDescriptions)
                    {
                        
                        string[] lines = description.InnerText
                            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                        await GetNewCardValues(lines, card);
                    }
                }
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            Console.WriteLine(exception.InnerException);
        }
    }

    private async Task<Edition> GetEditionAsync(string name)
    {
        try
        {
            var edition = await _context.Editions.Where(x => x.Name.Contains(name)).FirstOrDefaultAsync();

            if (edition != null) return edition;
            
            edition = new Edition(name);
            await _context.Editions.AddAsync(edition);
            await _context.SaveChangesAsync();

            return edition;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

public interface IGetCardsFromRutter
{
    Task Execute(int startValue, int endValue);
    Task SaveCardByRutterCode(int rutterCode);
}