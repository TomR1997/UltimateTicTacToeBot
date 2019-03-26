using System;

namespace UltimateTicTacToeBot
{
    /// <summary>
    /// Stores a move.
    /// </summary>
    class Move
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Move(int x, int y)
        {
            X = x;
            Y = y;
        }
       
        override public string ToString()
        {
            return string.Format("place_move {0} {1}", X, Y);
        }
    }
}
