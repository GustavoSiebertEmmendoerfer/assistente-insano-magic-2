using System.Net;
using HtmlAgilityPack;

namespace CasaRutterCards;

public class GetCards
{
    private string[] columns = { "Edição", "Idioma", "Qualidade", "Extras", "Estoque"};
    public async Task<List<Card>> Get(int index, int startValue)
    {
        List<Card> cards = new();
        for(int i = startValue; i<=index; i++)
        {
            var url = $"https://www.casaderuter.com.br/?view=ecom/item&tcg=1&card={i}";
            Console.Clear();
            var progress = ((double)i / index) * 100;
            Console.WriteLine($"Card Progress of: %{progress}");
            Console.WriteLine($"Cards range from - {i} to {index}");
            var client = new HttpClient();

            var response = await client.GetAsync(url);

            var html = await response.Content.ReadAsStringAsync();

            var htmlDoc = new HtmlDocument();

            htmlDoc.LoadHtml(WebUtility.HtmlDecode(html));

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

                    Card card;

                    if (pop.Count() > 1)
                        card = new Card(i, pop.ElementAt(0), pop.ElementAt(1));
                    else
                        card = new Card(i, pop.ElementAt(0), "");
                    
                    foreach (var description in cardDescriptions)
                    {
                        string[] lines = description.InnerText
                            .Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                            .Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToArray();
                        GetValues(lines, card);
                    }

                    cards.Add(card);
                }
                else
                {
                    break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        return cards;
    }
    public void GetValues(string [] values, Card card)
    { 
        var dic = new Dictionary<string, string>();
        var realValues = values.Where(x => !columns.Contains(x)).ToArray();
        var pricesValues = realValues.Where(x => x.StartsWith("R$")).ToList();
        foreach (var value in realValues.Except(pricesValues))
        {
            var column = values[Array.IndexOf(values, value) - 1];
            if(columns.Contains(column))
                dic.Add(column,value);
        }

        var edition = new Edition(dic);
        
        var prices = new List<Price>();
        foreach (var price in pricesValues)
        {
            var priceString = price.Substring(2, price.Length - 2);
            
            var value = double.Parse(priceString);
            
            var newPrice = new Price(edition.Id, value, pricesValues.Last() == price);
            edition.AddPrice(newPrice);
        }
        card.AddEdition(edition);
    }
}