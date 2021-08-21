using GhostInTheCellAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostInTheCellAI
{
    public class AI
    {
        private List<Factory> Factories = new List<Factory>();
        private string[] _Inputs;
        //TODO add remaining turns into calculation.

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
            //TODO Find best action to take 1 step at a time.
            //TODO create Points System
            /*
             * source troops - destination troops - TTA * production
             * 60 3 10 * 3 = 141
             * 60 10 2 * 1 = 48
             * first step find closest takeover. base TTA.
             * 
             * (if enemy) source troops - (destination troops + (TTA * Produciton))
             */
            //TODO add troops to possibletakeovers
            //TODO step2 Choose best one.
            string action = "WAIT";

            List<Factory> ownedFactories = Factories.FindAll(fac => fac.Owner == Owner.Player);
            Dictionary<Factory, List<Factory>> possibleTakeOvers = FindPossibleTakeOvers(ownedFactories);
            if (possibleTakeOvers.Count < 1)
            {
                return action;
            }

            return action;
        }

        private static Dictionary<Factory, List<Factory>> FindPossibleTakeOvers(List<Factory> ownedFactories)
        {
            Dictionary<Factory, List<Factory>> possibleTakeOvers = new Dictionary<Factory, List<Factory>>();
            for (int i = 0; i < ownedFactories.Count; i++)
            {
                Factory factory = ownedFactories[i];
                int numberOfLinks = factory.Links.Count;

                List<Factory> possibleDestinations = new List<Factory>();
                for (int links = 0; links < numberOfLinks; links++)
                {
                    Factory destinationFactory = GetDestinationFactory(factory, links);
                    if (factory.Cyborgs > destinationFactory.Cyborgs)
                    {
                        possibleDestinations.Add(destinationFactory);
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
