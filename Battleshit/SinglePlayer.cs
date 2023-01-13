using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Battleshit
{
    /// <summary>
    /// Interaction logic for SinglePlayer.xaml
    /// </summary>
    public partial class SinglePlayer : Page
    {
        private Gamestate gamestate;
        private readonly int rows = 10, cols = 10;
        private readonly Image[,] boardImages1;
        private readonly Image[,] boardImages2;
        private BoardValues[,] hiddenBoard;

        private readonly Random random = new Random();

        private bool clickable = true;

        public SinglePlayer()
        {
            InitializeComponent();
            this.gamestate = new Gamestate(rows, cols);
            this.boardImages1 = SetupBoard(Board1, gamestate.Board1, true);
            this.boardImages2 = SetupBoard(Board2, gamestate.Board2, false);

            // create hidden empty board for computer
            this.hiddenBoard = new BoardValues[this.rows, this.cols];

            GameStatusLabel.Content = "Your turn to fire!";
        }

        public Image[,] SetupBoard(UniformGrid Board, BoardValues[,] boardValues, bool isPlayerBoard)
        {
            Image[,] images = new Image[this.rows, this.cols];
            Board.Rows = this.rows;
            Board.Columns = this.cols;

            RotateTransform rotateTransform = new(90);


            for (int y = 0; y < this.rows; y++)
            {
                for (int x = 0; x < this.cols; x++)
                {
                    Image image = new();

                    if (!isPlayerBoard) // allow click on enemy board
                    {
                        image.MouseEnter += new MouseEventHandler(HighlightElement);
                        image.MouseLeave += new MouseEventHandler(UnhighlightElement);
                        image.MouseDown += PlayerTurnClick;
                    }

                    switch (boardValues[y, x])
                    {
                        case BoardValues.Empty:
                            image.Source = Images.Shit_bg;
                            images[y, x] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Head_x:
                            if (isPlayerBoard) { image.Source = Images.Shit_head; }
                            else { image.Source = Images.Shit_bg; }
                            images[y, x] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Head_y:
                            if (isPlayerBoard) { image.Source = Images.Shit_head; }
                            else { image.Source = Images.Shit_bg; }
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = rotateTransform;
                            images[y, x] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Body_x:
                            if (isPlayerBoard) { image.Source = Images.Shit_body; }
                            else { image.Source = Images.Shit_bg; }
                            images[y, x] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Body_y:
                            if (isPlayerBoard) { image.Source = Images.Shit_body; }
                            else { image.Source = Images.Shit_bg; }
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = rotateTransform;
                            images[y, x] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Tail_x:
                            if (isPlayerBoard) { image.Source = Images.Shit_tail; }
                            else { image.Source = Images.Shit_bg; }
                            images[y, x] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Tail_y:
                            if (isPlayerBoard) { image.Source = Images.Shit_tail; }
                            else { image.Source = Images.Shit_bg; }
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = rotateTransform;
                            images[y, x] = image;
                            Board.Children.Add(image);
                            break;
                        default: break;
                    }
                }
            }

            return images;
        }

        private void DrawBoard(Image[,] boardImages, BoardValues[,] boardValues, bool isPlayerBoard)
        {
            RotateTransform rotateTransform = new(90);

            for (int y = 0; y < this.rows; y++)
            {
                for (int x = 0; x < this.cols; x++)
                {
                    switch (boardValues[y, x])
                    {
                        case BoardValues.Empty:
                            boardImages[y, x].Source = Images.Shit_bg;
                            break;
                        case BoardValues.Head_x:
                            if (isPlayerBoard) { boardImages[y, x].Source = Images.Shit_head; }
                            else { boardImages[y, x].Source = Images.Shit_bg; }
                            boardImages[y, x].RenderTransform = null;
                            break;
                        case BoardValues.Head_y:
                            if (isPlayerBoard) { boardImages[y, x].Source = Images.Shit_head; }
                            else { boardImages[y, x].Source = Images.Shit_bg; }
                            boardImages[y, x].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[y, x].RenderTransform = rotateTransform;
                            break;
                        case BoardValues.Body_x:
                            if (isPlayerBoard) { boardImages[y, x].Source = Images.Shit_body; }
                            else { boardImages[y, x].Source = Images.Shit_bg; }
                            boardImages[y, x].RenderTransform = null;
                            break;
                        case BoardValues.Body_y:
                            if (isPlayerBoard) { boardImages[y, x].Source = Images.Shit_body; }
                            else { boardImages[y, x].Source = Images.Shit_bg; }
                            boardImages[y, x].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[y, x].RenderTransform = rotateTransform;
                            break;
                        case BoardValues.Tail_x:
                            if (isPlayerBoard) { boardImages[y, x].Source = Images.Shit_tail; }
                            else { boardImages[y, x].Source = Images.Shit_bg; }
                            boardImages[y, x].RenderTransform = null;
                            break;
                        case BoardValues.Tail_y:
                            if (isPlayerBoard) { boardImages[y, x].Source = Images.Shit_tail; }
                            else { boardImages[y, x].Source = Images.Shit_bg; }
                            boardImages[y, x].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[y, x].RenderTransform = rotateTransform;
                            break;
                        case BoardValues.Destroyed:
                            boardImages[y, x].Source = Images.Shit_destroyed;
                            break;
                        case BoardValues.Miss:
                            boardImages[y, x].Source = Images.Shit_bg_miss;
                            break;
                        case BoardValues.Sunk:
                            boardImages[y, x].Source = Images.Shit_sunk;
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
        }

        // Set opacity of half when mouse hover
        private void HighlightElement(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            img.Opacity = 0.5;

        }

        // Reset opacity when mouse not hover
        private void UnhighlightElement(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            img.Opacity = 1;
        }

        // Process a turn when player clicks
        private void PlayerTurnClick(object sender, RoutedEventArgs e)
        {
            // Disable click
            if (!this.clickable)
            {
                return;
            }

            this.clickable = false;

            Image img = sender as Image;
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
            if (CheckWon(this.gamestate.Board2))
            {
                GameStatusLabel.Content = "You won!";
                return;
            }

            // Do computer turn
            GameStatusLabel.Content = "Wait for computer...";
            int rnd_x = random.Next(0, cols);
            int rnd_y = random.Next(0, rows);

            // Find optimal next position to check if exists
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    // Check if shit at position is destroyed and not sunken
                    if (this.hiddenBoard[i, j] == BoardValues.Destroyed)
                    {
                        var dir = FindShitDir(this.hiddenBoard, i, j, BoardValues.Destroyed);
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
                        (FindShitDir(this.hiddenBoard, i, j + c + 1, BoardValues.Destroyed) == null) && (FindShitDir(this.hiddenBoard, i, j + c + 1, BoardValues.Sunk) == null)) 
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
                        (FindShitDir(this.hiddenBoard, i + c + 1, j, BoardValues.Destroyed) == null) && (FindShitDir(this.hiddenBoard, i + c + 1, j, BoardValues.Sunk) == null)) 
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
            if (CheckWon(this.gamestate.Board1))
            {
                GameStatusLabel.Content = "You lost!";
                return;
            }

            // Enable Click
            this.clickable = true;
            GameStatusLabel.Content = "Your turn";

        }

        // Find orientation of shit
        // True = vertical
        private bool? FindShitDir(BoardValues[,] Board, int y, int x, BoardValues BoardValue)
        {
            // Check if any adjacent direction is the specified boardvalue
            if (x + 1 < cols && Board[y, x + 1] == BoardValue)
            {
                return false;
            }
            else if (x - 1 >= 0 && Board[y, x - 1] == BoardValue)
            {
                return false;
            }
            else if (y + 1 < rows && Board[y + 1, x] == BoardValue)
            {
                return true;
            }
            else if (y - 1 >= 0 && Board[y - 1, x] == BoardValue)
            {
                return true;
            }

            return null;
        }

        private bool CheckWon(BoardValues[,] Board)
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
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

        private void ApplySunk(BoardValues[,] Board, int y, int x)
        {
            int i = 1;
            int c = 1;
            switch (Board[y, x])
            {
                case BoardValues.Head_x:
                    Board[y, x] = BoardValues.Destroyed;
                    while ((x + i) < this.cols)
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
                    while ((y + i) < this.rows)
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
                    while ((x + i) < this.cols)
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
                    while ((y + i) < this.rows)
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
            this.gamestate.RandomizeBoard(this.gamestate.Board1, this.gamestate.availableShits1);
            DrawBoard(this.boardImages1, this.gamestate.Board1, true);
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
