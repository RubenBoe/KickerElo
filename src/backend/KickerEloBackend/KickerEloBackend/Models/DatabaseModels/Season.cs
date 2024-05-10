using System;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class Season : AbstractEntityClass
    {
        public int SeasonId { get; set; }
        public int ClientID { get; set; }
        public int SeasonNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
