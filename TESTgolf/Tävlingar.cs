using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TESTgolf
{
    class Tävlingar 
    { 
        public int tavlingId { get; set; }
        public string tavlingsNamn { get; set; }
        public DateTime startDatum { get; set; }
        public DateTime slutDatum { get; set; }
        public DateTime sistaAnmalningsDatum { get; set; }
        public DateTime sistaAvbokningsDatum { get; set; }        
        public double klassA { get; set; }
        public double klassB { get; set; }
        public double klassC { get; set; }
        

        public override string ToString()
        {
            return tavlingsNamn;
        }        
    }
}
