using System;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class Game
    {
        public string GameID { get; set; }
        public string SeasonID { get; set; }
        public int ClientID { get; set; }
        public DateTime Date { get; set; }
    }
}
