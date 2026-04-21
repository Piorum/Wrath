using Moonfire.Ansi;
using Moonfire.Ansi.Models;

namespace Wrath.Utils.Text;

public class TerminalStringSegment(string text, AnsiStyleData? style = null)
{
    private const int tabWidth = 4;
    private static readonly string tabString = new(' ', tabWidth);
    
    private static readonly int templateStyleId = AnsiStyleFactory.GetStyleId((null, null, AnsiProperty.None));

    //Normalize text
    public readonly string Text = text.Replace("\r\n", "\n")
        .Replace("\r", "\n")
        .Replace("\t", tabString);

    //Get style Id
    public readonly int StyleId = style is null 
        ? templateStyleId 
        : AnsiStyleFactory.GetStyleId((style.ForegroundColor, style.BackgroundColor, style.Properties));
}
