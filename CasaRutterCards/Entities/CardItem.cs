using CasaRutterCards.Entities.Base;

namespace CasaRutterCards.Entities;

public class CardItem : EntityBase
{
    public int EditionId { get; set; }
    public int CardId { get; set; }
    public Edition Edition { get; set; }
    public int Quantity { get; set; }
    public List<Price> Prices { get; set; }
    public string Quality { get; set; } = "";
    public string Extra { get; set; } = "";

    public CardItem() { }
    public CardItem(int editionId, int quantity, string quality, string extra) : base()
    {
        EditionId = editionId;
        Quantity = quantity;
        if(!string.IsNullOrEmpty(quality))
            Quality = quality;
        if(!string.IsNullOrEmpty(extra))
            Extra = extra;
        Prices = new List<Price>();
    }
    
    public CardItem(int quantity, string quality, string extra) : base()
    {
        Quantity = quantity;
        if(!string.IsNullOrEmpty(quality))
            Quality = quality;
        if(!string.IsNullOrEmpty(extra))
            Extra = extra;
        Prices = new List<Price>();
    }

    public void SetCardId(int id) => CardId = id;
    public void AddPrice(Price price) => Prices.Add(price);

    public void UpdatePrice(Price price) => Prices.FirstOrDefault(x => price.Id == x.Id)!.UpdatePrice(price.Value);
}