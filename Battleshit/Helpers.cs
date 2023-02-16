using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleshit
{
    internal class Helpers
    {
        // Find orientation of shit
        // True = vertical
        public static bool? FindShitDir(BoardValues[,] Board, int y, int x, BoardValues BoardValue)
        {
            // Check if any adjacent direction is the specified boardvalue
            if (x + 1 < SinglePlayer.cols && Board[y, x + 1] == BoardValue)
            {
                return false;
            }
            else if (x - 1 >= 0 && Board[y, x - 1] == BoardValue)
            {
                return false;
            }
            else if (y + 1 < SinglePlayer.rows && Board[y + 1, x] == BoardValue)
            {
                return true;
            }
            else if (y - 1 >= 0 && Board[y - 1, x] == BoardValue)
            {
                return true;
            }

            return null;
        }

        public static bool CheckWon(BoardValues[,] Board)
        {
            for (int i = 0; i < SinglePlayer.rows; i++)
            {
                for (int j = 0; j < SinglePlayer.cols; j++)
                {
                    // exist a shit
                    if (new[] { 1, 2, 3, 4, 5, 6 }.Contains((int)Board[i, j]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public static bool IsValidPos(int x, int y, int cols, int rows)
        {
            if (x < 0 || y < 0 || x > cols - 1 || y > rows - 1)
                return false;
            return true;
        }
    }
}
