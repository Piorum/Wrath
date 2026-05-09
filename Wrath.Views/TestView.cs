using Moonfire.Ansi;
using Moonfire.Ansi.Models;
using Moonfire.Glyph;
using Moonfire.Rendering.Enums;
using Moonfire.Rendering.Interfaces;
using Moonfire.Rendering.Models;

namespace Wrath.Views;

public class TestView : IRelativeMoonfireView
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public FillStyle FillStyleX { set; get; } = FillStyle.Max;
    public FillStyle FillStyleY { set; get; } = FillStyle.Max;
    public int StaticX  { set; get; } = 1; //1 = 1 Cell
    public int StaticY  { set; get; } = 1; //1 = 1 Cell
    public float PercentX  { set; get; } = 1.0f; //1.0f == 100%
    public float PercentY  { set; get; } = 1.0f; //1.0f == 100%

    public int MinX => FillStyleX == FillStyle.Static ? StaticX : 0;
    public int MinY => FillStyleY == FillStyle.Static ? StaticY : 0;

    public int OriginX { set; get; }
    public int OriginY { set; get; }
    public int SizeX { set; get; }
    public int SizeY { set; get; }

    public bool Dirty { set; get; }


    public Task<bool> Arrange()
    {
        return Task.FromResult(true);
    }

    private static readonly int[] colors = new int[65536];
    private static readonly int[] charId = [GlyphFactory.GetGlyphIds("▀").First().id, GlyphFactory.GetGlyphIds("▄").First().id];
    static TestView()
    {
        Random rng = new();
        for(int i = 0; i < 65536; i++)
        {
            var high = rng.Next(0, 65535);
            var low = rng.Next(0, 65535);
            colors[i] = AnsiStyleFactory.GetStyleId(new(new((byte)high, (byte)high, (byte)high), new((byte)low, (byte)low, (byte)low), AnsiProperty.None));

        };
    }

    private int flip = 0;
    private int iterator = 0;
    public Task Draw(TerminalContext context)
    {
        foreach(ref var cell in context)
            cell = new(charId[flip++ & 1], 1, colors[iterator++ & 0xFFFF]);

        flip++;

        return Task.CompletedTask;
    }

    public Task Invalidate()
    {
        Dirty = true;
        return Task.CompletedTask;
    }
}
