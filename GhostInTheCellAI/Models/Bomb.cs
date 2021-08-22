using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostInTheCellAI.Models
{
    /*If entityType equals BOMB then the arguments are:
    arg1: player that send the bomb: 1 if it is you, -1 if it is your opponent
    arg2: identifier of the factory from where the bomb is launched
    arg3: identifier of the targeted factory if it's your bomb, -1 otherwise
    arg4: remaining number of turns before the bomb explodes (positive integer) if that's your bomb, -1 otherwise
    arg5: unused
    */
    public class Bomb
    {
        public Owner Owner { get; set; }
        public int Id { get; set; }
        public Factory Source { get; set; }
        public Factory Destination { get; set; }
        public int TurnsToArrive { get; set; }
    }
}
