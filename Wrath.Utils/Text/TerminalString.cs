using System.Collections.Immutable;
using Moonfire.Glyph;
using Moonfire.Rendering.Models;

namespace Wrath.Utils.Text;

public class TerminalString
{
    public ImmutableList<ImmutableList<TerminalCell>> Lines { get; private set; }
    
    public TerminalString(List<TerminalStringSegment> segments)
    {
        var lines = ImmutableList.CreateBuilder<ImmutableList<TerminalCell>>();
        var currentLine = ImmutableList.CreateBuilder<TerminalCell>();

        foreach(var segment in segments)
        {
            var parts = segment.Text.Split('\n');

            foreach(var (part, isBreak) in parts.Select((p, i) => (p, i < parts.Length - 1)))
            {
                currentLine.AddRange(GlyphFactory.GetGlyphIds(part)
                    .Select(g => new TerminalCell(g.id, g.width, segment.StyleId)));

                if(isBreak)
                {
                    lines.Add([..currentLine]);
                    currentLine.Clear();
                }
            }

            //Commit line if last part ended with newline
            if(segment.Text.EndsWith('\n'))
            {
                lines.Add([..currentLine]);
                currentLine.Clear();
            }
        }

        //Commit last line if last segment didn't end with newline
        if(currentLine.Count != 0)
            lines.Add([..currentLine]);

        Lines = [..lines];
    }

    public void Write(TerminalContext context, TerminalStringOptions? options = null) =>
        TerminalStringWriter.Write(context, this, options);
}

public static class TerminalStringExtensions
{
    public static void Write(this ImmutableList<TerminalString> list, TerminalContext context, TerminalStringOptions? options = null) =>
        TerminalStringWriter.Write(context, list, options);
}
