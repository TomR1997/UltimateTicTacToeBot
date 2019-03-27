using System;
using System.Collections.Generic;
using System.Linq;

namespace UltimateTicTacToeBot.Bot
{
    class BotStarter
    {
        private readonly string empty = ".";
        private readonly string mandatory = "-1";

        static void Main(string[] args)
        {
            BotParser parser = new BotParser(new BotStarter());
            parser.Run();
        }

        public Move MiniMax(string[,] macroboard, string[,] board, string p1, string p2, BotState currentState)
        {
            var moves = currentState.Field.GetAvailableMoves();
            var scores = new List<int>();

            var allMoves = new List<Move>();

            foreach (var m in moves)
            {
                board[m.X, m.Y] = p1;
                scores.Add(CalculateScore(macroboard, board, m, GetTopLeft(m), p1, p2, true, currentState));
                allMoves.Add(m);

                board[m.X, m.Y] = p2;
                var score = CalculateScore(macroboard, board, m, GetTopLeft(m), p1, p2, false, currentState);
                if (score > 0)
                {
                    scores.Add(score);
                    allMoves.Add(m);
                }

                board[m.X, m.Y] = empty;
            }

            return BestMove(scores, allMoves);
        }

        public bool CheckWinGame(string[,] macroboard, string player)
        {
            return (
                macroboard[0, 0].Equals(player) && macroboard[1, 0].Equals(player) && macroboard[2, 0].Equals(player) ||
                macroboard[0, 1].Equals(player) && macroboard[1, 1].Equals(player) && macroboard[2, 1].Equals(player) ||
                macroboard[0, 2].Equals(player) && macroboard[1, 2].Equals(player) && macroboard[2, 2].Equals(player) ||

                macroboard[0, 0].Equals(player) && macroboard[0, 1].Equals(player) && macroboard[0, 2].Equals(player) ||
                macroboard[1, 0].Equals(player) && macroboard[1, 1].Equals(player) && macroboard[1, 2].Equals(player) ||
                macroboard[2, 0].Equals(player) && macroboard[2, 1].Equals(player) && macroboard[2, 2].Equals(player) ||

                macroboard[0, 0].Equals(player) && macroboard[1, 1].Equals(player) && macroboard[2, 2].Equals(player) ||
                macroboard[0, 2].Equals(player) && macroboard[1, 1].Equals(player) && macroboard[2, 0].Equals(player));
        }

        public int CalculateScore(string[,] macroboard, string[,] board, Move move, Move topLeft, string p1, string p2, bool isp1, BotState currentState)
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
                return 250;
            }
            else if (CheckWinTile(topLeft, board, p2))
            {
                return -250;
            }
            else if (isp1)
            {
                return CheckGameWonOrFull(macroboard, board, move, topLeft, p1, p2, currentState) ? 0 : BestInRow(board, move, topLeft, p1);
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
            return (board[topLeft.X, topLeft.Y].Equals(player) && board[topLeft.X + 1, topLeft.Y].Equals(player) && board[topLeft.X + 2, topLeft.Y].Equals(player) ||
                board[topLeft.X, topLeft.Y + 1].Equals(player) && board[topLeft.X + 1, topLeft.Y + 1].Equals(player) && board[topLeft.X + 2, topLeft.Y + 1].Equals(player) ||
                board[topLeft.X, topLeft.Y + 2].Equals(player) && board[topLeft.X + 1, topLeft.Y + 2].Equals(player) && board[topLeft.X + 2, topLeft.Y + 2].Equals(player) ||

                board[topLeft.X, topLeft.Y].Equals(player) && board[topLeft.X, topLeft.Y + 1].Equals(player) && board[topLeft.X, topLeft.Y + 2].Equals(player) ||
                board[topLeft.X + 1, topLeft.Y].Equals(player) && board[topLeft.X + 1, topLeft.Y + 1].Equals(player) && board[topLeft.X + 1, topLeft.Y + 2].Equals(player) ||
                board[topLeft.X + 2, topLeft.Y].Equals(player) && board[topLeft.X + 2, topLeft.Y + 1].Equals(player) && board[topLeft.X + 2, topLeft.Y + 2].Equals(player) ||

                board[topLeft.X, topLeft.Y].Equals(player) && board[topLeft.X + 1, topLeft.Y + 1].Equals(player) && board[topLeft.X + 2, topLeft.Y + 2].Equals(player) ||
                board[topLeft.X + 2, topLeft.Y].Equals(player) && board[topLeft.X + 1, topLeft.Y + 1].Equals(player) && board[topLeft.X, topLeft.Y + 2].Equals(player));
        }

