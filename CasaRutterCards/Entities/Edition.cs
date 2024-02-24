using CasaRutterCards.Entities.Base;
using CasaRutterCards.Utils;

namespace CasaRutterCards.Entities;

public class Edition : EntityBase
{
    public string Name { get; set; } = "";
    public Edition() { }
    public Edition(string name) : base()
    {
        Name = name;
    }
}