using GhostInTheCellAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostInTheCellAI
{
    public class ActionService
    {
        public string CalculateBestAction(Game game)
        {
            string action = "WAIT";

            List<Factory> ownedFactories = game.Factories.FindAll(fac => fac.Owner == Owner.Player);
            Dictionary<Factory, Dictionary<Factory, int>> possibleTakeOvers = FindPossibleTakeOvers(ownedFactories);
            Factory bombTarget = null;
            if (game.RemainingBombs > 0)
            {
                bombTarget = FindBombTarget(game);
            }
            if (possibleTakeOvers.Count < 1)
            {
                if (bombTarget != null)
                {
                    action = $"BOMB {ownedFactories[0].Id} {bombTarget.Id}";
                }
                return action;
            }

            Factory[] sourceAndDestination = GetBestTakeOver(possibleTakeOvers);

            if (sourceAndDestination[0] == null)
            {

                sourceAndDestination = FindClosestFactoryToEnemy(ownedFactories);
                Console.Error.WriteLine($"sourceAndDestination Length = {sourceAndDestination.Length}, sourceAndDestination = {sourceAndDestination.FirstOrDefault()}");
            }


            int numberOfCyborgs = 0;
            if (sourceAndDestination[0] != null && sourceAndDestination[1] != null)
            {
                Factory source = sourceAndDestination[0];
                Factory destination = sourceAndDestination[1];
                if (sourceAndDestination[1].Owner == Owner.Enemy)
                {
                    foreach (var link in source.Links)
                    {
                        if (link.Contains(source, destination))
                        {
                            numberOfCyborgs = destination.Production * link.Distance + destination.Cyborgs + 1;
                        }
                    }
                }
                else
                {
                    numberOfCyborgs = destination.Cyborgs + 1;
                }
                action = $"MOVE {source.Id} {destination.Id} {destination.Cyborgs + 1}";
            }

            Factory bombSource = FindBombSource(sourceAndDestination[0], ownedFactories);
            if (bombTarget != null && bombSource != null)
            {
                action += $";BOMB {sourceAndDestination[0].Id} {bombTarget.Id}";
                game.RemainingBombs--;
            }
            return action;
        }

        private static Factory FindBombSource(Factory factory, List<Factory> ownedFactories)
        {
            foreach (var targetFactory in ownedFactories)
            {
                if (targetFactory != factory)
                {
                    return targetFactory;
                }
            }

            return null;
        }

        private static Factory[] FindClosestFactoryToEnemy(List<Factory> ownedFactories)
        {
            Factory source = null;
            Factory destination = null;
            int distance = int.MaxValue;

            foreach (var factory in ownedFactories)
            {
                for (int linkNumber = 0; linkNumber < factory.Links.Count; linkNumber++)
                {
                    Factory destinationFactory = GetDestinationFactory(factory, linkNumber);
                    if (destinationFactory.Owner == Owner.Enemy)
                    {
                        if (destination == null)
                        {
                            source = factory;
                            destination = destinationFactory;
                            distance = factory.Links[linkNumber].Distance;
                        }
                        if (factory.Links[linkNumber].Distance == distance && destinationFactory.Production > destination.Production)
                        {
                            source = factory;
                            destination = destinationFactory;
                            distance = factory.Links[linkNumber].Distance;
                        }
                        else if (factory.Links[linkNumber].Distance < distance && destinationFactory.Production >= destination.Production)
                        {
                            source = factory;
                            destination = destinationFactory;
                            distance = factory.Links[linkNumber].Distance;
                        }
                    }
                }
            }
            Console.Error.WriteLine($"source = {source}, destination = {destination}");
            return new Factory[] { source, destination };
        }

        private Factory FindBombTarget(Game game)
        {
            //TODO Check if bomb is already being sent.
            Factory target = null;
            foreach (var factory in game.Factories)
            {
                if (factory.Owner == Owner.Enemy)
                {
                    if (target == null && factory.Production > 0 && factory.Cyborgs > 0)
                    {
                        target = factory;
                    }
                    else if (target != null && factory.Production > 0 && factory.Cyborgs > target.Cyborgs)//TODO check if production is more the target.
                    {
                        target = factory;
                    }
                }
            }
            return target;
        }

        private static Factory[] GetBestTakeOver(Dictionary<Factory, Dictionary<Factory, int>> possibleTakeOvers)
        {
            Factory source = null;
            Factory destination = null;

            KeyValuePair<Factory, int> factoryDestinationAndDistance = new KeyValuePair<Factory, int>();
            foreach (var FactorySourceAndFactoryDest in possibleTakeOvers)
            {
                Factory newSource = FactorySourceAndFactoryDest.Key;
                foreach (var newFactoryDestinationAndDistance in FactorySourceAndFactoryDest.Value)
                {
                    //Console.Error.WriteLine($"Current = {factoryDestinationAndDistance}");
                    //Console.Error.WriteLine($"new = {newFactoryDestinationAndDistance.Key}");
                    if (factoryDestinationAndDistance.Key == null)
                    {
                        if (newFactoryDestinationAndDistance.Key.Owner != Owner.Player)
                        {
                            source = FactorySourceAndFactoryDest.Key;
                            factoryDestinationAndDistance = newFactoryDestinationAndDistance;
                            destination = factoryDestinationAndDistance.Key;
                        }
                    }
                    else
                    {
                        if (!IsCurrentTargetBest(source.Cyborgs, newSource.Cyborgs, factoryDestinationAndDistance, newFactoryDestinationAndDistance))
                        {
                            if (newFactoryDestinationAndDistance.Key.Owner != Owner.Player)
                            {
                                source = newSource;
                                factoryDestinationAndDistance = newFactoryDestinationAndDistance;
                                destination = newFactoryDestinationAndDistance.Key;
                            }
                        }
                    }
                }
            }
            Factory[] factories = { source, destination };
            return factories;
        }


        //TODO Add minuspoints for own owned factories. Removed for now.
        private static bool IsCurrentTargetBest(int cyborgsCurrent, int cyborgsChallenger, KeyValuePair<Factory, int> CurrentFactoryAndDistance, KeyValuePair<Factory, int> ChallengerFactoryAndDistance)
        {
            bool currentIsClosest = CurrentFactoryAndDistance.Value <= ChallengerFactoryAndDistance.Value;

            int cyborgsClose;
            int cyborgsFurthest;
            KeyValuePair<Factory, int> closestFactoryAndDistance;
            KeyValuePair<Factory, int> FurthestFactoryAndDistance;

            switch (currentIsClosest)
            {
                case true:
                    cyborgsClose = cyborgsCurrent;
                    cyborgsFurthest = cyborgsChallenger;
                    closestFactoryAndDistance = CurrentFactoryAndDistance;
                    FurthestFactoryAndDistance = ChallengerFactoryAndDistance;
                    break;
                case false:
                    cyborgsClose = cyborgsChallenger;
                    cyborgsFurthest = cyborgsCurrent;
                    closestFactoryAndDistance = ChallengerFactoryAndDistance;
                    FurthestFactoryAndDistance = CurrentFactoryAndDistance;
                    break;
            }


            //closest	(cyborgs - dest + (abs(Sourcedistance - Destinationdistance)* production))* prod
            //farthest  cyborgs - dest * prod
            int score1 = (cyborgsClose - closestFactoryAndDistance.Key.Cyborgs + (Math.Abs(closestFactoryAndDistance.Value - FurthestFactoryAndDistance.Value)) * closestFactoryAndDistance.Key.Production) * closestFactoryAndDistance.Key.Production;
            int score2 = (cyborgsFurthest - FurthestFactoryAndDistance.Key.Cyborgs) * FurthestFactoryAndDistance.Key.Production;

            //Console.Error.Write($"currentIsClosest = {currentIsClosest}, score1 = {score1}, score2 = {score2}, ");
            // Score 1 = (new) score 2 = (old)
            //currentIsClosest = False, score1 = 30, score2 = 0, True
            bool isScore1More = score1 >= score2;
            bool answer = currentIsClosest ? isScore1More : !isScore1More;

            //Console.Error.WriteLine(answer);
            return answer;
        }

        //TODO add troops to possibletakeovers
        private static Dictionary<Factory, Dictionary<Factory, int>> FindPossibleTakeOvers(List<Factory> ownedFactories)
        {
            Dictionary<Factory, Dictionary<Factory, int>> possibleTakeOvers = new Dictionary<Factory, Dictionary<Factory, int>>();
            for (int i = 0; i < ownedFactories.Count; i++)
            {
                Factory factory = ownedFactories[i];
                int numberOfLinks = factory.Links.Count;
                //Console.Error.WriteLine($"Owned factory = {factory}");

                Dictionary<Factory, int> possibleDestinations = new Dictionary<Factory, int>();
                for (int links = 0; links < numberOfLinks; links++)
                {
                    Factory destinationFactory = GetDestinationFactory(factory, links);
                    int destinationCyborgs;
                    if (destinationFactory.Owner == Owner.Enemy)
                    {
                        destinationCyborgs = destinationFactory.Cyborgs + (destinationFactory.Production * factory.Links[links].Distance);
                    }
                    else
                    {
                        destinationCyborgs = destinationFactory.Cyborgs;
                    }
                    if (factory.Cyborgs > destinationCyborgs && (destinationFactory.Production != 0 || destinationFactory.Owner != Owner.Enemy))
                    {
                        possibleDestinations.Add(destinationFactory, factory.Links[links].Distance);
                        //Console.Error.WriteLine($"Possible Destination = {destinationFactory}");
                    }
                }
                if (possibleDestinations.Count > 0)
                {
                    possibleTakeOvers.Add(factory, possibleDestinations);
                }
            }
            return possibleTakeOvers;
        }

        private static Factory GetDestinationFactory(Factory factory, int linknumber)
        {

            return factory.Links[linknumber].Factory1.Id == factory.Id ? factory.Links[linknumber].Factory2 : factory.Links[linknumber].Factory1;
        }
    }
}
