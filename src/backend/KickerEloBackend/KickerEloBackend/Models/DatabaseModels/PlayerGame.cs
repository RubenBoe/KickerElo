using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class PlayerGame : AbstractEntityClass
    {
        public int GameID { get; set; }
        public int PlayerID { get; set; }
        public int Team { get; set; }
        public int Points { get; set; }
        public int EloGain { get; set; }
    }
}
