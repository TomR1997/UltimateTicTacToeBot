using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimateTicTacToeBot.Bot
{
	/// <summary>
	/// Main class that will keep reading output from the engine.
	/// Will either update the bot state or get actions.
	/// </summary>
	class BotParser
	{
		private BotStarter bot;
		private BotState currentState;
		private string player1 = "0";
		private string player2 = "1";
		private string empty = ".";
		private string mandatory = "-1";
        public BotParser(BotStarter bot)
		{
			this.bot = bot;
			currentState = new BotState();
		}

		public void Run()
		{
			string line;

			while ((line = Console.ReadLine()) != null)
			{
				if (line.Length == 0) continue;

				String[] parts = line.Split(' ');
				switch (parts[0])
				{
					case "settings":
						ParseSettings(parts[1], parts[2]);
						break;
					case "update":
						if (parts[1].Equals("game"))
						{
							ParseGameData(parts[2], parts[3]);
						}
						break;
					case "action":
						if (parts[1].Equals("move"))
						{
							// move requested 
							currentState.Timebank = Convert.ToInt32(parts[2]);
                            Move move = MiniMax(currentState.Field.GetMacroBoard(), currentState.Field.GetBoard(), player1, player2);

                            if (move != null)
							{

								Console.WriteLine(move);
							}
							else
							{
								Console.WriteLine("pass");
							}
						}
						break;
					default:
						Console.WriteLine("unknown command");
						break;
				}
			}
		}
        public Move MiniMax(string[,] macroboard, string[,] board, string p1, string p2)
        {
            var moves = currentState.Field.GetAvailableMoves();
            var scores = new List<int>();

            var allMoves = new List<Move>();

            foreach (Move m in moves)
            {
                board[m.X, m.Y] = p1;
                scores.Add(CalculateScore(macroboard, board, m, GetTopLeft(m), p1, p2, true));
                allMoves.Add(m);

                board[m.X, m.Y] = p2;
                int score = CalculateScore(macroboard, board, m, GetTopLeft(m), p1, p2, false);
                if (score > 0)
                {
                    scores.Add(score);
                    allMoves.Add(m);
                }

                board[m.X, m.Y] = empty;
            }

            return BestMove(scores, allMoves);
        }

        public bool CheckWinGame(string[,] board, string player)
		{
			if (
				board[0, 0].Equals(player) && board[1, 0].Equals(player) && board[2, 0].Equals(player) ||
				board[0, 1].Equals(player) && board[1, 1].Equals(player) && board[2, 1].Equals(player) ||
				board[0, 2].Equals(player) && board[1, 2].Equals(player) && board[2, 2].Equals(player) ||

				board[0, 0].Equals(player) && board[0, 1].Equals(player) && board[0, 2].Equals(player) ||
				board[1, 0].Equals(player) && board[1, 1].Equals(player) && board[1, 2].Equals(player) ||
				board[2, 0].Equals(player) && board[2, 1].Equals(player) && board[2, 2].Equals(player) ||

				board[0, 0].Equals(player) && board[1, 1].Equals(player) && board[2, 2].Equals(player) ||
				board[0, 2].Equals(player) && board[1, 1].Equals(player) && board[2, 0].Equals(player))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public int CalculateScore(string[,] macroboard, string[,] board, Move move, Move topLeft, string p1, string p2, bool isp1)
		{
			if (CheckWinGame(macroboard, p1))
			{
				return 1000;
			}
			else if (CheckWinGame(macroboard, p2))
			{
				return -1000;
			}
			else if (CheckWinTile(topLeft, board, p1))
			{
				return 100;
			}
			else if (CheckWinTile(topLeft, board, p2))
			{
				return -100;
			}
            else if (isp1)
            {
                return CheckGameWonOrFull(BestInRow(board, move, topLeft, p1), macroboard, board, move, topLeft, p1, p2);
            }
            else
            {
                return -BestInRow(board, move, topLeft, p2);
            }
		}
        public Move GetTopLeft(Move move)
        {
            var x = 0;
            var y = 0;

            if (move.X >= 0 && move.X <= 2)
            {
                x = 0;
            }
            else if (move.X >= 3 && move.X <= 5)
            {
                x = 3;
            }
            else
            {
                x = 6;
            }

            if (move.Y >= 0 && move.Y <= 2)
            {
                y = 0;
            }
            else if (move.Y >= 3 && move.Y <= 5)
            {
                y = 3;
            }
            else
            {
                y = 6;
            }

            return new Move(x, y);
        }

        public bool CheckWinTile(Move topLeft, string[,] board, string player)
        {
            if (board[topLeft.X, topLeft.Y].Equals(player) && board[topLeft.X + 1, topLeft.Y].Equals(player) && board[topLeft.X + 2, topLeft.Y].Equals(player) ||
                board[topLeft.X, topLeft.Y + 1].Equals(player) && board[topLeft.X + 1, topLeft.Y + 1].Equals(player) && board[topLeft.X + 2, topLeft.Y + 1].Equals(player) ||
                board[topLeft.X, topLeft.Y + 2].Equals(player) && board[topLeft.X + 1, topLeft.Y + 2].Equals(player) && board[topLeft.X + 2, topLeft.Y + 2].Equals(player) ||

                board[topLeft.X, topLeft.Y].Equals(player) && board[topLeft.X, topLeft.Y + 1].Equals(player) && board[topLeft.X, topLeft.Y + 2].Equals(player) ||
                board[topLeft.X + 1, topLeft.Y].Equals(player) && board[topLeft.X + 1, topLeft.Y + 1].Equals(player) && board[topLeft.X + 1, topLeft.Y + 2].Equals(player) ||
                board[topLeft.X + 2, topLeft.Y].Equals(player) && board[topLeft.X + 2, topLeft.Y + 1].Equals(player) && board[topLeft.X + 2, topLeft.Y + 2].Equals(player) ||

                board[topLeft.X, topLeft.Y].Equals(player) && board[topLeft.X + 1, topLeft.Y + 1].Equals(player) && board[topLeft.X + 2, topLeft.Y + 2].Equals(player) ||
                board[topLeft.X + 2, topLeft.Y].Equals(player) && board[topLeft.X + 1, topLeft.Y + 1].Equals(player) && board[topLeft.X, topLeft.Y + 2].Equals(player)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		public Move BestMove(List<int> scores, List<Move> moves)
		{
			int p1bestmove = scores.Max();
			int p2bestmove = scores.Min();

			if (p1bestmove >= (p2bestmove * -1))
			{
				return moves[scores.IndexOf(p1bestmove)];
			}
			else
			{
				return moves[scores.IndexOf(p2bestmove)];
            }
		}
        public int BestInRow(string[,] board, Move move, Move topLeft, string player)
        {
            int score = 0;

            Move originMove = new Move(move.X - topLeft.X, move.Y - topLeft.Y);

            if (originMove.X == 0)
            {
                if (originMove.Y == 0)
                {
                    score += CalculateRowValueOne(board, topLeft, player);
                    score += CalculateColumnValueOne(board, topLeft, player);
                    score += CalculateDiagonalValueOne(board, topLeft, player);
                }
                else if (originMove.Y == 1)
                {
                    score += CalculateRowValueTwo(board, topLeft, player);
                    score += CalculateColumnValueOne(board, topLeft, player);
                }
                else
                {
                    score += CalculateColumnValueOne(board, topLeft, player);
                    score += CalculateColumnValueThree(board, topLeft, player);
                    score += CalculateDiagonalValueTwo(board, topLeft, player);
                }
            }
            else if (originMove.X == 1)
            {
                if (originMove.Y == 0)
                {
                    score += CalculateRowValueOne(board, topLeft, player);
                    score += CalculateColumnValueTwo(board, topLeft, player);
                }
                else if (originMove.Y == 1)
                {
                    score += CalculateColumnValueTwo(board, topLeft, player);
                    score += CalculateRowValueTwo(board, topLeft, player);
                    score += CalculateDiagonalValueOne(board, topLeft, player);
                    score += CalculateDiagonalValueTwo(board, topLeft, player);
                }
                else
                {
                    score += CalculateColumnValueTwo(board, topLeft, player);
                    score += CalculateRowValueThree(board, topLeft, player);
                }
            }
            else
            {
                if (originMove.Y == 0)
                {
                    score += CalculateRowValueOne(board, topLeft, player);
                    score += CalculateDiagonalValueTwo(board, topLeft, player);
                    score += CalculateColumnValueThree(board, topLeft, player);
                }
                else if (originMove.Y == 1)
                {
                    score += CalculateRowValueTwo(board, topLeft, player);
                    score += CalculateColumnValueThree(board, topLeft, player);
                }
                else
                {
                    score += CalculateColumnValueThree(board, topLeft, player);
                    score += CalculateRowValueThree(board, topLeft, player);
                    score += CalculateDiagonalValueOne(board, topLeft, player);;
                }
            }

            return score;
        }

        public int CheckGameWonOrFull(int rowScore, string[,] macroboard, string[,] board, Move move, Move topLeft, string p1, string p2)
        {
            Move originMove = new Move(move.X - topLeft.X, move.Y - topLeft.Y);
            if (macroboard[originMove.X, originMove.Y].Equals(p1) || macroboard[originMove.X, originMove.Y].Equals(p2) || CheckEnemyCanWinTile(board, GetTopLeftByGame(originMove), p1))
            {
                return 0;
            }

            return rowScore;
        }

        public Move GetTopLeftByGame(Move originMove)
        {
            int x = 0;
            int y = 0;

            if (originMove.X == 0 && originMove.Y == 0)
            {
                x = 0;
                y = 0;
            }
            else if (originMove.X == 1 && originMove.Y == 0)
            {
                x = 3;
                y = 0;
            }
            else if (originMove.X == 2 && originMove.Y == 0)
            {
                x = 6;
                y = 0;
            }
            else if (originMove.X == 0 && originMove.Y == 1)
            {
                x = 0;
                y = 3;
            }
            else if (originMove.X == 1 && originMove.Y == 1)
            {
                x = 3;
                y = 3;
            }
            else if (originMove.X == 2 && originMove.Y == 1)
            {
                x = 6;
                y = 3;
            }
            else if (originMove.X == 0 && originMove.Y == 2)
            {
                x = 0;
                y = 6;
            }
            else if (originMove.X == 1 && originMove.Y == 2)
            {
                x = 3;
                y = 6;
            }
            else
            {
                x = 6;
                y = 6;
            }
            return new Move(x, y);
        }
        public bool CheckEnemyCanWinTile(string[,] board, Move topLeft, string player)
        {
            int worst = 0;
            worst = WorstScore(worst, CalculateRowValueOne(board, topLeft, player));
            worst = WorstScore(worst, CalculateRowValueTwo(board, topLeft, player));
            worst = WorstScore(worst, CalculateRowValueThree(board, topLeft, player));
            worst = WorstScore(worst, CalculateColumnValueOne(board, topLeft, player));
            worst = WorstScore(worst, CalculateColumnValueTwo(board, topLeft, player));
            worst = WorstScore(worst, CalculateColumnValueThree(board, topLeft, player));
            worst = WorstScore(worst, CalculateDiagonalValueOne(board, topLeft, player));
            worst = WorstScore(worst, CalculateDiagonalValueTwo(board, topLeft, player));

            if (worst <= -2)
            {
                return true;
            }
            return false;
        }

        public int WorstScore(int worst, int current)
        {
            if (current < worst)
            {
                worst = current;
            }
            return worst;
        }
        public int CalculateRowValueOne(string[,] board, Move topLeft, string player)
        {
            int score = 0;
            for (int i = 0; i < 3; i++)
            {
                if (board[topLeft.X + i, topLeft.Y].Equals(player))
                {
                    score += 2;
                }
                else if (board[topLeft.X + i, topLeft.Y].Equals(empty))
                {
                    score += 1;
                }
                else
                {
                    score -= 2;
                }
            }

            return score;
        }

        public int CalculateRowValueTwo(string[,] board, Move topLeft, string player)
        {
            int score = 0;
            for (int i = 0; i < 3; i++)
            {
                if (board[topLeft.X + i, topLeft.Y + 1].Equals(player))
                {
                    score += 2;
                }
                else if (board[topLeft.X + i, topLeft.Y + 1].Equals(empty))
                {
                    score += 1;
                }
                else
                {
                    score -= 2;
                }
            }

            return score;
        }

        public int CalculateRowValueThree(string[,] board, Move topLeft, string player)
        {
            int score = 0;
            for (int i = 0; i < 3; i++)
            {
                if (board[topLeft.X + i, topLeft.Y + 2].Equals(player))
                {
                    score += 2;
                }
                else if (board[topLeft.X + i, topLeft.Y + 2].Equals(empty))
                {
                    score += 1;
                }
                else
                {
                    score -= 2;
                }
            }

            return score;
        }

        public int CalculateColumnValueOne(string[,] board, Move topLeft, string player)
        {
            int score = 0;
            for (int i = 0; i < 3; i++)
            {
                if (board[topLeft.X, topLeft.Y + i].Equals(player))
                {
                    score += 2;
                }
                else if (board[topLeft.X, topLeft.Y + i].Equals(empty))
                {
                    score += 1;
                }
                else
                {
                    score -= 2;
                }
            }

            return score;
        }

        public int CalculateColumnValueTwo(string[,] board, Move topLeft, string player)
        {
            int score = 0;
            for (int i = 0; i < 3; i++)
            {
                if (board[topLeft.X + 1, topLeft.Y + i].Equals(player))
                {
                    score += 2;
                }
                else if (board[topLeft.X + 1, topLeft.Y + i].Equals(empty))
                {
                    score += 1;
                }
                else
                {
                    score -= 2;
                }
            }

            return score;
        }

        public int CalculateColumnValueThree(string[,] board, Move topLeft, string player)
        {
            int score = 0;
            for (int i = 0; i < 3; i++)
            {
                if (board[topLeft.X + 2, topLeft.Y + i].Equals(player))
                {
                    score += 2;
                }
                else if (board[topLeft.X + 2, topLeft.Y + i].Equals(empty))
                {
                    score += 1;
                }
                else
                {
                    score -= 2;
                }
            }

            return score;
        }

        public int CalculateDiagonalValueOne(string[,] board, Move topLeft, string player)
        {
            int score = 0;
            for (int i = 0; i < 3; i++)
            {
                if (board[topLeft.X + i, topLeft.Y + i].Equals(player))
                {
                    score += 2;
                }
                else if (board[topLeft.X + i, topLeft.Y + i].Equals(empty))
                {
                    score += 1;
                }
                else
                {
                    score -= 2;
                }
            }

            return score;
        }

        public int CalculateDiagonalValueTwo(string[,] board, Move topLeft, string player)
        {
            int score = 0;
            for (int i = 0; i < 3; i++)
            {
                if (board[topLeft.X + i, topLeft.Y + 2 - i].Equals(player))
                {
                    score += 2;
                }
                else if (board[topLeft.X + i, topLeft.Y + 2 - i].Equals(empty))
                {
                    score += 1;
                }
                else
                {
                    score -= 2;
                }
            }

            return score;
        }
        /// <summary>
        /// Parses all the game settings given by the engine
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void ParseSettings(String key, String value)
		{
			try
			{
				switch (key)
				{
					case "timebank":
						int time = Convert.ToInt32(value);
						currentState.MaxTimebank = time;
						currentState.Timebank = time;
						break;
					case "time_per_move":
						currentState.TimePerMove = Convert.ToInt32(value);
						break;
					case "player_names":
						String[] playerNames = value.Split(',');
						foreach (String playerName in playerNames)
						{
							Player player = new Player(playerName);
							currentState.Players.Add(playerName, player);
						}
						break;
					case "your_bot":
						currentState.MyName = value;
						break;
					case "your_botid":
						int myId = Convert.ToInt32(value);
						int opponentId = 2 - (myId + 1);
						currentState.Field.MyId = myId;
						currentState.Field.OpponentId = opponentId;
						break;
					default:
						Console.Error.WriteLine(String.Format(
								"Cannot parse settings input with key '{0}'", key));
						break;
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(String.Format(
						"Cannot parse settings value '{0}' for key '{1}'", value, key));
				Console.Error.WriteLine(e.StackTrace);
			}
		}

		/// <summary>
		/// Parse data about the game given by the engine
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private void ParseGameData(String key, String value)
		{
			try
			{
				switch (key)
				{
					case "round":
						currentState.RoundNumber = Convert.ToInt32(value);
						break;
					case "move":
						currentState.MoveNumber = Convert.ToInt32(value);
						break;
					case "field":
						currentState.Field.ParseFromString(value);
						break;
					case "macroboard":
						currentState.Field.ParseMacroboardFromString(value);
						break;
					default:
						Console.Error.WriteLine(String.Format(
								"Cannot parse game data input with key '{0}'", key));
						break;
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(String.Format(
						"Cannot parse game data value '{0}' for key '{1}'", value, key));
				Console.Error.WriteLine(e.StackTrace);
			}
		}
	}
}
