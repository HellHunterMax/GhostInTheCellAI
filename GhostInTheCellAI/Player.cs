using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GhostInTheCellAI.Models;

namespace GhostInTheCellAI
{
    class Player
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            ActionServiceV2 actionService = new ActionServiceV2();
            AI ai = new AI(actionService, game);
            ai.Run();
        }
    }
}
