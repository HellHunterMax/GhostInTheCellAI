﻿using GhostInTheCellAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostInTheCellAI
{
    public class ActionServiceV2
    {
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

            foreach (var factory in updatedFactories)
            {
                if (factory.Owner == Owner.Enemy && factory.Cyborgs > 20 && factory.Production >1)
                {
                    Factory source = PossibleSource[0];
                    int distance = int.MaxValue;
                    foreach (var sourceFactory in PossibleSource)
                    {
                        for (int links = 0; links < sourceFactory.Links.Count; links++)
                        {
                            Factory destinationFactory = updatedFactories.First(F => F.Id == GetDestinationFactory(factory, links).Id);
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
                    actions.Add(new BombGameAction(source, factory, distance));
                }
            }
            actions.Sort(delegate (BombGameAction action1, BombGameAction action2) { return action2.Score.CompareTo(action1.Score); });
            return actions[0];
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

        private static List<MoveGameAction> FindPossibleTakeOvers(List<Factory> futureFactories, Game game)//TODO REfactor FindPossibleTakovers
        {
            List<MoveGameAction> possibleTakeOvers = new List<MoveGameAction>();
            List<Factory> ownedFactories = game.Factories.FindAll(fac => fac.Owner == Owner.Player);

            for (int i = 0; i < ownedFactories.Count; i++)
            {
                Factory factory = futureFactories.First(F => F.Id == ownedFactories[i].Id);
                int numberOfLinks = factory.Links.Count;
                //Console.Error.WriteLine($"Owned factory = {factory}");

                for (int links = 0; links < numberOfLinks; links++)
                {
                    Factory destinationFactory = futureFactories.First(F => F.Id == GetDestinationFactory(factory, links).Id);
                    if (destinationFactory.Owner == Owner.Enemy)
                    {
                        int destinationCyborgs = destinationFactory.Cyborgs + (destinationFactory.Production * factory.Links[links].Distance);
                        if (destinationCyborgs > 0 && factory.Cyborgs > destinationCyborgs)
                        {
                            possibleTakeOvers.Add(new MoveGameAction(factory, destinationFactory, factory.Links[links].Distance));
                        }
                    }
                    else if (destinationFactory.Owner == Owner.Neutral)
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
        private static Factory GetDestinationFactory(Factory factory, int linknumber)
        {

            return factory.Links[linknumber].Factory1.Id == factory.Id ? factory.Links[linknumber].Factory2 : factory.Links[linknumber].Factory1;
        }

    }
}
