namespace CasaRutterCards;

public class Card
{
    public int Id { get; set; }
    public string NamePortuguese { get; set; }
    public string NameEnglish { get; set; }
    public List<Edition> Editons { get; set; }

    public Card(int id, string namePortuguese, string nameEnglish)
    {
        Id = id;
        NamePortuguese = namePortuguese;
        NameEnglish = nameEnglish;
        Editons = new List<Edition>();
    }

    public Card()
    {
        
    }
    public void AddEdition(Edition edition)
    {
        edition.SetCardId(Id);
        Editons.Add(edition);
    }
}