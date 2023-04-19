using DSharpPlus;
using DSharpPlus.EventArgs;
using KansasBot.rsc.modules.whitelistmodule.services;

namespace KansasBot.rsc.modules.whitelistmodule
{
    public sealed class Allowlist
    {
        private AllowlistService Service;
        private InteractionCreateEventArgs Sender;

        public Allowlist(AllowlistService service, InteractionCreateEventArgs sender)
        {
            Service = service;
            Sender = sender;
        }
    }
}
