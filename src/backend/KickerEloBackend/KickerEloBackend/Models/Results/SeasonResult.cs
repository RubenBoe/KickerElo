using KickerEloBackend.Models.DatabaseModels;
using System;

namespace KickerEloBackend.Models.Results
{
    internal class SeasonResult
    {
        public SeasonResult(Season season) 
        {
            SeasonId = season.SeasonID;
            SeasonNumber = season.SeasonNumber;
            StartDate = season.StartDate;
            EndDate = season.EndDate;
        }
        public string SeasonId { get; set; }
        public int SeasonNumber { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; } = null;
    }
}
