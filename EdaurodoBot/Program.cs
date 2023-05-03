using EdaurodoBot.rsc.core;
using EdaurodoBot.rsc.core.config;

try
{
    EdaurodoMain bot = new EdaurodoMain(EdaurodoConfigLoader.LoadConfigAsync().GetAwaiter().GetResult());
    await bot.StartAsync();
    await Task.Delay(-1);
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Mesage: ");
    Console.ForegroundColor = ConsoleColor.Red;

    Console.WriteLine(ex.Message);

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Source: ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.Source);

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Target Site: ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.TargetSite);

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Stack Trace: ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex.StackTrace + "\n");

    Console.ForegroundColor = ConsoleColor.Blue;
    Console.WriteLine("Pressione qualquer botão para sair...");
    Console.ResetColor();
    Console.ReadKey();
}