using System.Collections.Immutable;
using Moonfire.Rendering;
using Moonfire.Rendering.Enums;
using Moonfire.Rendering.Interfaces;
using Moonfire.Rendering.Models;

namespace Wrath.Views.Abstract;

public abstract class LayoutViewDelegated : IRelativeMoonfireView
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

    public int MinX => SubViews.Max(sv => sv.MinX);
    public int MinY => SubViews.Max(sv => sv.MinY);
    
    public ImmutableList<IRelativeMoonfireView> SubViews = [];
    private LayoutToken? cachedLayout;
    private List<LayoutToken> layouts = [];    
    
    protected List<int>? XLevels { private set; get;}
    protected List<int>? YLevels { private set; get;}
    protected List<int>? ZLevels { private set; get;}

    private bool Dirty { set; get; }

    public async Task<bool> Arrange()
    {
        if (Dirty)
        {
            PopulateXYZLevels();
            await OnArrange();
            CheckLayout();

            Dirty = false;
        }

        return (await Task.WhenAll(SubViews.Select(v => v.Arrange()))).Any(r => r);
    }

    protected abstract Task OnArrange();

    private void CheckLayout()
    {
        LayoutToken newLayout = new(SubViews);

        if(cachedLayout is not null && cachedLayout == newLayout)
            return;

        cachedLayout = newLayout;

        List<LayoutToken> newLayouts = [];
        foreach(var zLevel in ZLevels!)
        {
            var views = SubViews.Where(v => v.Z == zLevel);
            LayoutToken layout = new(views);

            if(views.MaxBy(v => v.X)?.X >= views.MaxBy(v => v.Y)?.Y)
                layout.MapRegionsHorizontalSweep();
            else
                layout.MapRegionsVerticalSweep();
            
            newLayouts.Add(layout);
        }

        layouts = newLayouts;
    }    
    
    private void PopulateXYZLevels()
    {
        HashSet<int> xSet = [];
        HashSet<int> ySet = [];
        HashSet<int> zSet = [];

        foreach (var view in SubViews)
        {
            xSet.Add(view.X);
            ySet.Add(view.Y);
            zSet.Add(view.Z);
        }

        XLevels = [.. xSet.Order()];
        YLevels = [.. ySet.Order()];
        ZLevels = [.. zSet.Order()];
    }

    public async Task Draw(TerminalContext context)
    {
        foreach(ref var cell in context)
            cell = TerminalCell.Blank;
            
        foreach(var layout in layouts)
            await layout.Draw(context);
    }

    public async Task Invalidate()
    {
        Dirty = true;
        await Task.WhenAll(SubViews.Select(v => v.Invalidate()));
    }
}
