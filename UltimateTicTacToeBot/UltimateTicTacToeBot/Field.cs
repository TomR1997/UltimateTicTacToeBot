using System;
using System.Collections.Generic;
using System.Text;

namespace UltimateTicTacToeBot
{
    /// <summary>
    /// Handles everything that has to do with the field, such as storing 
    /// the current state and performing calculations on the field.
    /// </summary>
    class Field
    {
        public const String EmptyField = ".";
        public const String AvailableField = "-1";

        // Size of board
        public const int Cols = 9;
        public const int Rows = 9;

        public int MyId { get; set; }
        public int OpponentId { get; set; }      
        
        private string[,] board;
        private string[,] macroboard;

        public Field()
        {
            board = new string[Cols, Rows];
            macroboard = new string[Cols / 3, Rows / 3];
            ClearBoard();
        }

        /// <summary>
        /// Initialise field from comma separated String
        /// </summary>
        /// <param name="s"></param>
        public void ParseFromString(String s)
        {
            s = s.Replace(";", ",");
            string[] r = s.Split(',');
            int counter = 0;
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {
                    board[x, y] = r[counter];
                    counter++;
                }
            }
        }

        /// <summary>
        /// Initialise macroboard from comma separated String
        /// </summary>
        /// <param name="s"></param>
        public void ParseMacroboardFromString(String s)
        {
            String[] r = s.Split(',');
            int counter = 0;
            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    macroboard[x, y] = r[counter];
                    counter++;
                }
            }
        }

        public void ClearBoard()
        {
            for (int x = 0; x < Cols; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    board[x, y] = EmptyField;
                }
            }
        }

        public List<Move> GetAvailableMoves()
        {
            var moves = new List<Move>();

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {
                    if (IsInActiveMicroboard(x, y) && board[x, y] == EmptyField)
                    {
                        moves.Add(new Move(x, y));
                    }
                }
            }

            return moves;
        }

        public bool IsInActiveMicroboard(int x, int y)
        {
            return macroboard[x / 3, y / 3] == AvailableField;
        }
        

        /// <summary>
        /// Creates comma separated String with player ids for the microboards.
        /// </summary>
        /// <returns>String with player names for every cell, or 'empty' when cell is empty.</returns>
        override public string ToString()
        {
            var r = new StringBuilder("");
            int counter = 0;
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Cols; x++)
                {
                    if (counter > 0)
                    {
                        r.Append(",");
                    }
                    r.Append(board[x, y]);
                    counter++;
                }
            }
            return r.ToString();
        }


        /// <summary>
        /// Checks whether the field is full
        /// </summary>
        /// <returns>Returns true when field is full, otherwise returns false</returns>
        public bool IsFull()
        {
            for (int x = 0; x < Cols; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (board[x, y] == EmptyField)
                    {
                        // At least one cell is not filled
                        return false; 
                    }
                }
            }
            // All cells are filled
            return true;
        }
        
        public bool IsEmpty()
        {
            for (int x = 0; x < Cols; x++)
            {
                for (int y = 0; y < Rows; y++)
                {
                    if (board[x, y] != EmptyField)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Returns the player id for the given column and row
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string GetPlayerId(int column, int row)
        {
            return board[column, row];
        }
		
		public string[,] GetBoard()
		{
			return board;
		}

		public string[,] GetMacroBoard()
		{
			return macroboard;
		}
	}
}
