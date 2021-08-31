using System;

namespace GhostInTheCellAI.Models
{
    public class MoveGameAction : GameAction
    {

        public int Cyborgs { get; set; }
        public Factory Destination { get; private set; }
        public int CyborgsAfterProductionTime => Destination.Owner == Owner.Enemy ? Destination.Cyborgs + (Destination.Production * Distance) : Destination.Cyborgs;

        public override string Name() => "MOVE";

        public MoveGameAction(Factory source, Factory destination, int distance)
        {
            Source = source;
            Destination = destination;
            Distance = distance;
            Cyborgs = CyborgsAfterProductionTime + 1;
            GiveScore();
        }

        private void GiveScore()
        {
            Score = ((Destination.Production * 100) / Distance) + Source.Cyborgs - Destination.Cyborgs;
        }

        //DOTO IsPossible is wrong needs refactor.
        public bool IsPossible()
        {
            if (Destination.Owner == Owner.Player)
            {
                return false;
            }

            if (Source.Cyborgs > CyborgsAfterProductionTime)
            {
                return true;
            }
            return false;
        }

        public void PlayOut()
        {
            Source.Cyborgs -= Cyborgs;
            if (Destination.Owner == Owner.Enemy || Destination.Owner == Owner.Neutral)
            {
                Destination.Cyborgs -= Cyborgs;
                if (Destination.Cyborgs < 0)
                {
                    Destination.Owner = Owner.Player;
                    Destination.Cyborgs = Math.Abs(Destination.Cyborgs);
                }
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
