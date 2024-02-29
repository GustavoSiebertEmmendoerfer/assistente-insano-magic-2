namespace CasaRutterCards;

public static class CardColumns
{
    public static string GetKey(int value)
    {
        switch ((CardColumnEnum)value)
        {
            case CardColumnEnum.Edition: return "Edição";
            case CardColumnEnum.Quality: return "Qualidade";
            case CardColumnEnum.Extra: return "Extras";
            case CardColumnEnum.Language: return "Idioma";
            case CardColumnEnum.Price: return "Preço";
            case CardColumnEnum.Quantity: return "Estoque";
            default: return "";
        }
    }
}