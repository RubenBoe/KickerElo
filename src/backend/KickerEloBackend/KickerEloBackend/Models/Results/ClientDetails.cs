using System;
using System.Collections.Generic;

namespace KickerEloBackend.Models.Results
{
    internal class ClientDetails
    {
        public string ClientName { get; set; }
        public DateTime CreationDate { get; set; }
        public int NumberOfPlayers { get; set; }
        public IEnumerable<SeasonResult> Seasons { get; set; }
        public PlayerResult CurrentLeader { get; set; }
    }
}
