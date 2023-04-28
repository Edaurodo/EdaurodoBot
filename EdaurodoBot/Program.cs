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
        $"Message: {ex.Message}" +
        $"\nSource: {ex.Source}" +
        $"\nTarget Site: {ex.TargetSite}" +
        $"\nStack Trace: {ex.StackTrace}\n" +
        $"Pressione qualquer botão para sair...");
    Console.ReadKey();
}
