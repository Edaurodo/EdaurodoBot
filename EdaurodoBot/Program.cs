using EdaurodoBot.rsc.core;
using EdaurodoBot.rsc.core.data;

try
{
    EdaurodoMain bot = new EdaurodoMain(EdaurodoConfigLoader.LoadConfigAsync().GetAwaiter().GetResult());
    await bot.StartAsync();
    await Task.Delay(-1);
}
catch (Exception ex)
{
    Console.WriteLine(
        $"\n\nMessage: {ex.Message}" +
        $"\n\nSource: {ex.Source}" +
        $"\n\nInner Exception: {ex.InnerException}" +
        $"\n\nTarget Site: {ex.TargetSite}" +
        $"\n\nStack Trace: {ex.StackTrace}\n\n");
}