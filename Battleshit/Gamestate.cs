using System;
using System.Diagnostics;
using System.Windows.Controls;

namespace Battleshit
{
    public class Gamestate
    {
        public int Rows { get; }
        public int Cols { get; }
        public BoardValues[,] Board1 { get; }
        public BoardValues[,] Board2 { get; }

        public Shit[] availableShits1 { get; } = { Shit.Carrier, Shit.Destroyer, Shit.Battleship, Shit.Cruiser, Shit.Submarine };
        public Shit[] availableShits2 { get; } = { Shit.Carrier, Shit.Destroyer, Shit.Battleship, Shit.Cruiser, Shit.Submarine };
        private Shit[] placedShits1 { get; }
        private Shit[] placedShits2 { get; }

        public bool? who_won { get; set; }
        public bool whos_turn;

        private readonly Random random = new Random();

        public Gamestate(int rows, int cols)
        {
            who_won = null;
            Rows = rows;
            Cols = cols;
            Board1 = new BoardValues[rows, cols];
            Board2 = new BoardValues[rows, cols];

            placeShits(Board1, availableShits1);
            placeShits(Board2, availableShits2);
        }

        private void placeShits(BoardValues[,] Board, Shit[] availableShits)
        {
            foreach (Shit shit in availableShits)
            {
                int rnd_x = random.Next(0, Cols);
                int rnd_y = random.Next(0, Rows);
                bool orientation = (rnd_x + rnd_y) % 2 == 0;    // false = horizontal
                // check if shit fits
                while (placeShit(Board, shit.length, orientation, rnd_x, rnd_y))
                {
                    rnd_x = random.Next(0, Cols);
                    rnd_y = random.Next(0, Rows);
                    orientation = (rnd_x + rnd_y) % 2 == 0;
                };
            }
        }

        public bool placeShit(BoardValues[,] Board, int length, bool orientation, int location_x, int location_y)
        {
            if ((location_x + length > Cols) && !orientation ) { return true; }
            if ((location_y + length > Rows) && orientation) { return true; }
            for (int i = 0; i < length; i++)
            {
                if (!orientation) // horizontal
                {
                    if (Board[location_y, location_x + i] != 0) { return true; }
                } else
                {
                    if (Board[location_y + i, location_x] != 0) { return true; }
                }
            }

            for (int i = 0; i < length; i++)
            {
                if (!orientation) // horizontal
                {
                    if (i == 0)
                    {
                        Board[location_y, location_x + i] = BoardValues.Head_x;
                    } else if (i == (length - 1))
                    {
                        Board[location_y, location_x + i] = BoardValues.Tail_x;
                    } else
                    {
                        Board[location_y, location_x + i] = BoardValues.Body_x;
                    }
                    
                }
                else
                {
                    if (i == 0)
                    {
                        Board[location_y + i, location_x] = BoardValues.Head_y;
                    }
                    else if (i == (length - 1))
                    {
                        Board[location_y + i, location_x] = BoardValues.Tail_y;
                    }
                    else
                    {
                        Board[location_y + i, location_x] = BoardValues.Body_y;
                    }
                }
            }

            return false;
        }


    }
}
