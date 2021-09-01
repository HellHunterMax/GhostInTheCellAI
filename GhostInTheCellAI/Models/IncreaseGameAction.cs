using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostInTheCellAI.Models
{
    public class IncreaseGameAction : GameAction
    {
        public override string Name() => "INC";
        public IncreaseGameAction(Factory source)
        {
            Source = source;
            GiveScore();
        }
        public void GiveScore()
        {
            int closestDistance = int.MaxValue;
            foreach (var link in Source.Links)
            {
                if (link.Other(Source).Owner == Owner.Enemy )
                {
                    if (link.Distance < closestDistance)
                    {
                        closestDistance = link.Distance;
                    }
                }
            }

            var prod = Source.Production switch
            {
                0 => 3,
                1 => 2,
                2 => 1,
                _ => 0,
            };
            Score = closestDistance * prod;
        }

        public override string ToString()
        {
            return $"{Name()} {Source.Id}";
        }
        public override string WriteAction()
        {
            return $"{Name()} Source={Source.Id} Score={Score}";
        }
    }
}
