using System;
using System.Collections.Generic;

namespace KickerEloBackend.Models.Results
{
    internal class GameResults
    {
        public string GameID {  get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<TeamResult> TeamResults { get; set; }
    }

    internal class TeamResult
    {
        public IEnumerable<PlayerResult> PlayerResults { get; set; }
        public int TeamNumber { get; set; }
        public int Points { get; set; }
    }

    internal class PlayerResult
    {
        public string PlayerID { get; set; }
        public int EloGain { get; set; }
        /// <summary>
        /// The new Elo number after the game
        /// </summary>
        public int EloNumber { get; set; }
    }
}
