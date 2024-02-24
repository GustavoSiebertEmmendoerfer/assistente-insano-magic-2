using CasaRutterCards.Entities.Base;
namespace CasaRutterCards.Entities;
public class Price : EntityBase
{
    public int CardItemId { get; set; }
    public double? Value { get; set; }
    public bool HasDiscont { get; set; }

    public Price() { }
    public Price(int cardItemId, double? value, bool hasDiscont) : base()
    {
        CardItemId = cardItemId;
        Value = value;
        HasDiscont = hasDiscont;
    }

    public void UpdatePrice(double? newValue)
        => Value = newValue;
}