using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class PlayerElo : AbstractEntityClass
    {
        public int PlayerID { get; set; }
        public int SeasonID { get; set; }
        public int EloNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
