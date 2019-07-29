using System;
using System.Collections.Generic;
using System.Linq;

namespace RockPaperScissors
{

    class Program
    {
        static void Main(string[] args)
    {
        var rps = new RPSGame("scissors", "paper", "rock", "lizard", "spock");

            int wins = 0, losses = 0, draws = 0;
            int round = 1;


            while (true)
        {

                if (wins == 3 || losses == 3)
                    break;

                Console.WriteLine("--------------------");
                Console.WriteLine("LETS PLAY!");
                Console.WriteLine("\nRound: {0}", round);
                Console.WriteLine("--------------------");
                Console.WriteLine("First to win 3 rounds is the Game winner!");
                Console.WriteLine("Make your move!: " + string.Join(", ", rps.Choices) + ", or press 'q' to quit the game");
                Console.WriteLine("");
                string choice = Console.ReadLine().Trim().ToLower();

            if (choice == "q")
                break;

            if (!rps.Choices.Contains(choice))

            {
                Console.WriteLine("Invalid choice!");
                continue;
            }

                int result = rps.Next(choice);

                Console.WriteLine("");
                Console.WriteLine("You chose {0} and your opponent chose {1}!", choice, rps.LastCPUChoice);

            switch (result)

            {
                case 1:
                    Console.WriteLine("");
                    Console.WriteLine("{0} beats {1} - You WON this round.", choice, rps.LastCPUChoice);
                        wins++;
                        round++;
                        break;
                case 0:

                    Console.WriteLine("");
                    Console.WriteLine("This round is a DRAW!");
                        draws++;
                        round++;
                        break;
                case -1:
                    Console.WriteLine("");
                    Console.WriteLine("{0} beats {1} - You LOST this round", rps.LastCPUChoice, choice);
                        losses++;
                        round++;
                        break;
            }

            Console.WriteLine("");
            Console.WriteLine("Do you want to play again? Press 'y' for Yes or 'n' for No");
            if (!Console.ReadLine().StartsWith("y", StringComparison.OrdinalIgnoreCase)) break;
        }
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine("MATCH RESULT - Win 3 to win the game.");
            Console.WriteLine("-------------------------------------------------");
            round--;
            Console.WriteLine("The game was completed in {0} moves/rounds", round);
            Console.WriteLine("\nWins: {0}\nLosses: {1}\nDraws: {2}", wins, losses, draws);


            Console.WriteLine("-------------------------------------------------");
            if (losses == 3 && wins < 3)
                Console.WriteLine("You Lost the Match!");

            else if (wins == 3 && losses < 3)
                Console.WriteLine("You Won the Match!");

            else
                Console.WriteLine("Game Ended with no overall winner - 3 wins required");

        }

    class RPSGame
    {
        public RPSGame(params string[] choices)
        {
            Choices = choices;

            _rpsCPU = new RPSCPU(choices);
        }

        // Play next turn.
        public int Next(string choice)
        {

            string cpuChoice = _rpsCPU.NextMove(); // Gets the CPU's next move.
            LastCPUChoice = cpuChoice; // Saves the CPU's move in a property so the player can see it.

            _rpsCPU.AddPlayerMove(choice); // Let the CPU know the players choice
            return GetWinner(Choices, choice, cpuChoice); // Return -1 if CPU wins, 0 if draw, and 1 if player wins.
        }

        public static int GetWinner(string[] choices, string choice1, string choice2)

        {
            if (choice1 == choice2)
                return 0;

            if (GetVictories(choices, choice1).Contains(choice2))
                return 1;

            else if (GetVictories(choices, choice2).Contains(choice1))
                return -1;

            throw new Exception("No Winner found.");
        }

        public static IEnumerable<string> GetVictories(string[] choices, string choice)
        {
            //Index of choices.
            int index = Array.IndexOf(choices, choice);

            if (index % 2 != 0 && index == choices.Length - 1)
                yield return choices[0];

            for (int i = index - 2; i >= 0; i -= 2)
                yield return choices[i];

            for (int i = index + 1; i < choices.Length; i += 2)
                yield return choices[i];
        }

        public string LastCPUChoice
        {
            private set;
            get;
        }

        public readonly string[] Choices;
        private RPSCPU _rpsCPU;

        class RPSCPU
        {

            public RPSCPU(params string[] choices)
            {
                _choices = choices;
                _choiceProbability = new Dictionary<string, int>();

                // The CPU sets the probability for each choice to be chosen as 1.
                foreach (string choice in choices)
                    _choiceProbability.Add(choice, 1);

                _random = new Random();
            }

            // Probability increased of selecting each choice that beats the provided move.
            public void AddPlayerMove(string choice)
            {

                int index = Array.IndexOf(_choices, choice);

                foreach (string winChoice in _choices.Except(GetVictories(_choices, choice)))
                    if (winChoice != choice)
                        _choiceProbability[winChoice]++;
            }

            // Get the next move of the CPU's.
            public string NextMove()
            {

                double r = _random.NextDouble();

                double divisor = _choiceProbability.Values.Sum();

                var weightedChoiceRanges = new Dictionary<double, string>();

                double currentPos = 0.0;

                // Maps probabilities to ranges between 0.0 and 1.0. Returns weighted random choice.

                foreach (var choice in _choiceProbability)
                {

                    double weightedRange = choice.Value / divisor;
                    if (r <= currentPos + (choice.Value / divisor))

                        return choice.Key;
                    currentPos += weightedRange;
                }

                throw new Exception("Unable to calculate move.");
            }

            Random _random;
            private readonly string[] _choices;
            private Dictionary<string, int> _choiceProbability;
            }
        }
    }
}