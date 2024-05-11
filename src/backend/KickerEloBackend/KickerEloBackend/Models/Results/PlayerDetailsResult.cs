using System;
using System.Collections.Generic;

namespace KickerEloBackend.Models.Results
{
    internal class PlayerDetailsResult
    {
        public string PlayerID { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public int EloNumber { get; set; }
        public DateTime LastUpdated { get; set; }
        public IEnumerable<GameResult> GamesPlayed { get; set; }
    }
}
