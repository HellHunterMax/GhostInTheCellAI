using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostInTheCellAI.Models
{
    public class MoveGameAction : GameAction
    {

        public int Cyborgs { get; set; }
        public Factory Destination { get; private set; }

        public override string Name() => "MOVE";

        public MoveGameAction(Factory source, Factory destination, int distance)
        {
            Source = source;
            Destination = destination;
            Distance = distance;
            Cyborgs = Math.Abs(Destination.Cyborgs) + 1;
            GiveScore();
        }

        private void GiveScore()
        {
            Score = ((Destination.Production * 100) / Distance ) + Source.Cyborgs - Destination.Cyborgs;
        }

        public bool IsPossible()
        {
            if (Destination.Owner == Owner.Enemy || Destination.Owner == Owner.Neutral)
            {
                if (Destination.Cyborgs >= 0 && Source.Cyborgs > Destination.Cyborgs)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (Destination.Cyborgs <= 0 && Source.Cyborgs > Math.Abs(Destination.Cyborgs))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void PlayOut()
        {
            Source.Cyborgs -= Cyborgs;
            if (Destination.Owner == Owner.Enemy || Destination.Owner == Owner.Neutral)
            {
                Destination.Cyborgs -= Cyborgs;
            }
            else
            {
                Destination.Cyborgs += Cyborgs;
            }
        }

        public override string ToString()
        {
            return $"{Name()} {Source.Id} {Destination.Id} {Cyborgs}";
        }
        public override string WriteAction()
        {
            return $"{Name()} Source={Source.Id} Destination={Destination.Id} Cyborgs={Cyborgs} Score={Score} Distance={Distance}";
        }
    }
}
