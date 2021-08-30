using GhostInTheCellAI.Models;
using System;
using System.Collections.Generic;

namespace GhostInTheCellAI
{
    public class AI
    {
        private readonly Game _game;
        private readonly ActionServiceV2 _actionService;
        //TODO add remaining turns into calculation.
        //TODO split the calculate best action so you can get multiple actions.
        // To debug: Console.Error.WriteLine("Debug messages...");

        public AI(ActionServiceV2 actionService, Game game)
        {
            _game = game;
            _actionService = actionService;
        }

        public void Run()
        {
            // game loop
            while (true)
            {
                _game.UpdateGame();

                List<GameAction> actions = new List<GameAction>();

                List<MoveGameAction> cyborgActions = _actionService.GetPossibleCyborgActions(_game);

                foreach (var factory in _game.Factories)
                {
                    Console.Error.WriteLine(factory);
                }
                cyborgActions.Sort(delegate (MoveGameAction action1, MoveGameAction action2) { return action1.Score.CompareTo(action2.Score); });

                foreach (var cyborgAction in cyborgActions)
                {
                    if (cyborgAction.IsPossible())
                    {
                        actions.Add(cyborgAction);
                        cyborgAction.PlayOut();
                    }
                }

                foreach (var a in actions)
                {
                    Console.Error.WriteLine(a.WriteAction());
                }
                //TODO BombAction
                //TODO Increaseroduction Action
                //TODO bomb Defence
                /*
                GameAction bombAction = _actionService.GetPossibleBombAction(_game, unavailableFactories);
                List<GameAction> increaseProduction = _actionService.GetPossibleFactoryProductionIncrease(_game);
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