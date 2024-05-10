using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class Game : AbstractEntityClass
    {
        public string GameID { get; set; }
        public string SeasonID { get; set; }
        public int ClientID { get; set; }
        public DateTime Date { get; set; }
    }
}
