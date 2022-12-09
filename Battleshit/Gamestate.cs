namespace Battleshit
{
    public class Gamestate
    {
        public int Rows { get; }
        public int Cols { get; }
        public BoardValues[,] Board1 { get; }
        public BoardValues[,] Board2 { get; }

        private Shit[] availableShits1 { get; }
        private Shit[] availableShits2 { get; }
        private Shit[] placedShits1 { get; }
        private Shit[] placedShits2 { get; }



        public Gamestate(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Board1 = new BoardValues[Rows, Cols];
        }

        private void PlaceShit(string name, int length)
        {

        }
    }
}
