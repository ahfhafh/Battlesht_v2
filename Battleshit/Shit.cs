using System.Windows.Media;

namespace Battleshit
{
    public class Shit
    {
        public string name { get; }
        public int shit_type { get; }
        public int length { get; }
        public readonly static Shit Carrier = new Shit("Carrier", 5, 5);
        public readonly static Shit Destroyer = new Shit("Destroyer", 1, 2);
        public readonly static Shit Submarine = new Shit("Submarine", 2, 3);
        public readonly static Shit Cruiser = new Shit("Cruiser", 3, 3);
        public readonly static Shit Battleship = new Shit("Battleship", 4, 4);

        public Shit(string shit_name, int shit_type, int shit_length)
        {
            name = shit_name;
            this.shit_type = shit_type;
            length = shit_length;
        }
    }
}
