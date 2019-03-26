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
		private readonly BotStarter bot;
		private readonly BotState currentState;
        private readonly string player1 = "0";
        private readonly string player2 = "1";
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

                string[] parts = line.Split(' ');
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
							currentState.Timebank = Convert.ToInt32(parts[2]);

                            Move move = bot.MiniMax(currentState.Field.GetMacroBoard(), currentState.Field.GetBoard(), player1, player2, currentState);

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
        
        /// <summary>
        /// Parses all the game settings given by the engine
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        private void ParseSettings(string key, string value)
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
                        string[] playerNames = value.Split(',');
						foreach (string playerName in playerNames)
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
						Console.Error.WriteLine(string.Format(
								"Cannot parse settings input with key '{0}'", key));
						break;
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(string.Format(
						"Cannot parse settings value '{0}' for key '{1}'", value, key));
				Console.Error.WriteLine(e.StackTrace);
			}
		}

		/// <summary>
		/// Parse data about the game given by the engine
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		private void ParseGameData(string key, string value)
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
						Console.Error.WriteLine(string.Format(
								"Cannot parse game data input with key '{0}'", key));
						break;
				}
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(string.Format(
						"Cannot parse game data value '{0}' for key '{1}'", value, key));
				Console.Error.WriteLine(e.StackTrace);
			}
		}
	}
}
