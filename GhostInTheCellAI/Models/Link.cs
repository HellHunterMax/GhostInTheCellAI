using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostInTheCellAI.Models
{
    public class Link
    {
        public Factory Factory1 { get; set; }
        public Factory Factory2 { get; set; }
        public int Distance { get; set; }
    }
}
