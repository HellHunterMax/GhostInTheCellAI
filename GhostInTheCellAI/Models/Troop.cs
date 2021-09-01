/*
 * If entityType equals TROOP then the arguments are:
arg1: player that owns the troop: 1 for you or -1 for your opponent
arg2: identifier of the factory from where the troop leaves
arg3: identifier of the factory targeted by the troop
arg4: number of cyborgs in the troop (positive integer)
arg5: remaining number of turns before the troop arrives (positive integer)
*/
namespace GhostInTheCellAI.Models
{
    public class Troop
    {
        public int Id { get; set; }
        public Owner Owner { get; set; }
        public Factory Source { get; set; }
        public Factory Destination { get; set; }
        public int Size { get; set; }
        public int TurnsToArrive { get; set; }

        public override string ToString()
        {
            return $"Source = {Source.Id}, Destination = {Destination.Id}, TTA = {TurnsToArrive}, SIZE = {Size}, Destination Cyborgs ={Destination.Cyborgs}";
        }
    }
}
