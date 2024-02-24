namespace CasaRutterCards.Utils;

public static class ValuesFromDictionary
{
    public static string GetValue(this Dictionary<string, string> dictionary, int i)
    {
        if (dictionary.Where(x => x.Key == CardColumns.GetKey(i)).Any())
        {
            var lol = dictionary.Where(x => x.Key == CardColumns.GetKey(i)).First().Value;
            return lol;
        }

        return null;
    }
}