using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Battleshit
{
    /// <summary>
    /// Interaction logic for SinglePlayer.xaml
    /// </summary>
    public partial class SinglePlayer : Page
    {
        private Gamestate gamestate;
        public static readonly int rows = 10, cols = 10;
        private readonly Image[,] boardImages1;
        private readonly Image[,] boardImages2;
        private BoardValues[,] hiddenBoard;

        private readonly Random random = new();

        private bool clickable = true;
        private bool gameStarted = false;

        private readonly MediaPlayer bgPlayer = new();
        private readonly MediaPlayer splashPlayer = new();

        private bool shitPickedUp = false;
        private bool pickedUpShitXorY;
        private int pickedUpShitLength;
        private Grid currentImgMouseOver;
        readonly RotateTransform rt = new(90);
        readonly RotateTransform nort = new(0);
        Point rtpoint = new(0.5, 0.5);
        private readonly SolidColorBrush Yellow = new(Color.FromArgb(80, 233, 224, 110));
        private readonly SolidColorBrush None = new(Color.FromArgb(0, 233, 224, 110));

        private readonly Rectangle[,] boardRecs1 = new Rectangle[10, 10];

        public SinglePlayer()
        {
            InitializeComponent();

            GRID.MouseEnter += Bloom;
            GRID.MouseLeave += Bloop;
            Rectangle rect = new()
            {
                Name = "RECTANGLE",
                Stroke = new SolidColorBrush(Colors.Black),
                Fill = None,
            };
            RegisterName(rect.Name, rect);
            GRID.Children.Add(rect);

            this.gamestate = new Gamestate(rows, cols);
            this.boardImages1 = SetupBoard(Board1, gamestate.Board1, true);
            this.boardImages2 = SetupBoard(Board2, gamestate.Board2, false);

            // create hidden empty board for computer
            this.hiddenBoard = new BoardValues[rows, cols];

            // label
            GameStatusLabel.Foreground = Brushes.Red;
            GameStatusLabel.Content = "Fire to start game!";

            // music
            bgPlayer.Open(new Uri("pack://siteoforigin:,,,/Assets/sound-bg.mp3"));
            bgPlayer.MediaFailed += OnMediaFailed;
            bgPlayer.MediaEnded += OnMediaEnded;
            bgPlayer.Volume = MainWindow.GameVolume;
            volumeSlider2.Value = MainWindow.GameVolume;
            mute2.IsChecked = MainWindow.GameMute;
            bgPlayer.Play();

            // sfx
            splashPlayer.Open(new Uri("pack://siteoforigin:,,,/Assets/splash.mp3"));
            splashPlayer.MediaFailed += OnMediaFailed;
            splashPlayer.MediaEnded += OnMediaEnded_Splash;
            splashPlayer.Volume = MainWindow.GameVolume + 0.2;

            // rightclick mouse event
            MouseRightButtonDown += ChangePickedUpShitOri;
        }

        private void Bloop(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;

            ((Rectangle)grid.FindName("RECTANGLE")).Fill = None;
        }

        private void Bloom(object sender, MouseEventArgs e)
        {
            Grid grid = sender as Grid;

            ((Rectangle)grid.FindName("RECTANGLE")).Fill = Yellow;
        }

        private void ChangePickedUpShitOri(object sender, EventArgs e)
        {
            if (shitPickedUp && !gameStarted)
            {
                HandleUnhighlight(currentImgMouseOver, null);
                pickedUpShitXorY = !pickedUpShitXorY;
                HandleHighlight(currentImgMouseOver, null);
            }
        }

        public Image[,] SetupBoard(UniformGrid Board, BoardValues[,] boardValues, bool isPlayerBoard)
        {
            Image[,] images = new Image[rows, cols];
            Board.Rows = rows;
            Board.Columns = cols;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Grid cell = new();
                    Image img = new();

                    if (!isPlayerBoard) // allow click on enemy board
                    {
                        cell.MouseEnter += HighlightElement;
                        cell.MouseLeave += UnhighlightElement;
                        cell.MouseDown += PlayerTurnClick;
                    }
                    else
                    {
                        Grid.SetZIndex(img, -1);
                        Rectangle rect = new()
                        {
                            Fill = None,
                        };
                        boardRecs1[y, x] = rect;
                        cell.Children.Add(rect);

                        cell.MouseEnter += HandleHighlight;
                        cell.MouseLeave += HandleUnhighlight;
                        cell.MouseLeftButtonDown += HandleShitPickupNDrop;
                    }

                    switch (boardValues[y, x])
                    {
                        case BoardValues.Empty:
                            img.Source = Images.Shit_bg;
                            images[y, x] = img;
                            cell.Children.Add(img);
                            Board.Children.Add(cell);
                            break;
                        case BoardValues.Head_x:
                            if (isPlayerBoard) { img.Source = Images.Shit_head; }
                            else { img.Source = Images.Shit_bg; }
                            images[y, x] = img;
                            cell.Children.Add(img);
                            Board.Children.Add(cell);
                            break;
                        case BoardValues.Head_y:
                            if (isPlayerBoard) { img.Source = Images.Shit_head; }
                            else { img.Source = Images.Shit_bg; }
                            cell.Children.Add(img);
                            img.RenderTransformOrigin = rtpoint;
                            images[y, x] = img;
                            img.RenderTransform = rt;
                            Board.Children.Add(cell);
                            break;
                        case BoardValues.Body_x:
                            if (isPlayerBoard) { img.Source = Images.Shit_body; }
                            else { img.Source = Images.Shit_bg; }
                            images[y, x] = img;
                            cell.Children.Add(img);
                            Board.Children.Add(cell);
                            break;
                        case BoardValues.Body_y:
                            if (isPlayerBoard) { img.Source = Images.Shit_body; }
                            else { img.Source = Images.Shit_bg; }
                            cell.Children.Add(img);
                            img.RenderTransformOrigin = rtpoint;
                            images[y, x] = img;
                            img.RenderTransform = rt;
                            Board.Children.Add(cell);
                            break;
                        case BoardValues.Tail_x:
                            if (isPlayerBoard) { img.Source = Images.Shit_tail; }
                            else { img.Source = Images.Shit_bg; }
                            images[y, x] = img;
                            cell.Children.Add(img);
                            Board.Children.Add(cell);
                            break;
                        case BoardValues.Tail_y:
                            if (isPlayerBoard) { img.Source = Images.Shit_tail; }
                            else { img.Source = Images.Shit_bg; }
                            cell.Children.Add(img);
                            img.RenderTransformOrigin = rtpoint;
                            images[y, x] = img;
                            img.RenderTransform = rt;
                            Board.Children.Add(cell);
                            break;
                        default: break;
                    }
                }
            }

            return images;
        }

        private void HandleShitPickupNDrop(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            Debug.WriteLine("HandlePickupNDrop, picked up" + shitPickedUp);
            if (!shitPickedUp && !gameStarted) // --------------------------------------------- Pickup Shit ------------------------------
            {
                Debug.WriteLine("Shit picked up");
                pickedUpShitLength = 0;
                // remove shit
                Grid cell = sender as Grid;
                // get index of img in board
                UniformGrid parent = (UniformGrid)cell.Parent;
                int index = parent.Children.IndexOf(cell);
                int y = index / 10;
                int x = index % 10;

                BoardValues imgtype = gamestate.Board1[y, x];
                switch (imgtype)
                {
                    case BoardValues.Head_x:
                        while (gamestate.Board1[y, x] != BoardValues.Tail_x)
                        {
                            cell.Cursor = Cursors.Arrow;
                            boardRecs1[y, x].Fill = None;
                            boardImages1[y, x].Source = Images.Shit_bg;
                            gamestate.Board1[y, x] = BoardValues.Empty;
                            pickedUpShitLength++;
                            x++;
                        }
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        boardImages1[y, x].Source = Images.Shit_bg;
                        gamestate.Board1[y, x] = BoardValues.Empty;
                        pickedUpShitLength++;
                        pickedUpShitXorY = true;
                        shitPickedUp = true;
                        break;
                    case BoardValues.Head_y:
                        while (gamestate.Board1[y, x] != BoardValues.Tail_y)
                        {
                            boardRecs1[y, x].Fill = None;
                            cell.Cursor = Cursors.Arrow;
                            boardImages1[y, x].Source = Images.Shit_bg;
                            boardImages1[y, x].RenderTransform = nort;
                            gamestate.Board1[y, x] = BoardValues.Empty;
                            pickedUpShitLength++;
                            y++;
                        }
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        boardImages1[y, x].Source = Images.Shit_bg;
                        boardImages1[y, x].RenderTransform = nort;
                        gamestate.Board1[y, x] = BoardValues.Empty;
                        pickedUpShitLength++;
                        pickedUpShitXorY = false;
                        shitPickedUp = true;
                        break;
                    case BoardValues.Body_x:
                        while (gamestate.Board1[y, x] != BoardValues.Head_x) { x--; }
                        while (gamestate.Board1[y, x] != BoardValues.Tail_x)
                        {
                            boardRecs1[y, x].Fill = None;
                            cell.Cursor = Cursors.Arrow;
                            boardImages1[y, x].Source = Images.Shit_bg;
                            gamestate.Board1[y, x] = BoardValues.Empty;
                            pickedUpShitLength++;
                            x++;
                        }
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        boardImages1[y, x].Source = Images.Shit_bg;
                        gamestate.Board1[y, x] = BoardValues.Empty;
                        pickedUpShitLength++;
                        pickedUpShitXorY = true;
                        shitPickedUp = true;
                        break;
                    case BoardValues.Body_y:
                        while (gamestate.Board1[y, x] != BoardValues.Head_y) { y--; }
                        while (gamestate.Board1[y, x] != BoardValues.Tail_y)
                        {
                            boardRecs1[y, x].Fill = None;
                            cell.Cursor = Cursors.Arrow;
                            boardImages1[y, x].Source = Images.Shit_bg;
                            boardImages1[y, x].RenderTransform = nort;
                            gamestate.Board1[y, x] = BoardValues.Empty;
                            pickedUpShitLength++;
                            y++;
                        }
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        boardImages1[y, x].Source = Images.Shit_bg;
                        boardImages1[y, x].RenderTransform = nort;
                        gamestate.Board1[y, x] = BoardValues.Empty;
                        pickedUpShitLength++;
                        pickedUpShitXorY = false;
                        shitPickedUp = true;
                        break;
                    case BoardValues.Tail_x:
                        while (gamestate.Board1[y, x] != BoardValues.Head_x)
                        {

                            boardRecs1[y, x].Fill = None;
                            cell.Cursor = Cursors.Arrow;
                            boardImages1[y, x].Source = Images.Shit_bg;
                            gamestate.Board1[y, x] = BoardValues.Empty;
                            pickedUpShitLength++;
                            x--;
                        }
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        boardImages1[y, x].Source = Images.Shit_bg;
                        gamestate.Board1[y, x] = BoardValues.Empty;
                        pickedUpShitLength++;
                        pickedUpShitXorY = true;
                        shitPickedUp = true;
                        break;
                    case BoardValues.Tail_y:
                        while (gamestate.Board1[y, x] != BoardValues.Head_y)
                        {
                            boardRecs1[y, x].Fill = None;
                            cell.Cursor = Cursors.Arrow;
                            boardImages1[y, x].Source = Images.Shit_bg;
                            boardImages1[y, x].RenderTransform = nort;
                            gamestate.Board1[y, x] = BoardValues.Empty;
                            pickedUpShitLength++;
                            y--;
                        }
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        boardImages1[y, x].Source = Images.Shit_bg;
                        boardImages1[y, x].RenderTransform = nort;
                        gamestate.Board1[y, x] = BoardValues.Empty;
                        pickedUpShitLength++;
                        pickedUpShitXorY = false;
                        shitPickedUp = true;
                        break;
                    default:
                        break;
                }

                return;
            }
            else if (shitPickedUp) // --------------------------------------------- Drop Shit ------------------------------
            {
                Debug.WriteLine("Shit dropped");
                UniformGrid parent = (UniformGrid)currentImgMouseOver.Parent;
                int index = parent.Children.IndexOf(currentImgMouseOver);
                int y = index / 10;
                int x = index % 10;

                if (pickedUpShitXorY) // X
                {
                    // check for drop space validity
                    if (!CheckValidDropSpaceX(x, y)) { return; }
                    // replace image and gamestate board values
                    for (int z = 0; z < pickedUpShitLength; z++)
                    {
                        if (z == 0)
                        {
                            gamestate.Board1[y, x + z] = BoardValues.Head_x;
                            boardImages1[y, x + z].Source = Images.Shit_head;
                            boardImages1[y, x + z].MouseEnter += HandleHighlight;
                            boardImages1[y, x + z].MouseLeave += HandleUnhighlight;
                            boardImages1[y, x + z].MouseLeftButtonDown += HandleShitPickupNDrop;
                        }
                        else if (z == pickedUpShitLength - 1)
                        {
                            gamestate.Board1[y, x + z] = BoardValues.Tail_x;
                            boardImages1[y, x + z].Source = Images.Shit_tail;
                            boardImages1[y, x + z].MouseEnter += HandleHighlight;
                            boardImages1[y, x + z].MouseLeave += HandleUnhighlight;
                            boardImages1[y, x + z].MouseLeftButtonDown += HandleShitPickupNDrop;
                        }
                        else
                        {
                            gamestate.Board1[y, x + z] = BoardValues.Body_x;
                            boardImages1[y, x + z].Source = Images.Shit_body;
                            boardImages1[y, x + z].MouseEnter += HandleHighlight;
                            boardImages1[y, x + z].MouseLeave += HandleUnhighlight;
                            boardImages1[y, x + z].MouseLeftButtonDown += HandleShitPickupNDrop;
                        }

                        /*                        boardImages1[y, x + z].Cursor = Cursors.Hand;*/
                    }

                }
                else // Y
                {
                    // check for drop space validity
                    if (!CheckValidDropSpaceY(x, y)) { return; }
                    // replace image and gamestate board values
                    for (int z = 0; z < pickedUpShitLength; z++)
                    {
                        if (z == 0)
                        {
                            gamestate.Board1[y + z, x] = BoardValues.Head_y;
                            boardImages1[y + z, x].Source = Images.Shit_head;
                            boardImages1[y + z, x].MouseEnter += HandleHighlight;
                            boardImages1[y + z, x].MouseLeave += HandleUnhighlight;
                            boardImages1[y + z, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            boardImages1[y + z, x].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages1[y + z, x].RenderTransform = rt;
                        }
                        else if (z == pickedUpShitLength - 1)
                        {
                            gamestate.Board1[y + z, x] = BoardValues.Tail_y;
                            boardImages1[y + z, x].Source = Images.Shit_tail;
                            boardImages1[y + z, x].MouseEnter += HandleHighlight;
                            boardImages1[y + z, x].MouseLeave += HandleUnhighlight;
                            boardImages1[y + z, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            boardImages1[y + z, x].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages1[y + z, x].RenderTransform = rt;
                        }
                        else
                        {
                            gamestate.Board1[y + z, x] = BoardValues.Body_y;
                            boardImages1[y + z, x].Source = Images.Shit_body;
                            boardImages1[y + z, x].MouseEnter += HandleHighlight;
                            boardImages1[y + z, x].MouseLeave += HandleUnhighlight;
                            boardImages1[y + z, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            boardImages1[y + z, x].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages1[y + z, x].RenderTransform = rt;
                        }

                        /*                        boardImages1[y + z, x].Cursor = Cursors.Hand;*/
                    }
                }
                shitPickedUp = false;

                Print2DArray(gamestate.Board1);
                return;
            }
        }

        private bool CheckValidDropSpaceY(int x, int y)
        {
            if (y + pickedUpShitLength > rows) { return false; }
            for (int z = 0; z < pickedUpShitLength; z++)
            {
                if (gamestate.Board1[y + z, x] != BoardValues.Empty)
                {
                    return false;
                }
                if (Helpers.IsValidPos(x - 1, y + z, cols, rows))
                {
                    if (gamestate.Board1[y + z, x - 1] != BoardValues.Empty) { return false; }    // left
                }
                if (Helpers.IsValidPos(x, y - 1 + z, cols, rows))
                {
                    if (gamestate.Board1[y - 1 + z, x] != BoardValues.Empty) { return false; }    // top
                }
                if (Helpers.IsValidPos(x, y + 1 + z, cols, rows))
                {
                    if (gamestate.Board1[y + 1 + z, x] != BoardValues.Empty) { return false; }    // bottom
                }
                if (Helpers.IsValidPos(x + 1, y + z, cols, rows))
                {
                    if (gamestate.Board1[y + z, x + 1] != BoardValues.Empty) { return false; }    // right
                }
            }

            return true;
        }

        private bool CheckValidDropSpaceX(int x, int y)
        {
            if (x + pickedUpShitLength > cols) { return false; }
            for (int z = 0; z < pickedUpShitLength; z++)
            {
                if (gamestate.Board1[y, x + z] != BoardValues.Empty)
                {
                    return false;
                }
                if (Helpers.IsValidPos(x - 1 + z, y, cols, rows))
                {
                    if (gamestate.Board1[y, x - 1 + z] != BoardValues.Empty) { return false; }    // behind
                }
                if (Helpers.IsValidPos(x + z, y - 1, cols, rows))
                {
                    if (gamestate.Board1[y - 1, x + z] != BoardValues.Empty) { return false; }    // top
                }
                if (Helpers.IsValidPos(x + z, y + 1, cols, rows))
                {
                    if (gamestate.Board1[y + 1, x + z] != BoardValues.Empty) { return false; }    // bottom
                }
                if (Helpers.IsValidPos(x + 1 + z, y, cols, rows))
                {
                    if (gamestate.Board1[y, x + 1 + z] != BoardValues.Empty) { return false; }    // in front
                }
            }
            return true;
        }

        private void HandleUnhighlight(object sender, MouseEventArgs e)
        {
            Grid cell = sender as Grid;
            // get index of img in board
            UniformGrid parent = (UniformGrid)cell.Parent;
            int index = parent.Children.IndexOf(cell);
            int y = index / 10;
            int x = index % 10;

            BoardValues imgtype = gamestate.Board1[y, x];
            switch (imgtype)
            {
                case BoardValues.Head_x:
                    while (gamestate.Board1[y, x] != BoardValues.Tail_x)
                    {
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        x++;
                    }
                    boardRecs1[y, x].Fill = None;
                    cell.Cursor = Cursors.Arrow;
                    break;
                case BoardValues.Head_y:
                    while (gamestate.Board1[y, x] != BoardValues.Tail_y)
                    {
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        y++;
                    }
                    boardRecs1[y, x].Fill = None;
                    cell.Cursor = Cursors.Arrow;
                    break;
                case BoardValues.Body_x:
                    while (gamestate.Board1[y, x] != BoardValues.Head_x)
                    {
                        x--;
                    }
                    while (gamestate.Board1[y, x] != BoardValues.Tail_x)
                    {
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        x++;
                    }
                    boardRecs1[y, x].Fill = None;
                    cell.Cursor = Cursors.Arrow;
                    break;
                case BoardValues.Body_y:
                    while (gamestate.Board1[y, x] != BoardValues.Head_y)
                    {
                        y--;
                    }
                    while (gamestate.Board1[y, x] != BoardValues.Tail_y)
                    {
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        y++;
                    }
                    boardRecs1[y, x].Fill = None;
                    cell.Cursor = Cursors.Arrow;
                    break;
                case BoardValues.Tail_x:
                    while (gamestate.Board1[y, x] != BoardValues.Head_x)
                    {
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        x--;
                    }
                    boardRecs1[y, x].Fill = None;
                    cell.Cursor = Cursors.Arrow;
                    break;
                case BoardValues.Tail_y:
                    while (gamestate.Board1[y, x] != BoardValues.Head_y)
                    {
                        boardRecs1[y, x].Fill = None;
                        cell.Cursor = Cursors.Arrow;
                        y--;
                    }
                    boardRecs1[y, x].Fill = None;
                    cell.Cursor = Cursors.Arrow;
                    break;
                default:
                    break;
            }

            if (shitPickedUp)
            {
                // unhighlight
                if (pickedUpShitXorY) // X
                {
                    if (x + pickedUpShitLength > cols)
                    {
                        return;
                    }
                    for (int z = 0; z < pickedUpShitLength; z++)
                    {
                        boardRecs1[y, x + z].Fill = None;
                        boardImages1[y, x + z].Cursor = Cursors.Arrow;
                    }

                }
                else // Y
                {
                    if (y + pickedUpShitLength > rows)
                    {
                        return;
                    }
                    for (int z = 0; z < pickedUpShitLength; z++)
                    {
                        boardRecs1[y + z, x].Fill = None;
                        boardImages1[y + z, x].Cursor = Cursors.Arrow;
                    }
                }
            }

        }

        private void HandleHighlight(object sender, MouseEventArgs e)
        {
            if (!shitPickedUp && !gameStarted) // highlight shit
            {
                Grid cell = sender as Grid;
                // get index of img in board
                UniformGrid parent = (UniformGrid)cell.Parent;
                int index = parent.Children.IndexOf(cell);
                int y = index / 10;
                int x = index % 10; 
                BoardValues imgtype = gamestate.Board1[y, x];
                switch (imgtype)
                {
                    case BoardValues.Head_x:
                        while (gamestate.Board1[y, x] != BoardValues.Tail_x)
                        {
                            boardRecs1[y, x].Fill = Yellow;
                            cell.Cursor = Cursors.Hand;
                            x++;
                        }
                        boardRecs1[y, x].Fill = Yellow;
                        cell.Cursor = Cursors.Hand;
                        break;
                    case BoardValues.Head_y:
                        while (gamestate.Board1[y, x] != BoardValues.Tail_y)
                        {
                            boardRecs1[y, x].Fill = Yellow;
                            cell.Cursor = Cursors.Hand;
                            y++;
                        }
                        boardRecs1[y, x].Fill = Yellow;
                        cell.Cursor = Cursors.Hand;
                        break;
                    case BoardValues.Body_x:
                        while (gamestate.Board1[y, x] != BoardValues.Head_x)
                        {
                            x--;
                        }
                        while (gamestate.Board1[y, x] != BoardValues.Tail_x)
                        {
                            boardRecs1[y, x].Fill = Yellow;
                            cell.Cursor = Cursors.Hand;
                            x++;
                        }
                        boardRecs1[y, x].Fill = Yellow;
                        cell.Cursor = Cursors.Hand;
                        break;
                    case BoardValues.Body_y:
                        while (gamestate.Board1[y, x] != BoardValues.Head_y)
                        {
                            y--;
                        }
                        while (gamestate.Board1[y, x] != BoardValues.Tail_y)
                        {
                            boardRecs1[y, x].Fill = Yellow;
                            cell.Cursor = Cursors.Hand;
                            y++;
                        }
                        boardRecs1[y, x].Fill = Yellow;
                        cell.Cursor = Cursors.Hand;
                        break;
                    case BoardValues.Tail_x:
                        while (gamestate.Board1[y, x] != BoardValues.Head_x)
                        {
                            boardRecs1[y, x].Fill = Yellow;
                            cell.Cursor = Cursors.Hand;
                            x--;
                        }
                        boardRecs1[y, x].Fill = Yellow;
                        cell.Cursor = Cursors.Hand;
                        break;
                    case BoardValues.Tail_y:
                        while (gamestate.Board1[y, x] != BoardValues.Head_y)
                        {
                            boardRecs1[y, x].Fill = Yellow;
                            cell.Cursor = Cursors.Hand;
                            y--;
                        }
                        boardRecs1[y, x].Fill = Yellow;
                        cell.Cursor = Cursors.Hand;
                        break;
                    default:
                        break;
                }
            }
            else if (shitPickedUp && !gameStarted)  // highlight shit_bg
            {
                Grid cell = currentImgMouseOver = sender as Grid;

                // get index of img in board
                UniformGrid parent = (UniformGrid)cell.Parent;
                int index = parent.Children.IndexOf(cell);
                int y = index / 10;
                int x = index % 10;

                // highlight if possible to drop
                if (pickedUpShitXorY) // X
                {
                    // check for drop space validity
                    if (!CheckValidDropSpaceX(x, y)) { return; }
                    // show effect on drop space
                    for (int z = 0; z < pickedUpShitLength; z++)
                    {
                        boardRecs1[y, x + z].Fill = Yellow;
                        boardImages1[y, x + z].Cursor = Cursors.Hand;
                    }

                }
                else // Y
                {
                    // check for drop space validity
                    if (!CheckValidDropSpaceY(x, y)) { return; }
                    // show effect on drop space
                    for (int z = 0; z < pickedUpShitLength; z++)
                    {
                        boardRecs1[y + z, x].Fill = Yellow;
                        boardImages1[y + z, x].Cursor = Cursors.Hand;
                    }
                }
            }
        }

        private void DrawBoard(Image[,] images, BoardValues[,] boardValues, bool isPlayerBoard)
        {
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    images[y, x].Opacity = 1;
                    boardRecs1[y, x].Fill = None;
                    switch (boardValues[y, x])
                    {
                        case BoardValues.Empty:
                            images[y, x].Source = Images.Shit_bg;
                            break;
                        case BoardValues.Head_x:
                            if (isPlayerBoard)
                            {
                                images[y, x].Source = Images.Shit_head;
                                images[y, x].MouseEnter += HandleHighlight;
                                images[y, x].MouseLeave += HandleUnhighlight;
                                images[y, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            }
                            else { images[y, x].Source = Images.Shit_bg; }
                            images[y, x].RenderTransform = nort;
                            break;
                        case BoardValues.Head_y:
                            if (isPlayerBoard)
                            {
                                images[y, x].Source = Images.Shit_head;
                                images[y, x].MouseEnter += HandleHighlight;
                                images[y, x].MouseLeave += HandleUnhighlight;
                                images[y, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            }
                            else { images[y, x].Source = Images.Shit_bg; }
                            images[y, x].RenderTransformOrigin = rtpoint;
                            images[y, x].RenderTransform = rt;
                            break;
                        case BoardValues.Body_x:
                            if (isPlayerBoard)
                            {
                                images[y, x].Source = Images.Shit_body;
                                images[y, x].MouseEnter += HandleHighlight;
                                images[y, x].MouseLeave += HandleUnhighlight;
                                images[y, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            }
                            else { images[y, x].Source = Images.Shit_bg; }
                            images[y, x].RenderTransform = nort;
                            break;
                        case BoardValues.Body_y:
                            if (isPlayerBoard)
                            {
                                images[y, x].Source = Images.Shit_body;
                                images[y, x].MouseEnter += HandleHighlight;
                                images[y, x].MouseLeave += HandleUnhighlight;
                                images[y, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            }
                            else { images[y, x].Source = Images.Shit_bg; }
                            images[y, x].RenderTransformOrigin = rtpoint;
                            images[y, x].RenderTransform = rt;
                            break;
                        case BoardValues.Tail_x:
                            if (isPlayerBoard)
                            {
                                images[y, x].Source = Images.Shit_tail;
                                images[y, x].MouseEnter += HandleHighlight;
                                images[y, x].MouseLeave += HandleUnhighlight;
                                images[y, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            }
                            else { images[y, x].Source = Images.Shit_bg; }
                            images[y, x].RenderTransform = nort;
                            break;
                        case BoardValues.Tail_y:
                            if (isPlayerBoard)
                            {
                                images[y, x].Source = Images.Shit_tail;
                                images[y, x].MouseEnter += HandleHighlight;
                                images[y, x].MouseLeave += HandleUnhighlight;
                                images[y, x].MouseLeftButtonDown += HandleShitPickupNDrop;
                            }
                            else { images[y, x].Source = Images.Shit_bg; }
                            images[y, x].RenderTransformOrigin = rtpoint;
                            images[y, x].RenderTransform = rt;
                            break;
                        case BoardValues.Destroyed:
                            images[y, x].Source = Images.Shit_destroyed;
                            break;
                        case BoardValues.Miss:
                            images[y, x].Source = Images.Shit_bg_miss;
                            break;
                        case BoardValues.Sunk:
                            images[y, x].Source = Images.Shit_sunk;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Debug.Write(matrix[i, j] + "\t");
                }
                Debug.WriteLine("|");
            }

            Debug.WriteLine("_________________");
        }

        // Set opacity of half when mouse hover
        private void HighlightElement(object sender, MouseEventArgs e)
        {
            Grid cell = sender as Grid;
            cell.Opacity = 0.2;

        }

        // Reset opacity when mouse not hover
        private void UnhighlightElement(object sender, MouseEventArgs e)
        {
            Grid cell = sender as Grid;
            cell.Opacity = 1;
        }

        // Process a turn when player clicks
        private async void PlayerTurnClick(object sender, RoutedEventArgs e)
        {
            // Disable click
            if (!this.clickable || shitPickedUp)
            {
                return;
            }

            // Disable random btn
            if (!gameStarted)
            {
                gameStarted = true;
                this.RandomBtn.IsEnabled = false;
            }

            this.clickable = false;

            Grid img = sender as Grid;
            UniformGrid parent = (UniformGrid)img.Parent;

            // get index of img in board
            int index = parent.Children.IndexOf(img);
            int y = index / 10;
            int x = index % 10;

            // check if shit tried already
            if ((this.gamestate.Board2[y, x] == BoardValues.Destroyed) || (this.gamestate.Board2[y, x] == BoardValues.Miss) || (this.gamestate.Board2[y, x] == BoardValues.Sunk))
            {
                this.clickable = true;
                return;
            }
            // Check if is shit
            if (this.gamestate.Board2[y, x] != BoardValues.Empty)
            {
                // Mark as clicked
                ApplySunk(this.gamestate.Board2, y, x);
                // Check if entire shit has sunk
                DrawBoard(this.boardImages2, this.gamestate.Board2, false);
            }
            else
            {
                // Mark as clicked
                this.gamestate.Board2[y, x] = BoardValues.Miss;
                DrawBoard(this.boardImages2, this.gamestate.Board2, false);
            }

            // Check if player won
            if (Helpers.CheckWon(this.gamestate.Board2))
            {
                GameStatusLabel.Content = "You won!";
                Overlay.Visibility = Visibility.Visible;
                return;
            }

            // Do computer turn
            Board1Overlay.Opacity = 1;
            Board2Overlay.Opacity = 0.7;
            GameStatusLabel.Foreground = Brushes.White;
            GameStatusLabel.Content = "Wait for computer...";
            await Task.Delay(random.Next(500, 2000));
            int rnd_x = random.Next(0, cols);
            int rnd_y = random.Next(0, rows);

            // Find optimal next position to check if exists
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // Check if shit at position is destroyed and not sunken
                    if (this.hiddenBoard[i, j] == BoardValues.Destroyed)
                    {
                        var dir = Helpers.FindShitDir(this.hiddenBoard, i, j, BoardValues.Destroyed);
                        if (dir != null)
                        {
                            int c_y = 0;
                            int c_x = 0;
                            if ((bool)dir)  // vertical
                            {
                                // find next point to choose along vertical
                                // up
                                while (i + c_y >= 0 && this.hiddenBoard[i + c_y, j] != BoardValues.Empty && this.hiddenBoard[i + c_y, j] == BoardValues.Destroyed)
                                {
                                    c_y--;
                                }
                                if (i + c_y < 0 || this.hiddenBoard[i + c_y, j] != BoardValues.Empty)
                                {
                                    c_y++;
                                    // down
                                    while (i + c_y < rows && this.hiddenBoard[i + c_y, j] != BoardValues.Empty && this.hiddenBoard[i + c_y, j] == BoardValues.Destroyed)
                                    {
                                        c_y++;
                                    }
                                }
                            }
                            else  // horizontal
                            {
                                // find next point to choose along horizontal
                                // left
                                while (j + c_x >= 0 && this.hiddenBoard[i, j + c_x] != BoardValues.Empty && this.hiddenBoard[i, j + c_x] == BoardValues.Destroyed)
                                {
                                    c_x--;
                                }
                                if (j + c_x < 0 || this.hiddenBoard[i, j + c_x] != BoardValues.Empty)
                                {
                                    c_x++;
                                    // right
                                    while (j + c_x < cols && this.hiddenBoard[i, j + c_x] != BoardValues.Empty && this.hiddenBoard[i, j + c_x] == BoardValues.Destroyed)
                                    {
                                        c_x++;
                                    }
                                }
                            }

                            rnd_x = j + c_x;
                            rnd_y = i + c_y;
                            goto cont;
                        }


                        // If no direction pick a direction thats empty
                        if (j + 1 < cols && this.hiddenBoard[i, j + 1] == BoardValues.Empty)
                        {
                            rnd_x = j + 1;
                            rnd_y = i;
                        }
                        else if (j - 1 >= 0 && this.hiddenBoard[i, j - 1] == BoardValues.Empty)
                        {
                            rnd_x = j - 1;
                            rnd_y = i;
                        }
                        else if (i + 1 < rows && this.hiddenBoard[i + 1, j] == BoardValues.Empty)
                        {
                            rnd_x = j;
                            rnd_y = i + 1;
                        }
                        else if (i - 1 >= 0 && this.hiddenBoard[i - 1, j] == BoardValues.Empty)
                        {
                            rnd_x = j;
                            rnd_y = i - 1;
                        }
                        goto cont;
                    }
                }
            }

            // If a destroyed position can't be found
            // Pick a spot thats most likely to have a shit
            // Search by row
            int longest_empty = 0;
            List<Point> potential_spots = new List<Point>();
            for (int i = 0; i < rows; i++)
            {
                int c = 0;
                for (int j = 0; j < cols - 1; j++)
                {
                    while ((j + c + 1 < cols) && (this.hiddenBoard[i, j] == BoardValues.Empty) && this.hiddenBoard[i, j + c + 1] == BoardValues.Empty &&
                        (Helpers.FindShitDir(this.hiddenBoard, i, j + c + 1, BoardValues.Destroyed) == null) && (Helpers.FindShitDir(this.hiddenBoard, i, j + c + 1, BoardValues.Sunk) == null))
                    {
                        c++;
                    }
                    if (c > longest_empty) { longest_empty = c; potential_spots.Clear(); potential_spots.Add(new(i, j)); potential_spots.Add(new(i, j + c)); }
                    else if (c == longest_empty) { potential_spots.Add(new(i, j)); potential_spots.Add(new(i, j + c)); }
                    j += c;
                    c = 0;
                }
            }
            // Search by column
            for (int j = 0; j < cols; j++)
            {
                int c = 0;
                for (int i = 0; i < rows - 1; i++)
                {
                    while ((i + c + 1 < rows) && (this.hiddenBoard[i, j] == BoardValues.Empty) && this.hiddenBoard[i + c + 1, j] == BoardValues.Empty &&
                        (Helpers.FindShitDir(this.hiddenBoard, i + c + 1, j, BoardValues.Destroyed) == null) && (Helpers.FindShitDir(this.hiddenBoard, i + c + 1, j, BoardValues.Sunk) == null))
                    {
                        c++;
                    }
                    if (c > longest_empty) { longest_empty = c; potential_spots.Clear(); potential_spots.Add(new(i, j)); potential_spots.Add(new(i + c, j)); }
                    else if (c == longest_empty) { potential_spots.Add(new(i, j)); potential_spots.Add(new(i + c, j)); }
                    i += c;
                    c = 0;
                }
            }

            // Debugging purposes
            /*foreach (var z in potential_spots)
            {
                Debug.Write(z + " ");
            }

            Debug.WriteLine("");*/

            // Select random interval
            int indx = random.Next(0, potential_spots.Count / 2);
            Point interval_start = potential_spots[indx * 2];
            Point interval_end = potential_spots[(indx * 2) + 1];

            rnd_y = random.Next((int)interval_start.X, (int)interval_end.X + 1);
            rnd_x = random.Next((int)interval_start.Y, (int)interval_end.Y + 1);

        cont:
            // Check if shit tried already
            while ((this.hiddenBoard[rnd_y, rnd_x] == BoardValues.Destroyed) || (this.hiddenBoard[rnd_y, rnd_x] == BoardValues.Miss) || (this.hiddenBoard[rnd_y, rnd_x] == BoardValues.Sunk))
            {
                rnd_x = random.Next(0, cols);
                rnd_y = random.Next(0, rows);
            }
            // check if its shit
            if (this.gamestate.Board1[rnd_y, rnd_x] != BoardValues.Empty)
            {
                // Mark as clicked
                ApplySunk(this.gamestate.Board1, rnd_y, rnd_x);
                this.hiddenBoard[rnd_y, rnd_x] = BoardValues.Destroyed;
                // copy sunks to hidden
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        if (this.gamestate.Board1[i, j] == BoardValues.Sunk)
                        {
                            this.hiddenBoard[i, j] = BoardValues.Sunk;
                        }
                    }
                }
                DrawBoard(this.boardImages1, this.gamestate.Board1, true);
            }
            else
            {
                // Mark as clicked
                this.gamestate.Board1[rnd_y, rnd_x] = BoardValues.Miss;
                this.hiddenBoard[rnd_y, rnd_x] = BoardValues.Miss;
                DrawBoard(this.boardImages1, this.gamestate.Board1, true);
            }

            // Check if computer won
            if (Helpers.CheckWon(this.gamestate.Board1))
            {
                GameStatusLabel.Foreground = Brushes.White;
                GameStatusLabel.Content = "You lost!";
                Overlay.Visibility = Visibility.Visible;
                return;
            }

            // Enable Click
            this.clickable = true;
            GameStatusLabel.Foreground = Brushes.Red;
            Board1Overlay.Opacity = 0.7;
            Board2Overlay.Opacity = 1;
            GameStatusLabel.Content = "Your turn";

        }

        private void ApplySunk(BoardValues[,] Board, int y, int x)
        {
            splashPlayer.Play();
            int i = 1;
            int c = 1;
            switch (Board[y, x])
            {
                case BoardValues.Head_x:
                    Board[y, x] = BoardValues.Destroyed;
                    while ((x + i) < cols)
                    {
                        if (Board[y, x + i] == BoardValues.Empty || Board[y, x + i] == BoardValues.Miss)
                        {
                            break;
                        }
                        else if (Board[y, x + i] != BoardValues.Destroyed)
                        {
                            return;
                        }
                        i++;
                    }
                    for (int j = 0; j < i; j++)
                    {
                        Board[y, x + j] = BoardValues.Sunk;
                    }
                    break;
                case BoardValues.Head_y:
                    Board[y, x] = BoardValues.Destroyed;
                    while ((y + i) < rows)
                    {
                        if (Board[y + i, x] == BoardValues.Empty || Board[y + i, x] == BoardValues.Miss)
                        {
                            break;
                        }
                        else if (Board[y + i, x] != BoardValues.Destroyed)
                        {
                            return;
                        }
                        i++;
                    }
                    for (int j = 0; j < i; j++)
                    {
                        Board[y + j, x] = BoardValues.Sunk;
                    }
                    break;
                case BoardValues.Body_x:
                    Board[y, x] = BoardValues.Destroyed;
                    while ((x + i) < cols)
                    {
                        if (Board[y, x + i] == BoardValues.Empty || Board[y, x + i] == BoardValues.Miss)
                        {
                            break;
                        }
                        else if (Board[y, x + i] != BoardValues.Destroyed)
                        {
                            return;
                        }
                        i++;
                    }
                    i--;
                    while ((x + i) - c >= 0)
                    {
                        if (Board[y, (x + i) - c] == BoardValues.Empty || Board[y, (x + i) - c] == BoardValues.Miss)
                        {
                            break;
                        }
                        else if (Board[y, (x + i) - c] != BoardValues.Destroyed)
                        {
                            return;
                        }
                        c++;
                    }
                    for (int j = 0; j < c; j++)
                    {
                        Board[y, x + i - j] = BoardValues.Sunk;
                    }
                    break;
                case BoardValues.Body_y:
                    Board[y, x] = BoardValues.Destroyed;
                    while ((y + i) < rows)
                    {
                        if (Board[y + i, x] == BoardValues.Empty || Board[y + i, x] == BoardValues.Miss)
                        {
                            break;
                        }
                        else if (Board[y + i, x] != BoardValues.Destroyed)
                        {
                            return;
                        }
                        i++;
                    }
                    i--;
                    while (((y + i) - c) >= 0)
                    {
                        if (Board[(y + i) - c, x] == BoardValues.Empty || Board[(y + i) - c, x] == BoardValues.Miss)
                        {
                            break;
                        }
                        else if (Board[(y + i) - c, x] != BoardValues.Destroyed)
                        {
                            return;
                        }
                        c++;
                    }
                    for (int j = 0; j < c; j++)
                    {
                        Board[y + i - j, x] = BoardValues.Sunk;
                    }
                    break;
                case BoardValues.Tail_x:
                    Board[y, x] = BoardValues.Destroyed;
                    while ((x - i) >= 0)
                    {
                        if (Board[y, x - i] == BoardValues.Empty || Board[y, x - i] == BoardValues.Miss)
                        {
                            break;
                        }
                        else if (Board[y, x - i] != BoardValues.Destroyed)
                        {
                            return;
                        }
                        i++;
                    }
                    for (int j = 0; j < i; j++)
                    {
                        Board[y, x - j] = BoardValues.Sunk;
                    }
                    break;
                case BoardValues.Tail_y:
                    Board[y, x] = BoardValues.Destroyed;
                    while ((y - i) >= 0)
                    {
                        if (Board[y - i, x] == BoardValues.Empty || Board[y - i, x] == BoardValues.Miss)
                        {
                            break;
                        }
                        else if (Board[y - i, x] != BoardValues.Destroyed)
                        {
                            return;
                        }
                        i++;
                    }
                    for (int j = 0; j < i; j++)
                    {
                        Board[y - j, x] = BoardValues.Sunk;
                    }
                    break;
                default:
                    return;
            }
        }

        private void RandomizeBoard_btn(object sender, EventArgs e)
        {
            shitPickedUp = false;
            this.gamestate.RandomizeBoard(this.gamestate.Board1, this.gamestate.availableShits1);
            DrawBoard(this.boardImages1, this.gamestate.Board1, true);
        }

        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            splashPlayer.Volume = (double)e.NewValue + 0.2;
            bgPlayer.Volume = (double)e.NewValue;
        }

        private void OnMediaFailed(object sender, ExceptionEventArgs e)
        {
            var exception = e.ErrorException;
            throw exception;
        }
        private void OnMediaEnded(object sender, EventArgs e)
        {
            var player = (sender as MediaPlayer);
            player.Position = TimeSpan.Zero;
        }
        private void OnMediaEnded_Splash(object sender, EventArgs e)
        {
            var player = (sender as MediaPlayer);
            player.Position = TimeSpan.Zero;
            player.Stop();
        }

        private void HandleMuteCheck(object sender, RoutedEventArgs e)
        {
            MainWindow.keepVolume = bgPlayer.Volume;
            volumeSlider2.Value = 0;
            bgPlayer.Volume = 0;
            splashPlayer.Volume = 0;
        }

        private void HandleMuteUnchecked(object sender, RoutedEventArgs e)
        {
            volumeSlider2.Value = MainWindow.keepVolume;
            bgPlayer.Volume = MainWindow.keepVolume;
            splashPlayer.Volume = MainWindow.keepVolume + 0.2;
        }

        private void Restart_btn_click(object sender, EventArgs e)
        {
            this.gamestate = new Gamestate(rows, cols);
            this.gamestate.RandomizeBoard(this.gamestate.Board1, this.gamestate.availableShits1);
            this.gamestate.RandomizeBoard(this.gamestate.Board2, this.gamestate.availableShits2);
            DrawBoard(this.boardImages1, this.gamestate.Board1, true);
            DrawBoard(this.boardImages2, this.gamestate.Board2, false);

            // create hidden empty board for computer
            this.hiddenBoard = new BoardValues[rows, cols];

            GameStatusLabel.Content = "Fire to start game!";
            Overlay.Visibility = Visibility.Hidden;
            Board1Overlay.Opacity = 1;
            Board2Overlay.Opacity = 1;
            this.clickable = true;
            gameStarted = false;
            this.RandomBtn.IsEnabled = true;
        }


        /*private void Shit_MLButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rc = sender as Rectangle;
            DataObject data = new DataObject(rc.Fill);
            DragDrop.DoDragDrop(rc, data, DragDropEffects.Move);
        }

        private void Target_Drop(object sender, DragEventArgs e)
        {
            SolidColorBrush scb = (SolidColorBrush)e.Data.GetData(typeof(SolidColorBrush));
            Target.Fill = scb;
        }*/
    }
}


// sound and visual indicator when poop is hit
// error when trying to start game while holding poop
// ghost of poop when moving
// indicate you can rotate by right clicking