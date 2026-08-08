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

    public int MinX { get;} = 0;
    public int MinY { get;} = 0;

    private volatile ImmutableList<IRelativeMoonfireView> _subViews = [];
    required public ImmutableList<IRelativeMoonfireView> SubViews
    {
        set => _subViews = value;
        private get => _subViews;
    }

    private LayoutToken? cachedLayout;
    private List<LayoutToken> layouts = [];    
    
    protected List<int>? XLevels { private set; get;}
    protected List<int>? YLevels { private set; get;}
    protected List<int>? ZLevels { private set; get;}

    private bool Dirty { set; get; }

    public async Task<bool> Arrange()
    {
        var subViews = SubViews;
        
        if (Dirty)
        {
            PopulateXYZLevels(subViews);
            await OnArrange(subViews);
            CheckLayout(subViews);
        }

        return (await Task.WhenAll(subViews.Select(v => v.Arrange()))).Any(r => r);
    }

    protected abstract Task OnArrange(ImmutableList<IRelativeMoonfireView> subViews);

    private void CheckLayout(ImmutableList<IRelativeMoonfireView> subViews)
    {
        LayoutToken newLayout = new(subViews);

        if(cachedLayout is not null && cachedLayout == newLayout)
            return;

        cachedLayout = newLayout;

        List<LayoutToken> newLayouts = [];
        foreach(var zLevel in ZLevels!)
        {
            var views = subViews.Where(v => v.Z == zLevel);

            int[] minX = new int[XLevels!.Count];
            int[] minY = new int[YLevels!.Count];
            foreach(var view in views)
            {
                minX[view.X] = Math.Max(view.MinX, minX[view.X]);
                minY[view.Y] = Math.Max(view.MinY, minY[view.Y]);
            }
            if(minX.Sum() > SizeX || minY.Sum() > SizeY)
                continue;

            LayoutToken layout = new(views);

            if(views.MaxBy(v => v.X)?.X >= views.MaxBy(v => v.Y)?.Y)
                layout.MapRegionsHorizontalSweep();
            else
                layout.MapRegionsVerticalSweep();
            
            newLayouts.Add(layout);
        }

        layouts = newLayouts;
    }    
    
    private void PopulateXYZLevels(ImmutableList<IRelativeMoonfireView> subViews)
    {
        HashSet<int> xSet = [];
        HashSet<int> ySet = [];
        HashSet<int> zSet = [];

        foreach (var view in subViews)
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
        if(Dirty)
        {
            foreach(ref var cell in context)
                cell = TerminalCell.Blank;
            
            Dirty = false;
        }
            
        foreach(var layout in layouts)
            await layout.Draw(context);
    }

    public async Task Invalidate()
    {
        Dirty = true;
        await Task.WhenAll(SubViews.Select(v => v.Invalidate()));
    }
}
