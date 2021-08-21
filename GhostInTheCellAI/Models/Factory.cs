using System.Collections.Generic;

/*
 * arg1: player that owns the factory: 1 for you, -1 for your opponent and 0 if neutral
arg2: number of cyborgs in the factory
arg3: factory production (between 0 and 3)
arg4: unused
arg5: unused
*/

namespace GhostInTheCellAI.Models
{
    public enum Owner
    {
        Enemy = -1,
        Neutral = 0,
        Player = 1
    }

    public class Factory
    {
        public int Id { get; set; }
        public List<Link> Links { get; set; } = new();
        public int Production { get; set; }
        public int Cyborgs { get; set; }
        public Owner Owner { get; set; }

        public override string ToString()
        {
            return $"FACTORY Id = {Id}, Owner = {Owner}, Links = {Links.Count}, Production = {Production}, Cyborgs = {Cyborgs}";
        }
    }
}
