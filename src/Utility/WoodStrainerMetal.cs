using System.Text;
using Vintagestory.API.Config;

namespace WoodFruitPresses;

public class WoodStrainerMetal
{
    public string Wood { get; }
    public string Strainer { get; }
    public string Metal { get; }

    public WoodStrainerMetal(string wood, string strainer, string metal)
    {
        Wood = wood;
        Strainer = strainer;
        Metal = metal;
    }

    public bool Full => !string.IsNullOrEmpty(Wood) && !string.IsNullOrEmpty(Strainer) && !string.IsNullOrEmpty(Metal);

    public void OutputTranslatedDescription(StringBuilder dsc)
    {
        dsc.Append(Lang.Get("blockmaterial-Wood"));
        dsc.Append(": ");
        dsc.Append(Lang.Get($"material-{Wood}"));
        dsc.Append(" / ");
        dsc.AppendLine(Lang.Get($"material-{Strainer}"));

        dsc.Append(Lang.Get("blockmaterial-Metal"));
        dsc.Append(": ");
        dsc.AppendLine(Lang.Get($"material-{Metal}"));
    }

    public override string ToString()
    {
        return Wood + "-" + Strainer + "-" + Metal;
    }
}