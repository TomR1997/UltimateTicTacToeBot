using System;
using System.Linq;

namespace UltimateTicTacToeBot.Bot
{
    class BotStarter
    {
        private Random rand = new Random();

		/// <summary>
		/// Returns the next move to make. Edit this method to make your bot smarter.
		/// Currently does only random moves.
		/// </summary>
		/// <param name="state"></param>
		/// <returns>The column where the turn was made</returns>
		public Move GetMove(BotState state)
        {
            var moves = state.Field.GetAvailableMoves();

            if (moves.Count > 0)
            {
                // get random move from available moves
                return moves.ElementAt(rand.Next(moves.Count)); 
            }

            // pass
            return null;
        }

        static void Main(string[] args)
        {
            BotParser parser = new BotParser(new BotStarter());
            parser.Run();
        }

    }
}
