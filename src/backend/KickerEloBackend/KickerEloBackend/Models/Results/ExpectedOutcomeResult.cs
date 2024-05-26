using KickerEloBackend.Models.CommandModels;
using System.Collections.Generic;

namespace KickerEloBackend.Models.Results
{
    internal class ExpectedOutcomeResult
    {
        public IEnumerable<Team> Teams { get; set; }
    }
}
