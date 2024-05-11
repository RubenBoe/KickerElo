

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class PlayerGame : AbstractEntityClass
    {
        public string GameID { get; set; }
        public string PlayerID { get; set; }
        public int ClientID { get; set; }
        public int Team { get; set; }
        public int Points { get; set; }
        public int EloGain { get; set; }
    }
}
