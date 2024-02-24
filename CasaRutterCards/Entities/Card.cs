using CasaRutterCards.Entities.Base;

namespace CasaRutterCards.Entities;

public class Card : EntityBase
{
    public int RutterCode { get; set; }
    public string NamePortuguese { get; set; } = "";
    public string NameEnglish { get; set; } = "";
    public List<CardItem> CardItems { get; set; }

    public Card()
    {
        CardItems = new List<CardItem>();
    }
    public Card(int rutterCode, string namePortuguese, string nameEnglish) : base()
    {
        RutterCode = rutterCode;
        NamePortuguese = namePortuguese;
        NameEnglish = nameEnglish;
        CardItems = new List<CardItem>();
    }
    public string GetName()
    {
        if (string.IsNullOrEmpty(NameEnglish))
        {
            return NamePortuguese;
        }

        return NameEnglish;
    }
    public void AddCardItem(CardItem item)
    {
        item.SetCardId(Id);
        CardItems.Add(item);
    }
}