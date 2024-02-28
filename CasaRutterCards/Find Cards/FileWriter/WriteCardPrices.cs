using System.Text;

namespace CasaRutterCards.Find_Cards.FileWriter;

public class WriteCardPrices
{
    public async Task Write(string path, StringBuilder contet)
    {
        await using var writer = new StreamWriter(path + "Cards.txt");
            await writer.WriteAsync(contet.ToString());
    }
}