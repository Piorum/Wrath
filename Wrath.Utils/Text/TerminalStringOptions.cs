namespace Wrath.Utils.Text;

public class TerminalStringOptions((int x, int y)? offset = null, TerminalStringFormat formatting = TerminalStringFormat.None)
{
    public (int X, int Y) Offset = offset ?? (0, 0);
    public TerminalStringFormat Formatting = formatting;
}

[Flags]
public enum TerminalStringFormat
{
    None  = 0,
    Wrap = 1 << 0,
    OverflowEllipsis = 1 << 1, 
    RightAlign = 1 << 2,
    BottomAlign = 1 << 3,
    RightToLeft = 1 << 4
}
