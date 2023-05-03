using Newtonsoft.Json;

namespace EdaurodoBot.rsc.modules.allowlistmodule.config
{
    public struct AllowlistQuestion
    {
        [JsonProperty("question")]
        public string Question { get; private set; }

        [JsonProperty("alternatives")]
        public IEnumerable<string> Alternatives { get; private set; }

        [JsonProperty("correct_response")]
        public int CorrectAnswer { get; private set; }

        public AllowlistQuestion(string? question, IEnumerable<string>? alternatives, int? correctanswer)
        {
            Question = question ?? "Escreva aqui sua pergunta!";
            Alternatives = alternatives ?? new List<string>() {
                "Aqui as alternativas para sua pergunta!", 
                "Cada pergunta pode conter até no maximo 25 alternativas!",
                "As alternativas começam a contar a partir de 1!",
                "Esta aqui é a 4ª alternativa por exemplo!",
                "A baixo você coloca qual é a alternativa correta"
            };
            CorrectAnswer = correctanswer ?? 1;
        }
    }
}
