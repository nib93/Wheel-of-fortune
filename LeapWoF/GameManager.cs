using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using LeapWoF.Interfaces;

namespace LeapWoF
{

    /// <summary>
    /// The GameManager class, handles all game logic
    /// </summary>
    public class GameManager
    {

        /// <summary>
        /// The input provider
        /// </summary>
        private IInputProvider inputProvider;

        /// <summary>
        /// The output provider
        /// </summary>
        private IOutputProvider outputProvider;

        private string userGuess;
        private string TemporaryPuzzle;
        public List<string> charGuessList = new List<string>();
        public List<int> money = new List<int> {
         -1,
         0,
         10,
         20,
         30,
         40,
         50
        };

        public Dictionary<string, decimal> currentRoundMoney = new Dictionary<string, decimal>();   
        public Dictionary<string, decimal> totalMoney = new Dictionary<string, decimal>();   

        public int Total_winning_money = 0;
        public int Current_winning_money = 0;
        public int spinning_money = 0;
        public List<string> playerNames = new List<string> { "Denvinn Magsino", "Marlon Hernandez" };
        public GameState GameState { get; private set; }
        public string player;
        // Don't change this constructor, intiate a new Game Player
        public GameManager() : this(new ConsoleInputProvider(), new ConsoleOutputProvider())
        {

        }

        public GameManager(IInputProvider inputProvider, IOutputProvider outputProvider)
        {
            if (inputProvider == null)
                throw new ArgumentNullException(nameof(inputProvider));
            if (outputProvider == null)
                throw new ArgumentNullException(nameof(outputProvider));

            this.inputProvider = inputProvider;
            this.outputProvider = outputProvider;

            GameState = GameState.WaitingToStart;
        }

        /// <summary>
        /// Manage game according to game state
        /// </summary>
        public void StartGame()
        {
            InitGame();

            while (true)
            {

                PerformSingleTurn();

                if (GameState == GameState.RoundOver)
                {

                    StartNewRound();
                    continue;
                }

                if (GameState == GameState.GameOver)
                {
                    
                    outputProvider.WriteLine("Game over");
                    break;
                }
            }
        }

        public void StartNewRound()
        {
            InitRoundMoney();   
            TemporaryPuzzle = "Hello World";
            // update the game state
            GameState = GameState.RoundStarted;
        }

        public void DisplayPlayerNames()
        {
            outputProvider.WriteLine();
            foreach (string playerName in playerNames)
            {
                outputProvider.WriteLine($"{playerName}!");
            }
            outputProvider.WriteLine();
        }

        public void PerformSingleTurn()
        {
            DrawPuzzle();
            outputProvider.WriteLine("Type 1 to spin, 2 to solve, 3 to display round money");
            GameState = GameState.WaitingForUserInput;

            var action = inputProvider.Read();

            switch (action)
            {
                case "1":
                    Spin();
                    break;
                case "2":
                    Solve();
                    break;
                case "3":
                    ShowRoundMoney();
                    break;
            }
        }

        /// <summary>
        /// Draw the puzzle
        /// </summary>
        private void DrawPuzzle()
        {
            outputProvider.WriteLine("The puzzle is:");
            outputProvider.WriteLine(boardStatus());
            outputProvider.WriteLine();
        }

        public void ShowRoundMoney()
        {
            foreach(var playerName in playerNames)
            {
                Console.WriteLine($"{playerName}: {currentRoundMoney[playerName]}");
            }
        }

        public void ShowTotalMoney()
        {
            foreach(var playerName in playerNames)
            {
                Console.WriteLine($"{playerName}: {currentRoundMoney[playerName]}");
            }
        }

        public void InitTotalMoney()
        {
            foreach (var playerName in playerNames)
            {
                totalMoney.Add(playerName , 0m);
            }
        }

        public void InitRoundMoney()
        {
            foreach (var playerName in playerNames)
            {
                currentRoundMoney.Add(playerName , 0m);
            }
        }

        public void AddRoundMoney(string name, decimal amt)
        {
            currentRoundMoney[name] += amt;  
            Console.WriteLine(name, amt);
        }

