using Moonfire.Rendering.Enums;
using Moonfire.Rendering.Interfaces;
using Moonfire.Rendering.Models;

namespace Wrath.Views.Abstract;

public abstract class LeafView : IRelativeMoonfireView
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public FillStyle FillStyleX { set; get; } = FillStyle.Max;
    public FillStyle FillStyleY { set; get; } = FillStyle.Max;
    public int StaticX { set; get; } = 1; //1 = 1 Cell
    public int StaticY { set; get; } = 1; //1 = 1 Cell
    public float PercentX { set; get; } = 1.0f; //1.0f == 100%
    public float PercentY { set; get; } = 1.0f; //1.0f == 100%

    public int OriginX { set; get; }
    public int OriginY { set; get; }
    public int SizeX { set; get; }
    public int SizeY { set; get; }

    public int MinX => minX;
    public int MinY => minY;

    protected int minX = 1;
    protected int minY = 1;

    public bool Dirty { set; get; }

    public async Task<bool> Arrange()
    {
        if (Dirty)
        {
            await OnArrange();

            Dirty = false;

            return true;
        }

        return false;
    }

    protected abstract Task OnArrange();

    public async Task Draw(TerminalContext context)
    {
        await OnDraw(context);
    }

    protected abstract Task OnDraw(TerminalContext context);

    public Task Invalidate()
    {
        Dirty = true;
        return Task.CompletedTask;
    }
}
