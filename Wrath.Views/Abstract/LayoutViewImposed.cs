using System.Collections.Immutable;
using Moonfire.Rendering;
using Moonfire.Rendering.Enums;
using Moonfire.Rendering.Interfaces;
using Moonfire.Rendering.Models;

namespace Wrath.Views.Abstract;

public abstract class LayoutViewImposed : IRelativeMoonfireView
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

    public int MinX { set; get; }
    public int MinY { set; get; }

    private volatile ImmutableList<IMoonfireView> _subViews = [];
    required public ImmutableList<IMoonfireView> SubViews
    {
        set => _subViews = value;
        private get => _subViews;
    }

    private LayoutToken? cachedLayout;
    private List<LayoutToken> layouts = [];    

    public bool Arrange()
    {
        throw new NotImplementedException();
    }

    public void Draw(TerminalContext context)
    {
        throw new NotImplementedException();
    }

    public void Invalidate()
    {
        throw new NotImplementedException();
    }
}
