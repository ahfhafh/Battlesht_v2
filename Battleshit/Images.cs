using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Battleshit
{
    public static class Images
    {
        public readonly static ImageSource Shit_bg = LoadImage("shit_bg.png");
        public readonly static ImageSource Shit_bg_miss = LoadImage("shit_bg_miss.png");
        public readonly static ImageSource Shit_destroyed = LoadImage("shit_destroyed.png");
        public readonly static ImageSource Shit_sunk = LoadImage("shit_sunk.png");

        public readonly static ImageSource Shit_head = LoadImage("stool_head.png");
        public readonly static ImageSource Shit_body = LoadImage("stool_mid.png");
        public readonly static ImageSource Shit_tail = LoadImage("stool_tail.png");

        private static ImageSource LoadImage(string filename)
        {
            return new BitmapImage(new Uri($"Assets/{filename}", UriKind.Relative));
        }
    }
}
