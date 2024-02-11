namespace CasaRutterCards;

public class Edition
{
    public Edition()
    {
        
    }
    
    public int Id { get; set; }
    public int CardId { get; set; }
    public string Name { get; set; } = "";
    public string Language { get; set; } = "";
    public string Quality { get; set; } = "";
    public string Extra { get; set; } = "";
    public int? Quantity { get; set; }
    public List<Price>? Prices { get; set; }

    public Edition(Dictionary<string,string> dictionary)
    {
        if(GetValue(dictionary,0) != null)
            Name = GetValue(dictionary,0);
        if (GetValue(dictionary, 1) != null)
            Language = GetValue(dictionary, 1);
        if(GetValue(dictionary,2) != null)
            Quality = GetValue(dictionary,2);
        if(GetValue(dictionary,3) != null)
            Extra = GetValue(dictionary,3);
        if(GetValue(dictionary,4) != null)
            Quantity = int.Parse(GetValue(dictionary,4).Replace("unid.", ""));

        Prices = new List<Price>();
    }

    public void SetCardId(int id) => CardId = id;

    public void AddPrices(List<Price> prices)
    {
        Prices.AddRange(prices);
    }
    
    public void AddPrice(Price price)
    {
        Prices.Add(price);
    }
    
    string GetValue(Dictionary<string, string> dictionary, int i)
    {
        if (dictionary.Where(x => x.Key == CardColumns.GetKey(i)).Any())
        {
             var lol = dictionary.Where(x => x.Key == CardColumns.GetKey(i)).First().Value;
             return lol;
        }

        return null;
    }

}