using GhostInTheCellAI.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GhostInTheCellAI
{
    public class AI
    {
        private readonly Game _game;
        private readonly ActionServiceV2 _actionService;
        //TODO add remaining turns into calculation.
        //TODO refactor so you calculate factories for each new action so you can take TTA into account.
        //TODO calculate if 2 factories together can take over.
        // To debug: Console.Error.WriteLine("Debug messages...");

        public AI(ActionServiceV2 actionService, Game game)
        {
            _game = game;
            _actionService = actionService;
        }

        //TODO send units closer to enemy.
        public void Run()
        {
            // game loop
            while (true)
            {
                _game.UpdateGame();

                foreach (var troop in _game.Troops)
                {
                    Console.Error.WriteLine(troop);
                }

                List<GameAction> actions = new List<GameAction>();

                List<MoveGameAction> cyborgActions = _actionService.GetPossibleCyborgActions(_game);

                Console.Error.WriteLine();
                Console.Error.WriteLine("Current Factories");
                foreach (var factory in _game.Factories)
                {
                    Console.Error.WriteLine(factory);
                }
                cyborgActions.Sort(delegate (MoveGameAction action1, MoveGameAction action2) { return action2.Score.CompareTo(action1.Score); });

                foreach (var cyborgAction in cyborgActions)
                {
                    if (cyborgAction.IsPossible())
                    {
                        Factory currentSourceFactory = _game.Factories.First(F => F.Id == cyborgAction.Source.Id);
                        if (currentSourceFactory.Owner == Owner.Player)
                        {
                            actions.Add(cyborgAction);
                            cyborgAction.PlayOut();
                        }
                    }
                }

                if (_game.RemainingBombs > 0)
                {
                    List<Factory> unavailableFactories = (from a in actions select a.Source).ToList();
                    Console.Error.WriteLine(unavailableFactories.Count);
                    GameAction bombAction = _actionService.GetBombAction(_game, unavailableFactories);
                    if (bombAction != null)
                    {
                        actions.Add(bombAction);
                        _game.RemainingBombs -= 1;
                    }
                }

                foreach (var a in actions)
                {
                    Console.Error.WriteLine(a.WriteAction());
                }

                if (!actions.Any())
                {
                    List<IncreaseGameAction> increaseProduction = _actionService.GetPossibleFactoryProductionIncrease(_game);
                    if (increaseProduction.Any())
                    {
                        increaseProduction.Sort(delegate (IncreaseGameAction action1, IncreaseGameAction action2) { return action2.Score.CompareTo(action1.Score); });
                        actions.Add(increaseProduction[0]);
                    }

                }
                //TODO Increaseroduction Action
                //TODO bomb Defence  HOW TO: Save factories that have TTA for bomb and when its about to hit send to other factory.
                /*
                 * 
                */
                string action = String.Join(";", actions);

                if (string.IsNullOrEmpty(action))
                {
                    action = "WAIT";
                }
                Console.WriteLine(action);
            }
        }

    }
}