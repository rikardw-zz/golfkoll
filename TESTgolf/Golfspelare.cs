using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TESTgolf
{
    class Golfspelare
    {        
        public int GolfId { get; set; }  
        public string Fornamn { get; set; }
        public string Efternamn { get; set; }
        public string Mobil { get; set; }
        public string Adress { get; set; }
        public string GatuNr { get; set; }
        public int PostNr { get; set; }
        public string Email { get; set; }
        public bool Medlemsavg { get; set; }
        public string Handicap { get; set; } 
        public int Status { get; set; }
         

        public override string ToString()
        {
            return GolfId + " " + Fornamn + " " + Efternamn + " " + Status;
        }
    }
}
