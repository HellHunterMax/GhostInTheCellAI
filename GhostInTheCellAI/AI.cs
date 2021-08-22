using GhostInTheCellAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostInTheCellAI
{
    public class AI
    {
        private readonly List<Factory> Factories = new List<Factory>();
        private string[] _Inputs;
        //TODO add remaining turns into calculation.
        //TODO Add minuspoints for own owned factories.
        //TODO push Cyborgs to closest to enemy.
        //TODO Dont send more units then units in factory.

        public AI()
        {
            BuildFactories();
        }

        public void Run()
        {
            // game loop
            while (true)
            {
                List<Troop> troops = new List<Troop>();
                int entityCount = int.Parse(Console.ReadLine()); // the number of entities (e.g. factories and troops)
                for (int i = 0; i < entityCount; i++)
                {
                    _Inputs = Console.ReadLine().Split(' ');
                    int entityId = int.Parse(_Inputs[0]);
                    string entityType = _Inputs[1];
                    if (entityType == "FACTORY")
                    {
                        Factory factory = Factories.First((fac => fac.Id == entityId));
                        UpdateFactory(factory);
                    }
                    else if (entityType == "TROOP")
                    {
                        troops.Add(
                            new Troop()
                            {
                                Id = entityId,
                                Owner = (Owner)int.Parse(_Inputs[2]),
                                Source = Factories.First((fac => fac.Id == int.Parse(_Inputs[3]))),
                                Destination = Factories.First((fac => fac.Id == int.Parse(_Inputs[4]))),
                                Size = int.Parse(_Inputs[5]),
                                TurnsToArrive = int.Parse(_Inputs[6])
                            });
                    }
                }

                string action = CalculateBestAction(troops);
                // To debug: Console.Error.WriteLine("Debug messages...");


                // Write an action using Console.WriteLine() Any valid action, such as "WAIT" or "MOVE source destination cyborgs"
                Console.WriteLine(action);
            }
        }

        private string CalculateBestAction(List<Troop> troops)
        {
            //TODO add troops to possibletakeovers
            //TODO Add remove 0 production factories from possibletakeovers except opponants.
            string action = "WAIT";

            List<Factory> ownedFactories = Factories.FindAll(fac => fac.Owner == Owner.Player);
            Dictionary<Factory, Dictionary<Factory, int>> possibleTakeOvers = FindPossibleTakeOvers(ownedFactories);
            if (possibleTakeOvers.Count < 1)
            {
                return action;
            }

            Factory[] sourceAndDestination = FindSourceAndDestination(possibleTakeOvers);


            //MOVE source destination cyborgCount
            action = $"MOVE {sourceAndDestination[0].Id} {sourceAndDestination[1].Id} {sourceAndDestination[1].Cyborgs + 1}";
            return action;
        }

        private static Factory[] FindSourceAndDestination(Dictionary<Factory, Dictionary<Factory, int>> possibleTakeOvers)
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
                        source = FactorySourceAndFactoryDest.Key;
                        factoryDestinationAndDistance = newFactoryDestinationAndDistance;
                        destination = factoryDestinationAndDistance.Key;
                    }
                    else
                    {
                        if (!IsCurrentTargetBest(source.Cyborgs, newSource.Cyborgs, factoryDestinationAndDistance, newFactoryDestinationAndDistance))
                        {
                            source = newSource;
                            factoryDestinationAndDistance = newFactoryDestinationAndDistance;
                            destination = newFactoryDestinationAndDistance.Key;
                        }
                    }
                }
            }
            Factory[] factories = { source, destination };
            return factories;
        }

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

            Console.Error.WriteLine(answer);
            return answer;
        }

        private static Dictionary<Factory, Dictionary<Factory, int>> FindPossibleTakeOvers(List<Factory> ownedFactories)
        {
            Dictionary<Factory, Dictionary<Factory, int>> possibleTakeOvers = new Dictionary<Factory, Dictionary<Factory, int>>();
            for (int i = 0; i < ownedFactories.Count; i++)
            {
                Factory factory = ownedFactories[i];
                int numberOfLinks = factory.Links.Count;
                Console.Error.WriteLine($"Owned factory = {factory}");

                Dictionary<Factory, int> possibleDestinations = new Dictionary<Factory, int>();
                for (int links = 0; links < numberOfLinks; links++)
                {
                    Factory destinationFactory = GetDestinationFactory(factory, links);
                    if (factory.Cyborgs > destinationFactory.Cyborgs)
                    {
                        possibleDestinations.Add(destinationFactory, factory.Links[links].Distance);
                        Console.Error.WriteLine($"Possible Destination = {destinationFactory}");
                    }
                }
                if (possibleDestinations.Count > 0)
                {
                    possibleTakeOvers.Add(factory, possibleDestinations);
                }
            }
            return possibleTakeOvers;
        }

        private static Factory GetDestinationFactory(Factory factory, int links)
        {
            Factory destinationFactory;
            if (factory.Links[links].Factory1.Id == factory.Id)
            {
                destinationFactory = factory.Links[links].Factory2;
            }
            else
            {
                destinationFactory = factory.Links[links].Factory1;
            }

            return destinationFactory;
        }

        private void UpdateFactory(Factory factory)
        {
            factory.Owner = (Owner)int.Parse(_Inputs[2]);
            factory.Cyborgs = int.Parse(_Inputs[3]);
            factory.Production = int.Parse(_Inputs[4]);
        }

        private void BuildFactories()
        {
            int factoryCount = int.Parse(Console.ReadLine());

            for (int i = 0; i < factoryCount; i++)
            {
                Factories.Add(new Factory() { Id = i });
            }
            int linkCount = int.Parse(Console.ReadLine()); // the number of links between factories

            for (int i = 0; i < linkCount; i++)
            {
                _Inputs = Console.ReadLine().Split(' ');
                Factory factory1 = Factories.First(fac => fac.Id == int.Parse(_Inputs[0]));
                Factory factory2 = Factories.First(fac => fac.Id == int.Parse(_Inputs[1]));
                int distance = int.Parse(_Inputs[2]);

                Link link = new Link()
                {
                    Factory1 = factory1,
                    Factory2 = factory2,
                    Distance = distance
                };
                factory1.Links.Add(link);
                factory2.Links.Add(link);
            }
        }
    }
}
