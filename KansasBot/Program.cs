using KansasBot.rsc.core;
using KansasBot.rsc.core.data;

var x = DateTime.Now.ToUniversalTime().Subtract(DateTime.Now.ToUniversalTime().AddMinutes(2));

Console.WriteLine(x.ToString("hh\\:mm\\:ss"));

//Console.WriteLine(0 * (-1));


try
{
    var configLoader = new KansasConfigLoader();
    var config = await configLoader.LoadConfigAsync();
    KansasMain bot = new KansasMain(config);
    await bot.StartAsync();
    await Task.Delay(-1);
}
catch (Exception ex)
{
    Console.WriteLine(
        $"Message: {ex.Message}\n\n" +
        $"Source: {ex.Source}\n\n" +
        $"Inner Exception: {ex.InnerException}\n\n" +
        $"Target Site: {ex.TargetSite}\n\n" +
        $"Stack Trace: {ex.StackTrace}");
    Console.WriteLine(ex.ToString());
}