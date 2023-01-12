using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        private readonly Random random = new Random();

        private bool clickable = true;

        public SinglePlayer()
        {
            InitializeComponent();
            this.gamestate = new Gamestate(rows, cols);
            this.boardImages1 = SetupBoard(Board1, gamestate.Board1, true);
            this.boardImages2 = SetupBoard(Board2, gamestate.Board2, false);

            GameStatusLabel.Content = "Your turn to fire!";
        }

        public Image[,] SetupBoard(UniformGrid Board, BoardValues[,] boardValues, bool isPlayerBoard)
        {
            Image[,] images = new Image[this.rows, this.cols];
            Board.Rows = this.rows;
            Board.Columns = this.cols;

            RotateTransform rotateTransform = new(90);

            Print2DArray(boardValues);

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
        private async void PlayerTurnClick(object sender, RoutedEventArgs e)
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
                // Check if entire shit has sunk
                int i = 1;
                switch (this.gamestate.Board2[y, x])
                {
                    case BoardValues.Head_x:
                        while ((x + i) < this.cols)
                        {
                            if (this.gamestate.Board2[y, x + i] == BoardValues.Empty || this.gamestate.Board2[y, x + i] == BoardValues.Miss)
                            {
                                break;
                            } else if (this.gamestate.Board2[y, x + i] != BoardValues.Destroyed)
                            {
                                goto notSunk;
                            }
                            i++;
                        }
                        for (int j = 0; j < i; j++)
                        {
                            this.gamestate.Board2[y, x + j] = BoardValues.Sunk;
                        }
                        break;
                    case BoardValues.Head_y:
                        while ((y + i) < this.rows)
                        {
                            if (this.gamestate.Board2[y + i, x] == BoardValues.Empty || this.gamestate.Board2[y + i, x] == BoardValues.Miss)
                            {
                                break;
                            }
                            else if (this.gamestate.Board2[y + i, x] != BoardValues.Destroyed)
                            {
                                goto notSunk;
                            }
                            i++;
                        }
                        for (int j = 0; j < i; j++)
                        {
                            this.gamestate.Board2[y + j, x] = BoardValues.Sunk;
                        }
                        break;
                    case BoardValues.Body_x:
                        while ((x + i) < this.cols)
                        {
                            if (this.gamestate.Board2[y, x + i] == BoardValues.Empty || this.gamestate.Board2[y, x + i] == BoardValues.Miss)
                            {
                                break;
                            }
                            else if (this.gamestate.Board2[y, x + i] != BoardValues.Destroyed)
                            {
                                goto notSunk;
                            }
                            i++;
                        }
                        for (int j = 0; j < i; j++)
                        {
                            this.gamestate.Board2[y, x + j] = BoardValues.Sunk;
                        }
                        while ((x - i) >= 0)
                        {
                            if (this.gamestate.Board2[y, x - i] == BoardValues.Empty || this.gamestate.Board2[y, x - i] == BoardValues.Miss)
                            {
                                break;
                            }
                            else if (this.gamestate.Board2[y, x - i] != BoardValues.Destroyed)
                            {
                                goto notSunk;
                            }
                            i++;
                        }
                        for (int j = 0; j < i; j++)
                        {
                            this.gamestate.Board2[y, x - j] = BoardValues.Sunk;
                        }
                        break;
                    case BoardValues.Body_y:
                        while ((y + i) < this.rows)
                        {
                            if (this.gamestate.Board2[y + i, x] == BoardValues.Empty || this.gamestate.Board2[y + i, x] == BoardValues.Miss)
                            {
                                break;
                            }
                            else if (this.gamestate.Board2[y + i, x] != BoardValues.Destroyed)
                            {
                                goto notSunk;
                            }
                            i++;
                        }
                        for (int j = 0; j < i; j++)
                        {
                            this.gamestate.Board2[y + j, x] = BoardValues.Sunk;
                        }
                        while ((y - i) >= 0)
                        {
                            if (this.gamestate.Board2[y - i, x] == BoardValues.Empty || this.gamestate.Board2[y - i, x] == BoardValues.Miss)
                            {
                                break;
                            }
                            else if (this.gamestate.Board2[y - i, x] != BoardValues.Destroyed)
                            {
                                goto notSunk;
                            }
                            i++;
                        }
                        for (int j = 0; j < i; j++)
                        {
                            this.gamestate.Board2[y - j, x] = BoardValues.Sunk;
                        }
                        break;
                    case BoardValues.Tail_x:
                        while ((x - i) >= 0)
                        {
                            if (this.gamestate.Board2[y, x - i] == BoardValues.Empty || this.gamestate.Board2[y, x - i] == BoardValues.Miss)
                            {
                                break;
                            }
                            else if (this.gamestate.Board2[y, x - i] != BoardValues.Destroyed)
                            {
                                goto notSunk;
                            }
                            i++;
                        }
                        for (int j = 0; j < i; j++)
                        {
                            this.gamestate.Board2[y, x - j] = BoardValues.Sunk;
                        }
                        break;
                    case BoardValues.Tail_y:
                        while ((y - i) >= 0)
                        {
                            if (this.gamestate.Board2[y - i, x] == BoardValues.Empty || this.gamestate.Board2[y - i, x] == BoardValues.Miss)
                            {
                                break;
                            }
                            else if (this.gamestate.Board2[y - i, x] != BoardValues.Destroyed)
                            {
                                goto notSunk;
                            }
                            i++;
                        }
                        for (int j = 0; j < i; j++)
                        {
                            this.gamestate.Board2[y - j, x] = BoardValues.Sunk;
                        }
                        break;
                    default:
                        goto notSunk;
                }
                goto Sunk;
            notSunk:
                Debug.WriteLine("Not");
                // Mark as clicked
                this.gamestate.Board2[y, x] = BoardValues.Destroyed;
            Sunk:
                DrawBoard(this.boardImages2, this.gamestate.Board2, false);
            }
            else
            {
                // Mark as clicked
                this.gamestate.Board2[y, x] = BoardValues.Miss;
                DrawBoard(this.boardImages2, this.gamestate.Board2, false);
            }

            // Check if player won
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // exist a shit
                    if (new[] { 1, 2, 3, 4, 5, 6 }.Contains((int)this.gamestate.Board2[i, j]))
                    {
                        goto continueToComputerTurn;
                    }
                }
            }

            GameStatusLabel.Content = "You won!";
            return;

        continueToComputerTurn:
            // Do computer turn
            GameStatusLabel.Content = "Wait for computer...";
            await Task.Delay(1000);
            int rnd_x = random.Next(0, cols);
            int rnd_y = random.Next(0, rows);

            // Find optimal next position to check if exists
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    // Check if shit at position is destroyed and not sunken
                    if (gamestate.Board1[i, j] == BoardValues.Destroyed && gamestate.Board1[i, j] != BoardValues.Sunk)
                    {
                    }
                }
            }

            // Check if shit tried already
            while ((this.gamestate.Board1[rnd_y, rnd_x] == BoardValues.Destroyed) || (this.gamestate.Board1[rnd_y, rnd_x] == BoardValues.Miss))
            {
                rnd_x = random.Next(0, cols);
                rnd_y = random.Next(0, rows);
            }
            if (this.gamestate.Board1[rnd_y, rnd_x] != BoardValues.Empty)
            {
                // Mark as clicked
                this.gamestate.Board1[rnd_y, rnd_x] = BoardValues.Destroyed;
                DrawBoard(this.boardImages1, this.gamestate.Board1, true);
            }
            else
            {
                // Mark as clicked
                this.gamestate.Board1[rnd_y, rnd_x] = BoardValues.Miss;
                DrawBoard(this.boardImages1, this.gamestate.Board1, true);
            }

            // Check if computer won
            for (int i = 0; i < this.rows; i++)
            {
                for (int j = 0; j < this.cols; j++)
                {
                    // exist a shit
                    if (new[] { 1, 2, 3, 4, 5, 6 }.Contains((int)this.gamestate.Board1[i, j]))
                    {
                        goto continueGame;
                    }
                }
            }

            GameStatusLabel.Content = "You lost!";
            return;

        continueGame:

            // Enable Click
            this.clickable = true;
            GameStatusLabel.Content = "Your turn";

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
