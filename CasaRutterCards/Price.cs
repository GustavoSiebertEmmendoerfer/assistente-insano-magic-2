namespace CasaRutterCards;

public class Price
{
    public int Id { get; set; }
    public int EditionId { get; set; }
    public double? Value { get; set; }
    public bool IsDiscont { get; set; }

    public Price()
    {
        
    }
    public Price(int id, double? value, bool isDiscont)
    {
        EditionId = id;
        Value = value;
        IsDiscont = isDiscont;
    }
}