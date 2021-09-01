using GhostInTheCellAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostInTheCellAI
{
    public class ActionServiceV2
    {
        //TODO some factories take longer to reach ?
        private List<Factory> updatedFactories;
        public List<MoveGameAction> GetPossibleCyborgActions(Game game)
        {
            // Things ot take into account:
            // 4. think about TTA of troops.
            updatedFactories = GetUpDatedFactoriesWithTroopsAtDestinationAndBombsSent(game);
            List<MoveGameAction> gameActions = FindPossibleTakeOvers(updatedFactories, game);

            return gameActions;
        }


        private static List<Factory> GetUpDatedFactoriesWithTroopsAtDestinationAndBombsSent(Game game)
        {
            List<Factory> updatedFactories = new List<Factory>();
            foreach (var factory in game.Factories)
            {
                updatedFactories.Add(new Factory()
                {
                    Cyborgs = factory.Cyborgs,
                    Id = factory.Id,
                    Links = factory.Links,
                    Owner = factory.Owner,
                    Production = factory.Production
                });
            }
            foreach (var troop in game.Troops)
            {
                AddTroopsToFactory(updatedFactories, troop);
            }
            foreach (var bomb in game.Bombs)
            {
                if (bomb.Owner == Owner.Player)
                {
                    UpdateFactoryForBomb(updatedFactories, bomb);
                }
            }
            return updatedFactories;
        }

        private static void UpdateFactoryForBomb(List<Factory> updatedFactories, Bomb bomb)
        {
            Factory fac = updatedFactories.First(x => x.Id == bomb.Destination.Id);
            fac.Cyborgs += fac.Production * bomb.TurnsToArrive;
            if (fac.Cyborgs > 20)
            {
                fac.Cyborgs /= 2;
            }
            else if (fac.Cyborgs < 10)
            {
                fac.Cyborgs = 0;
            }
            else
            {
                fac.Cyborgs -= 10;
            }
        }

        internal GameAction GetBombAction(Game game, List<Factory> unavailableFactories)
        {
            List<BombGameAction> actions = new List<BombGameAction>();
            List<Factory> PossibleSource = new List<Factory>();
            foreach (var factory in game.Factories)
            {
                if (factory.Owner == Owner.Player && !unavailableFactories.Any(f => f.Id == factory.Id))
                {
                    PossibleSource.Add(factory);
                }
            }

            if (!PossibleSource.Any())
            {
                return null;
            }
            int bombDestination = -1;
            foreach (var bomb in game.Bombs)
            {
                if (bomb.Owner == Owner.Player)
                {
                    bombDestination = bomb.Destination.Id;
                }
            }

            foreach (var factory in updatedFactories)
            {
                if (factory.Id == bombDestination)
                {
                    continue;
                }
                if (factory.Owner == Owner.Enemy && factory.Production >1)
                {
                    Factory source = PossibleSource[0];
                    int distance = int.MaxValue;
                    foreach (var sourceFactory in PossibleSource)
                    {
                        for (int links = 0; links < sourceFactory.Links.Count; links++)
                        {
                            Factory destinationFactory = updatedFactories.First(F => F.Id == GetDestinationFactory(sourceFactory, links).Id);
                            if (destinationFactory == factory)
                            {
                                if (sourceFactory.Links[links].Distance < distance)
                                {
                                    source = sourceFactory;
                                    distance = sourceFactory.Links[links].Distance;
                                }
                                break;
                            }
                        }
                    }
                    var action = new BombGameAction(source, factory, distance);
                    if (action.IsValid(game))
                    {
                        actions.Add(action);
                    }
                }
            }
            if (actions.Any())
            {
                actions.Sort(delegate (BombGameAction action1, BombGameAction action2) { return action2.Score.CompareTo(action1.Score); });
                return actions[0];
            }
            return null;
        }

        internal List<IncreaseGameAction> GetPossibleFactoryProductionIncrease(Game game)
        {
            //Factory cyborgs > 10
            List<Factory> ownedFactories = GetPlayerFactories(game);
            List<Factory> ownedFactoriesWhoCanIncrease = ownedFactories.FindAll(x => x.Cyborgs > 9 && x.Production < 3);

            Console.Error.WriteLine($"Factories who can increse = {ownedFactoriesWhoCanIncrease.Count}");
            if (!ownedFactoriesWhoCanIncrease.Any())
            {
                return null;
            }

            Factory farthestFactory = null;
            GetFurthestFactory(out farthestFactory, ownedFactoriesWhoCanIncrease, game);
            int closestDistance = 0;
            Factory closestFactory = null;
            GetClosestFactory(out closestFactory, out closestDistance, ownedFactories, game);

            Console.Error.WriteLine($"farthestFactory = {farthestFactory}, closestFactory = {closestFactory}");
            if (farthestFactory == null || closestFactory == null)
            {
                return null;
            }
            List<IncreaseGameAction> actions = new List<IncreaseGameAction>();

            if (CanIncrease(game, closestDistance))
            {
                foreach (var factory in ownedFactoriesWhoCanIncrease)
                {
                    actions.Add(new IncreaseGameAction(factory));
                }
            }

            // All my Cyborgs + prod * minimum time (farthest) > all their cyborgs. then you may upgrade.
            //Get farthest factory.
            //Calculate
            //upgrade.
            return actions;
        }

        /// <summary>
        /// This calculates if you can increase a Factory.
        /// </summary>
        /// <param name="game"></param>
        /// <param name="closestDistance"></param>
        /// <returns></returns>
        private static bool CanIncrease(Game game, int closestDistance)
        {
            int myCyborgsCount = 0;
            int enemyCyborgsCount = 0;

            foreach (var factory in game.Factories)
            {
                if (factory.Owner == Owner.Enemy)
                {
                    enemyCyborgsCount += factory.Cyborgs;
                }
                else if (factory.Owner == Owner.Player)
                {
                    myCyborgsCount += factory.Cyborgs + (factory.Production * closestDistance);
                }
            }

            Console.Error.WriteLine($"myCyborgsCount - 10 > enemyCyborgsCount = {myCyborgsCount - 10 > enemyCyborgsCount}");

            if (myCyborgsCount - 10 > enemyCyborgsCount)
            {
                return true;
            }
            return false;
        }

        private static void GetClosestFactory(out Factory closestFactory, out int closestDistance, List<Factory> factories, Game game)
        {
            closestFactory = null;
            closestDistance = int.MaxValue;
            foreach (var playerFactory in factories)
            {
                for (int links = 0; links < playerFactory.Links.Count; links++)
                {
                    Factory destinationFactory = game.Factories.First(F => F.Id == GetDestinationFactory(playerFactory, links).Id);
                    int dist = playerFactory.Links[links].Distance;
                    if (destinationFactory.Owner == Owner.Enemy && dist < closestDistance)
                    {
                        closestDistance = dist;
                        closestFactory = destinationFactory;
                    }
                }
            }
        }

        private static void GetFurthestFactory(out Factory farthestFactory, List<Factory> factories, Game game)
        {
            farthestFactory = null;
            int FurthestDistance = 0;
            foreach (var playerFactory in factories)
            {
                for (int links = 0; links < playerFactory.Links.Count; links++)
                {
                    Factory destinationFactory = game.Factories.First(F => F.Id == GetDestinationFactory(playerFactory, links).Id);
                    int dist = playerFactory.Links[links].Distance;
                    if (destinationFactory.Owner == Owner.Enemy && dist > FurthestDistance)
                    {
                        FurthestDistance = dist;
                        farthestFactory = destinationFactory;
                    }
                }
            }
        }

        private static void AddTroopsToFactory(List<Factory> updatedFactories, Troop troop)
        {
            var factory = updatedFactories.First(x => x.Id == troop.Destination.Id);

            if (troop.Source.Owner == troop.Destination.Owner)
            {
                factory.Cyborgs += troop.Size;
            }
            else
            {
                factory.Cyborgs -= troop.Size;
                if (factory.Cyborgs < 0)
                {
                    factory.Owner = troop.Source.Owner;
                    factory.Cyborgs = Math.Abs(factory.Cyborgs);
                }
            }
        }


        //TODO change when to takeover factories (neutral)
        private static List<MoveGameAction> FindPossibleTakeOvers(List<Factory> futureFactories, Game game)//TODO REfactor FindPossibleTakovers
        {
            List<MoveGameAction> possibleTakeOvers = new List<MoveGameAction>();
            List<Factory> ownedFactories = GetPlayerFactories(game);

            for (int i = 0; i < ownedFactories.Count; i++)
            {
                Factory factory = futureFactories.First(F => F.Id == ownedFactories[i].Id);
                int numberOfLinks = factory.Links.Count;
                //Console.Error.WriteLine($"Owned factory = {factory}");

                for (int links = 0; links < numberOfLinks; links++)
                {
                    Factory destinationFactory = futureFactories.First(F => F.Id == GetDestinationFactory(factory, links).Id);
                    if (destinationFactory.Owner == Owner.Enemy && destinationFactory.Production > 0)
                    {
                        int destinationCyborgs = destinationFactory.Cyborgs + (destinationFactory.Production * factory.Links[links].Distance);
                        if (destinationCyborgs > 0 && factory.Cyborgs > destinationCyborgs)
                        {
                            possibleTakeOvers.Add(new MoveGameAction(factory, destinationFactory, factory.Links[links].Distance));
                        }
                    }
                    else if (destinationFactory.Owner == Owner.Neutral && destinationFactory.Production > 0)
                    {
                        if (factory.Cyborgs > destinationFactory.Cyborgs)
                        {
                            possibleTakeOvers.Add(new MoveGameAction(factory, destinationFactory, factory.Links[links].Distance));
                        }
                    }
                    else
                    {
                        if (destinationFactory.Cyborgs < 0 && factory.Cyborgs > Math.Abs(destinationFactory.Cyborgs))
                        {
                            possibleTakeOvers.Add(new MoveGameAction(factory, destinationFactory, factory.Links[links].Distance));
                            //Console.Error.WriteLine($"Possible Destination = {destinationFactory}");
                        }
                    }

                }
            }
            return possibleTakeOvers;
        }

        private static List<Factory> GetPlayerFactories(Game game)
        {
            return game.Factories.FindAll(fac => fac.Owner == Owner.Player);
        }

        private static Factory GetDestinationFactory(Factory factory, int linknumber)
        {

            return factory.Links[linknumber].Factory1.Id == factory.Id ? factory.Links[linknumber].Factory2 : factory.Links[linknumber].Factory1;
        }

    }
}