       public void AddTotalMoney(string name, decimal amt)
        {
            totalMoney[name] += amt;  
        }

        public string boardStatus()
        {
            string status = "";

            foreach (var puzzleChar in TemporaryPuzzle)
            {
                string stringChar = puzzleChar.ToString();

                if (stringChar == " ") status += " ";
                else if (charGuessList.Contains(puzzleChar.ToString().ToLower()))
                {
                    status += puzzleChar.ToString();
                }
                else status += "_ ";

            }

            return status;
        }

        public void Spin()
        {
            outputProvider.WriteLine("Spin Options:");
            foreach (int i in money)
            {
                if (i == -1) outputProvider.Write("Lose Turn\t");
                else if (i == 0) outputProvider.Write("Bankrupt\t");
                else outputProvider.Write(i + "\t");
            }

            outputProvider.WriteLine();

            outputProvider.WriteLine("Spinning the wheel...");
            //TODO - Implement wheel + possible wheel spin outcomes
            var random = new Random();
            int ran_index = random.Next(money.Count);
            spinning_money = money[ran_index];

            if (spinning_money == 0) Console.WriteLine("Spinner => Bankrupt ");
            else if (spinning_money == -1) Console.WriteLine("Spinner => Lose turn");
            else Console.WriteLine("Spinner => " + spinning_money);
            //b. Lose a turn should be an option and it should immediately end the current turn
            if (spinning_money == -1)
            {
                outputProvider.WriteLine("You lost your turn");
                GameState = GameState.TurnOver;
            }
            //c. Bankrupt should be an option and it should wipe out money for the current round(not total money)
            if (spinning_money == 0)
            {
                Current_winning_money = 0;
                GameState = GameState.TurnOver;
                GuessLetter();
                outputProvider.WriteLine(boardStatus());

            }
            //d. The dollar values should go up every round
            else if (spinning_money > 0)
            {

                GuessLetter();
                if (TemporaryPuzzle.ToLower().Contains(charGuessList[charGuessList.Count - 1]))
                {
                    Current_winning_money += spinning_money;
                    AddRoundMoney(player, spinning_money);
                }
                else
                {
                    GameState = GameState.TurnOver;
                }
                //outputProvider.WriteLine(boardStatus());

            }

        }

        public void Solve()
        {
            outputProvider.WriteLine("Please enter your solution: ");
            var guess = inputProvider.Read();
            guess = guess.ToLower();
            if (guess.ToLower() == TemporaryPuzzle.ToLower())
            {

                outputProvider.WriteLine("Congrats! You solved the puzzle!");
                Total_winning_money += Current_winning_money;

                GameState = GameState.GameOver;
                ShowRoundMoney();
                playerNames.Clear();    
            }

            charGuessList.Add(guess);
        }

        public void GuessLetter()
        {
            outputProvider.Write("Please guess a letter: ");
            var guess = inputProvider.Read().ToLower();

            while (charGuessList.Contains(guess))
            {
                outputProvider.Write("Invalid letter, guess a different letter: ");
                guess = inputProvider.Read();
            };

            charGuessList.Add(guess);
            CheckGuess();
        }

        public void CheckGuess()
        {
            if (TemporaryPuzzle.ToLower().Contains(charGuessList[charGuessList.Count - 1]))
            {
                outputProvider.WriteLine();
                outputProvider.WriteLine("Correct Letter!");
                outputProvider.WriteLine();

            }
        }
        /// <summary>
        /// Optional logic to accept configuration options
        /// Welcomes player to the game and shows contestants
        /// </summary>
        public void InitGame()
        {
            InitTotalMoney();
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            outputProvider.WriteLine("Enter your first name to enter the game arena!\n");
            string activePlayerName = inputProvider.Read();
            Console.Clear();

            outputProvider.WriteLine($"Welcome to the Wheel of Fortune {activePlayerName}");
            player = activePlayerName;
            playerNames.Add(activePlayerName);
            outputProvider.WriteLine("The other contestants are: ");
            DisplayPlayerNames();
            StartNewRound();
        }
    }
}