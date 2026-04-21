using Moonfire.Input.Models;
using Moonfire.Logging.Models;
using Moonfire.Rendering.Enums;
using Moonfire.Tui;
using Wrath.Views;

await TuiApp.InitLogging([LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal]);

Pane rootView = new() { 
    SubViews = [
        new Border() { SubView = new TestView() { X = 0, Y = 0, FillStyleY = FillStyle.Static, StaticY = 1 } }, 
        new Border() { SubView = new TestView() { X = 0, Y = 1, FillStyleX = FillStyle.Percent, PercentX = 0.15f } }, 
        new Border() { SubView = new TestView() { X = 1, Y = 1, FillStyleX = FillStyle.Percent, PercentX = 0.25f } }, 
        new Border() { SubView = new TestView() { X = 2, Y = 1 } }, 
        new Border() { SubView = new TestView() { X = 0, Y = 2, FillStyleY = FillStyle.Static, StaticY = 1 } }
    ]
};

TuiApp app = new(rootView);

app.InputHandler
    .Bind()
    .WithKey(InputKey.KeyboardBind(ConsoleKey.Q))
    .WithBind(new(async (_) => await app.Stop()))
    .Register();
app.InputHandler
    .Bind()
    .WithKey(InputKey.KeyboardBind(ConsoleKey.S))
    .WithBind(new(async (_) => await app.Renderer.EnqueueAction(rootView.Invalidate)))
    .Register();

await app.Run();
