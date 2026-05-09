using System.Collections.Immutable;
using Moonfire.Rendering.Enums;
using Moonfire.Rendering.Interfaces;
using Wrath.Views.Abstract;

namespace Wrath.Views;

public class Pane : LayoutViewDelegated
{
    protected override async Task OnArrange(ImmutableList<IRelativeMoonfireView> subViews)
    {
        var orderedByWidthStyle = subViews.OrderBy(sv => sv.FillStyleX).ToList();
        var orderedByHeightStyle = subViews.OrderBy(sv => sv.FillStyleY).ToList();
        ILookup<(int x, int y, int z), IRelativeMoonfireView> subViewGrid = subViews.ToLookup(sv => (sv.X, sv.Y, sv.Z));

        List<Task> measureAndPostionTasks = [];
        foreach (var zLevel in ZLevels!)
        {
            var zIndex = zLevel;
            measureAndPostionTasks.Add(Task.Run(async () =>
            {
                await Measure(zIndex, orderedByWidthStyle, orderedByHeightStyle);

                await Position(zIndex, subViewGrid);
            }));
        }

        await Task.WhenAll(measureAndPostionTasks);
    }

    private async Task Measure(int zIndex, List<IRelativeMoonfireView> orderedByWidthStyle, List<IRelativeMoonfireView> orderedByHeightStyle)
    {
        //Width
        var widthTask = Task.Run(() =>
        {
            int[] availableWidth = new int[YLevels!.Count];
            Array.Fill(availableWidth, SizeX);
            foreach (var view in orderedByWidthStyle)
            {
                if (view.Z != zIndex) continue;

                switch (view.FillStyleX)
                {
                    case FillStyle.Static:
                        view.SizeX = view.StaticX;
                        break;
                    case FillStyle.Min:
                        var minX = view.MinX;
                        view.SizeX = availableWidth[view.Y] > minX - 1 ? minX : 0;
                        break;
                    case FillStyle.Percent:
                        view.SizeX = Math.Max(view.MinY, (int)(availableWidth[view.Y] * view.PercentX));
                        break;
                    case FillStyle.Max:
                        view.SizeX = availableWidth[view.Y];
                        break;
                }

                view.SizeX = Math.Max(0, view.SizeX);

                if (view.FillStyleX != FillStyle.Max)
                    availableWidth[view.Y] -= view.SizeX;
            }
        });

        //Height
        var heightTask = Task.Run(() =>
        {
            int[] availableHeight = new int[XLevels!.Count];
            Array.Fill(availableHeight, SizeY);

            int[] largestAtY = new int[YLevels!.Count];

            foreach (var view in orderedByHeightStyle)
            {
                if (view.Z != zIndex) continue;

                switch (view.FillStyleY)
                {
                    case FillStyle.Static:
                        view.SizeY = view.StaticY;
                        break;
                    case FillStyle.Min:
                        var minY = view.MinY;
                        view.SizeY = availableHeight[view.X] > minY - 1 ? minY : 0;
                        break;
                    case FillStyle.Percent:
                        view.SizeY = Math.Max(view.MinX, (int)(availableHeight[view.X] * view.PercentY));
                        break;
                    case FillStyle.Max:
                        view.SizeY = availableHeight[view.X];
                        break;
                }

                view.SizeY = Math.Max(0, view.SizeY);

                if (view.SizeY > largestAtY[view.Y])
                {
                    if (view.FillStyleY != FillStyle.Max)
                        for (int i = 0; i < availableHeight.Length; i++)
                            availableHeight[i] -= view.SizeY - largestAtY[view.Y];
                    largestAtY[view.Y] = view.SizeY;
                }
            }
        });

        await Task.WhenAll(widthTask, heightTask);
    }

    private Task Position(int zIndex, ILookup<(int x, int y, int z), IRelativeMoonfireView> subViewGrid)
    {
        var CursorX = OriginX;
        var CursorY = OriginY;
        foreach (var yLevel in YLevels!)
        {
            var largestY = 0;
            foreach (var xLevel in XLevels!)
            {
                IRelativeMoonfireView? subView = subViewGrid[(xLevel, yLevel, zIndex)].FirstOrDefault();
                if (subView is null || subView.Z != zIndex) continue;

                subView.OriginX = CursorX;
                subView.OriginY = CursorY;

                CursorX += subView.SizeX;

                largestY = subView.SizeY > largestY ? subView.SizeY : largestY;
            }
            CursorX = OriginX;
            CursorY += largestY;
        }

        return Task.CompletedTask;
    }
}
