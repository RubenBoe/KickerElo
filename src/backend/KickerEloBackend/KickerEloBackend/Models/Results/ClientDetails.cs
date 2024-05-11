using System;

namespace KickerEloBackend.Models.Results
{
    internal class ClientDetails
    {
        public string ClientName { get; set; }
        public DateTime CreationDate { get; set; }
        public int NumberOfPlayers { get; set; }
        public SeasonResult CurrentSeason { get; set; }
        public PlayerResult CurrentLeader { get; set; }
    }
}
