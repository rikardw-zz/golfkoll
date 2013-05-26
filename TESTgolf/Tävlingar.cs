using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TESTgolf
{
    class Tävlingar 
    { 
        public int TävlingsID { get; set; }
        public string TävlingsNamn { get; set; }
        public DateTime StartDatum { get; set; }
        public DateTime StoppDatum { get; set; }
        public DateTime AnmälningsDatum { get; set; }
        public DateTime AvbokningsDatum { get; set; }
        public double KlassA { get; set; }
        public double KlassB { get; set; }
        public double KlassC { get; set; }
        

        public override string ToString()
        {
            return TävlingsNamn;
        }        
    }
}
