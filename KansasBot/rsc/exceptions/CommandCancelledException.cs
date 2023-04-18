namespace KansasBot.rsc.exceptions
{
    public sealed class CommandCancelledException : Exception
    {
        public CommandCancelledException() : base("A execução do comando foi cancelado devido a criterios não atendidos") { }
        public CommandCancelledException(string message) : base(message) { }
        public CommandCancelledException(Exception innerException) : base("A execução do comando foi cancelado devido a criterios não atendidos", innerException) { }
        public CommandCancelledException(string messsage, Exception innerException) : base(messsage, innerException) { }
    }
}
