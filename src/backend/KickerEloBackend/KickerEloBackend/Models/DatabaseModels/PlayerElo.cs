﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KickerEloBackend.Models.DatabaseModels
{
    internal class PlayerElo
    {
        public PlayerElo() { }
        public PlayerElo(string PlayerID, string SeasonID, int EloNumber) 
        {
            this.PlayerID = PlayerID;
            this.SeasonID = SeasonID;
            this.EloNumber = EloNumber;
        }
        public string PlayerID { get; set; }
        public string SeasonID { get; set; }
        public int EloNumber { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
