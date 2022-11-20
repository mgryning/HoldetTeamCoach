using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoldetTeamCoach
{
    [DebuggerDisplay("Name={Name}, Team={Team}, Position={Position}")]
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public string Team { get; set; } = string.Empty;
        public int TeamEnum { get; set; }
        public string Position { get; set; } = string.Empty;
        public double Price { get; set; }
        public int iPrice { get; set; }
        public string Odds { get; set; } = string.Empty;
        public float fOddsCountry { get; set; }
        public float fOddsPlayer { get; set; }
        public float fOddsAssist { get; set; }
        public float Score { get; set; }
        public int Id { get; set; }
    }
}
