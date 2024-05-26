using System.Collections.Generic;

namespace KickerEloBackend.Models.CommandModels
{
    internal class EnterGameCommand
    {
        public string ClientToken { get; set; }
        public IEnumerable<Team> Teams { get; set; }
    }

    internal class Team: TeamBase
    {
        public int Points { get; set; }
    }

    internal class TeamBase
    {
        public IEnumerable<string> PlayerIDs { get; set; }
        /// <summary>
        /// Should be 1 or 2
        /// </summary>
        public int TeamNumber { get; set; }
    }
}
