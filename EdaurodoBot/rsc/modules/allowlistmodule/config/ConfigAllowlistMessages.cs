using EdaurodoBot.rsc.modules.allowlistmodule.config.messages;
using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.allowlistmodule.config
{
    public sealed class ConfigAllowlistMessages
    {
        [JsonProperty("main_fixed_message")]
        public MainMessage MainMessage { get; private set; }

        public ConfigAllowlistMessages(MainMessage? mainmessage)
        {
            MainMessage = mainmessage ?? new MainMessage(null, null, null);
        }
    }
}
