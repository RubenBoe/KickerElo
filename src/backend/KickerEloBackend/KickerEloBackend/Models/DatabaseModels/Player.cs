using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class Player : AbstractEntityClass
    {
        public int PlayerID { get; set; }
        public int ClientID { get; set; }
        public string Nickname { get; set; }
        public string FullName { get; set; }
    }
}
