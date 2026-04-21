using Moonfire.Ansi;
using Moonfire.Ansi.Models;
using Moonfire.Glyph;
using Moonfire.Rendering.Models;
using Wrath.Views.Abstract;

namespace Wrath.Views;

public class Border : WrapperView
{
    private TerminalContext borderBuffer = new(new(0,0));

    public AnsiTruecolor? BorderColor { set; get; } = null;
    public AnsiTruecolor? BackgroundColor { set; get; } = null;
    
    //Blank
    private static readonly (int id, byte width) Blank = GlyphFactory.GetGlyphIds(" ").First();
    //Corners
    private static readonly (int id, byte width) TopLeft = GlyphFactory.GetGlyphIds("╭").First();
    private static readonly (int id, byte width) TopRight = GlyphFactory.GetGlyphIds("╮").First();
    private static readonly (int id, byte width) BottomLeft = GlyphFactory.GetGlyphIds("╰").First();
    private static readonly (int id, byte width) BottomRight = GlyphFactory.GetGlyphIds("╯").First();
    //Sides
    private static readonly (int id, byte width) Horizontal = GlyphFactory.GetGlyphIds("─").First();
    private static readonly (int id, byte width) Vertical = GlyphFactory.GetGlyphIds("│").First();
    //private static readonly (int id, byte width) TitleLeft = GlyphFactory.GetGlyphIds("╴").First();
    //private static readonly (int id, byte width) TitleRight = GlyphFactory.GetGlyphIds("╶").First();

    protected override Task OnArrange()
    {
        if(SizeX < 1 || SizeY < 1)
            return Task.CompletedTask;

        if(SizeX != borderBuffer.W || SizeY != borderBuffer.H)
            borderBuffer = new(new(SizeX, SizeY));

        if(borderBuffer.H < 2 || borderBuffer.W < 2)
            return Task.CompletedTask;
        
        var templateStyleId = AnsiStyleFactory.GetStyleId((BorderColor, BackgroundColor, AnsiProperty.None));
        TerminalCell blankCell = new(
            Blank.id,
            Blank.width,
            templateStyleId
        );

        TerminalCell horizontalCell = new(Horizontal.id, Horizontal.width, templateStyleId);
        TerminalCell verticalCell = new(Vertical.id, Vertical.width, templateStyleId);
        for (int x = 1; x < SizeX - 1; x++)
            (borderBuffer[x, 0], borderBuffer[x, SizeY - 1]) = (horizontalCell, horizontalCell);
        for (int y = 1; y < SizeY - 1; y++)
            (borderBuffer[0, y], borderBuffer[SizeX - 1, y]) = (verticalCell, verticalCell);
    
        borderBuffer[0, 0] = new(TopLeft.id, TopLeft.width, templateStyleId);
        borderBuffer[SizeX - 1, 0] = new(TopRight.id, TopRight.width, templateStyleId);
        borderBuffer[0, SizeY - 1] = new(BottomLeft.id, BottomLeft.width, templateStyleId);
        borderBuffer[SizeX - 1, SizeY - 1] = new(BottomRight.id, BottomRight.width, templateStyleId);
        
        for (int y = 1; y < SizeY - 1; y++)
            for (int x = 1; x < SizeX - 1; x++)
                borderBuffer[x, y] = blankCell;

        SubView.SizeX = SizeX - 2;
        SubView.SizeY = SizeY - 2;
        SubView.OriginX = OriginX + 1;
        SubView.OriginY = OriginY + 1;

        return Task.CompletedTask;        
    }

    protected override Task OnDraw(TerminalContext context)
    {
        borderBuffer.CopyTo(context);

        return Task.CompletedTask;
    }

    protected override TerminalContext ResizeContext(TerminalContext context) =>
        new(SubView.OriginX, SubView.OriginY, SubView.SizeX, SubView.SizeY, context);
}
