namespace GhostInTheCellAI.Models
{
    public class BombGameAction : GameAction
    {
        public override string Name() => "BOMB";
        public Factory Destination { get; private set; }
        public int Distance { get; private set; }

        public BombGameAction(Factory source, Factory destination, int distance)
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
        public bool IsValid(Game game)
        {
            int destinationId = Destination.Id;
            Owner destinationOwner = game.Factories.Find(x => x.Id == destinationId).Owner;
            int sourceId = Source.Id;
            Owner sourceOwner = game.Factories.Find(x => x.Id == sourceId).Owner;

            if (sourceOwner != Owner.Player || destinationOwner != Owner.Enemy)
            {
                return false;
                //throw new ImpossibleActionException($"{nameof(sourceOwner)} should be {Owner.Player} but is {sourceOwner}");
                //throw new ImpossibleActionException($"{nameof(destinationOwner)} should be {Owner.Enemy} but is {sourceOwner}");
            }
            return true;
        }

        public override string ToString()
        {
            return $"{Name()} {Source.Id} {Destination.Id}";
        }
        public override string WriteAction()
        {
            return $"{Name()} Source={Source.Id} Destination={Destination.Id} Score={Score} Distance={Distance}";
        }
    }
}
