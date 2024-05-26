using System.Collections.Generic;

namespace KickerEloBackend.Models.CommandModels
{
    internal class CalculateExpectedOutcomeCommand
    {
        public string ClientToken { get; set; }
        public IEnumerable<TeamBase> Teams { get; set; }
    }
}
