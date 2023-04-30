
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

/*
using EdaurodoBot.rsc.utils;
using Newtonsoft.Json;

var embed = new EdaurodoEmbed(null, "### > * Lembre-se que seu Embed não pode ser vazio!\nPersonalize abaixo!", null, null, new EdaurodoEmbedAuthor(null, null, null), new EdaurodoEmbedTitle(null, null), new EdaurodoEmbedFooter(null, null, null), new List<EdaurodoEmbedField>());

string json = JsonConvert.SerializeObject(embed, Formatting.Indented);

Console.WriteLine(json);
*/