using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhostInTheCellAI.Models
{
    public abstract class GameAction
    {
        public Factory Source { get; protected set; }
        public int Distance { get; protected set; }
        public int Score { get; set; }
        public abstract string Name();

        public static bool operator <(GameAction gameAction1, GameAction gameAction2)
        {
            return gameAction1.Score < gameAction2.Score;
        }
        public static bool operator >(GameAction gameAction1, GameAction gameAction2)
        {
            return gameAction1.Score > gameAction2.Score;
        }
        public static bool operator <=(GameAction gameAction1, GameAction gameAction2)
        {
            return gameAction1.Score <= gameAction2.Score;
        }
        public static bool operator >=(GameAction gameAction1, GameAction gameAction2)
        {
            return gameAction1.Score >= gameAction2.Score;
        }
        public abstract override string ToString();
        public abstract string WriteAction();
    }
}
