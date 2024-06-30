using System;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class Season
    {
        public string SeasonID { get; set; }
        public int ClientID { get; set; }
        public int SeasonNumber { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; } = null;
    }
}
