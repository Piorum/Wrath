using Moonfire.Input.Models;
using Moonfire.Logging.Models;
using Moonfire.Tui;
using Wrath.Views;

TestView testView = new();
Pane rootView = new() { SubViews = [testView]};
TuiApp app = new(rootView, new() { RendererBatchTimeout = TimeSpan.Zero });
await TuiApp.InitLogging([LogLevel.Debug, LogLevel.Info, LogLevel.Warn, LogLevel.Error, LogLevel.Fatal]);

app.InputHandler
    .Bind()
    .WithKey(InputKey.KeyboardBind(ConsoleKey.Q))
    .WithBind(new(async (_) => { await app.Stop(); }))
    .Register();

_ = Task.Run(async () =>
{
    while(true)
    {
        TaskCompletionSource tcs = new();
        
        await app.Renderer.EnqueueAction(async () =>
        {
            await testView.Invalidate();
            tcs.TrySetResult();
        });

        await tcs.Task;
        await Task.Yield();
    }
});

await app.Run();
