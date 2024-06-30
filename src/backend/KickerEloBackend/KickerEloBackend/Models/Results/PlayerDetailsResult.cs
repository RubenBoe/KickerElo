using System;
using System.Collections.Generic;

namespace KickerEloBackend.Models.Results
{
    internal class PlayerDetailsResultBase
    {
        public string PlayerID { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
        public int EloNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }
    internal class PlayerDetailsResult: PlayerDetailsResultBase
    {
        public PlayerDetailsResult() { }
        public PlayerDetailsResult(PlayerDetailsResultBase resultBase, IEnumerable<GameResult> gamesPlayed)
        {
            this.PlayerID = resultBase.PlayerID;
            this.Nickname = resultBase.Nickname;
            this.FullName = resultBase.FullName;
            this.EloNumber = resultBase.EloNumber;
            this.LastUpdated = resultBase.LastUpdated;

            GamesPlayed = gamesPlayed;
        }

        public IEnumerable<GameResult> GamesPlayed { get; set; }
    }
}
