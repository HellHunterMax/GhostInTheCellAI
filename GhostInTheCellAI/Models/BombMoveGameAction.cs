namespace GhostInTheCellAI.Models
{
    public class BombMoveGameAction : GameAction
    {
        public override string Name() => "BOMB";
        public Factory Destination { get; private set; }

        public BombMoveGameAction(Factory source, Factory destination, int distance)
        {
            Source = source;
            Destination = destination;
            Distance = distance;
            GiveScore();
        }
        public void GiveScore()
        {
            Score = ((Destination.Production * 100) / Distance) + Destination.Cyborgs;
        }

        public override string ToString()
        {
            return $"{Name()} {Source} {Destination}";
        }
        public override string WriteAction()
        {
            return $"{Name()} Source={Source.Id} Destination={Destination.Id} Score={Score} Distance={Distance}";
        }
    }
}
