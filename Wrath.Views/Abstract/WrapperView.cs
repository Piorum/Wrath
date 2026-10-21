using Moonfire.Rendering.Enums;
using Moonfire.Rendering.Interfaces;
using Moonfire.Rendering.Models;

namespace Wrath.Views.Abstract;

public abstract class WrapperView : IRelativeMoonfireView
{
    public int X => SubView.X;
    public int Y => SubView.Y;
    public int Z => SubView.Z;

    public FillStyle FillStyleX => SubView.FillStyleX;
    public FillStyle FillStyleY => SubView.FillStyleY;
    public int StaticX => SubView.StaticX + 2;
    public int StaticY => SubView.StaticY + 2;
    public float PercentX => SubView.PercentX;
    public float PercentY => SubView.PercentY;

    public int MinX => SubView.MinX + 2;
    public int MinY => SubView.MinY + 2;

    public int OriginX { set; get; }
    public int OriginY { set; get; }
    public int SizeX { set; get; }
    public int SizeY { set; get; }

    private bool Dirty { set; get; }

    required public IRelativeMoonfireView SubView;

    public bool Arrange()
    {
        if (Dirty)
            OnArrange();

        return SubView.Arrange();
    }

    protected abstract void OnArrange();

    public void Draw(TerminalContext context)
    {
        if(context.W < MinX || context.H < MinY)
            return;
        
        if (Dirty)
        {
            OnDraw(context);

            Dirty = false;
        }

        var shiftedContext = ResizeContext(context);
        SubView.Draw(shiftedContext);
    }

    protected abstract void OnDraw(TerminalContext context);

    protected virtual TerminalContext ResizeContext(TerminalContext context) => context;

    public void Invalidate()
    {
        Dirty = true;
        SubView.Invalidate();
    }
}
