using System.Collections.Immutable;
using Moonfire.Rendering.Models;

namespace Wrath.Utils.Text;

public static class TerminalStringWriter
{
    public static void Write(TerminalContext context, ImmutableList<TerminalString> text, TerminalStringOptions? options = null)
    {
        options ??= new();

        //Virtualized draw, draw based on options and provided text
    }

    public static void Write(TerminalContext context, TerminalString text, TerminalStringOptions? options = null)
    {
        options ??= new();

        //Virtualized draw, draw based on options and provided text
    }
}