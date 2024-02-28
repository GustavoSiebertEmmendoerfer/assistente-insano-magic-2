namespace CasaRutterCards.Find_Cards.FileReader;

public class ReadCardsFromText
{
    public async Task<string[]>  Read(string path)
    {
        using (var reader = new StreamReader(path + "CardsDesire.txt"))
        {
            var text = await reader.ReadToEndAsync();
            var textList = text.Split('\n');
            return textList;
        }
    }
}