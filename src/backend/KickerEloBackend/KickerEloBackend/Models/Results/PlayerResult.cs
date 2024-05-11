using System;

namespace KickerEloBackend.Models.Results
{
    /// <summary>
    /// Contains shallow information about a player in a given season. For details about a player, there is another class.
    /// </summary>
    internal class PlayerResult
    {
        public string PlayerID { get; set; }
        public string Nickname { get; set; }
        public int EloNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
