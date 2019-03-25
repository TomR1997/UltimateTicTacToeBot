using System;
using System.Collections.Generic;

namespace UltimateTicTacToeBot.Bot
{
    /// <summary>
    ///  This class stores all settings of the game and the information about the current state of the game.
    ///  When calling this in BotStarter.doMove(), you can trust that this state has been update to current 
    ///  game state (because updates are sent before action request).
    /// </summary>
    class BotState
    {
        public int RoundNumber { get; set; }
        public int Timebank { get; set; }
        public int TimePerMove { get; set; }
        public int MaxTimebank { get; set; }        
        public String MyName { get; set; }
        public int MoveNumber { get; set; }                
        public Dictionary<String, Player> Players { get; set; }
        public Field Field { get; set; }

        public BotState()
        {
            Field = new Field();
            Players = new Dictionary<string, Player>();
        }
    }
}