        public Move BestMove(List<int> scores, List<Move> moves)
        {
            var p1bestmove = scores.Max();
            var p2bestmove = scores.Min();

			return p1bestmove >= (p2bestmove * -1) ? moves[scores.IndexOf(p1bestmove)] : moves[scores.IndexOf(p2bestmove)];
        }
        public int BestInRow(string[,] board, Move move, Move topLeft, string player)
        {
            var score = 0;
            var originMove = new Move(move.X - topLeft.X, move.Y - topLeft.Y);

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
                    score += CalculateDiagonalValueOne(board, topLeft, player); ;
                }
            }

            return score;
        }

        public bool CheckGameWonOrFull(string[,] macroboard, string[,] board, Move move, Move topLeft, string p1, string p2, BotState currentState)
        {
            var originMove = new Move(move.X - topLeft.X, move.Y - topLeft.Y);
            var microboardMove = new Move(originMove.X * 3, originMove.Y * 3);
            return (macroboard[originMove.X, originMove.Y].Equals(p1) || macroboard[originMove.X, originMove.Y].Equals(p2) || CheckEnemyCanWinTile(macroboard, board, originMove, GetTopLeft(microboardMove), p1, currentState, move)) ? true : false;
        }

        public bool CheckEnemyCanWinTile(string[,] macroboard, string[,] board, Move originMove, Move topLeft, string player, BotState currentState, Move move)
        {
            int worst = 0;
			if (!macroboard[originMove.X, originMove.Y].Equals(mandatory))
			{
				worst = WorstScore(worst, CalculateRowValueOne(board, topLeft, player));
				worst = WorstScore(worst, CalculateRowValueTwo(board, topLeft, player));
				worst = WorstScore(worst, CalculateRowValueThree(board, topLeft, player));
				worst = WorstScore(worst, CalculateColumnValueOne(board, topLeft, player));
				worst = WorstScore(worst, CalculateColumnValueTwo(board, topLeft, player));
				worst = WorstScore(worst, CalculateColumnValueThree(board, topLeft, player));
				worst = WorstScore(worst, CalculateDiagonalValueOne(board, topLeft, player));
				worst = WorstScore(worst, CalculateDiagonalValueTwo(board, topLeft, player));
			}

            return worst < -4;
        }

        public int WorstScore(int worst, int current)
        {
            return current < worst ? current : worst;
        }
        public int CalculateRowValueOne(string[,] board, Move topLeft, string player)
        {
            var score = 0;
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
                    score -= 3;
                }
            }

            return score;
        }

        public int CalculateRowValueTwo(string[,] board, Move topLeft, string player)
        {
            var score = 0;
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
                    score -= 3;
                }
            }

            return score;
        }

        public int CalculateRowValueThree(string[,] board, Move topLeft, string player)
        {
            var score = 0;
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
                    score -= 3;
                }
            }

            return score;
        }

        public int CalculateColumnValueOne(string[,] board, Move topLeft, string player)
        {
            var score = 0;
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
                    score -= 3;
                }
            }

            return score;
        }

        public int CalculateColumnValueTwo(string[,] board, Move topLeft, string player)
        {
            var score = 0;
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
                    score -= 3;
                }
            }

            return score;
        }

        public int CalculateColumnValueThree(string[,] board, Move topLeft, string player)
        {
            var score = 0;
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
                    score -= 3;
                }
            }

            return score;
        }

        public int CalculateDiagonalValueOne(string[,] board, Move topLeft, string player)
        {
            var score = 0;
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
                    score -= 3;
                }
            }

            return score;
        }

        public int CalculateDiagonalValueTwo(string[,] board, Move topLeft, string player)
        {
            var score = 0;
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
                    score -= 3;
                }
            }

            return score;
        }

    }
}
