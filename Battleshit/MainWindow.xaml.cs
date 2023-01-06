using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Battleshit
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Gamestate gamestate;
        private readonly int rows = 10, cols = 10;
        private readonly Image[,] boardImages1;
        private readonly Image[,] boardImages2;

        private readonly Random random = new Random();

        private bool clickable = true;

        public MainWindow()
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

            for (int x = 0; x < this.rows; x++)
            {
                for (int y = 0; y < this.cols; y++)
                {
                    Image image = new();

                    if (!isPlayerBoard) // allow click on enemy board
                    {
                        image.MouseEnter += new MouseEventHandler(HighlightElement);
                        image.MouseLeave += new MouseEventHandler(UnhighlightElement);
                        image.MouseDown += PlayerTurnClick;
                    }

                    switch (boardValues[x, y])
                    {
                        case BoardValues.Empty:
                            image.Source = Images.Shit_bg;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Head_x:
                            if (isPlayerBoard) { image.Source = Images.Shit_head; }
                            else { image.Source = Images.Shit_bg; }
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Head_y:
                            if (isPlayerBoard) { image.Source = Images.Shit_head; }
                            else { image.Source = Images.Shit_bg; }
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = rotateTransform;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Body_x:
                            if (isPlayerBoard) { image.Source = Images.Shit_body; }
                            else { image.Source = Images.Shit_bg; }
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Body_y:
                            if (isPlayerBoard) { image.Source = Images.Shit_body; }
                            else { image.Source = Images.Shit_bg; }
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = rotateTransform;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Tail_x:
                            if (isPlayerBoard) { image.Source = Images.Shit_tail; }
                            else { image.Source = Images.Shit_bg; }
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case BoardValues.Tail_y:
                            if (isPlayerBoard) { image.Source = Images.Shit_tail; }
                            else { image.Source = Images.Shit_bg; }
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = rotateTransform;
                            images[x, y] = image;
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

            for (int x = 0; x < this.rows; x++)
            {
                for (int y = 0; y < this.cols; y++)
                {
                    switch (boardValues[x, y])
                    {
                        case BoardValues.Empty:
                            boardImages[x, y].Source = Images.Shit_bg;
                            break;
                        case BoardValues.Head_x:
                            if (isPlayerBoard) { boardImages[x, y].Source = Images.Shit_head; }
                            else { boardImages[x, y].Source = Images.Shit_bg; }
                            break;
                        case BoardValues.Head_y:
                            if (isPlayerBoard) { boardImages[x, y].Source = Images.Shit_head; }
                            else { boardImages[x, y].Source = Images.Shit_bg; }
                            boardImages[x, y].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[x, y].RenderTransform = rotateTransform;
                            break;
                        case BoardValues.Body_x:
                            if (isPlayerBoard) { boardImages[x, y].Source = Images.Shit_body; }
                            else { boardImages[x, y].Source = Images.Shit_bg; }
                            break;
                        case BoardValues.Body_y:
                            if (isPlayerBoard) { boardImages[x, y].Source = Images.Shit_body; }
                            else { boardImages[x, y].Source = Images.Shit_bg; }
                            boardImages[x, y].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[x, y].RenderTransform = rotateTransform;
                            break;
                        case BoardValues.Tail_x:
                            if (isPlayerBoard) { boardImages[x, y].Source = Images.Shit_tail; }
                            else { boardImages[x, y].Source = Images.Shit_bg; }
                            break;
                        case BoardValues.Tail_y:
                            if (isPlayerBoard) { boardImages[x, y].Source = Images.Shit_tail; }
                            else { boardImages[x, y].Source = Images.Shit_bg; }
                            boardImages[x, y].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[x, y].RenderTransform = rotateTransform;
                            break;
                        case BoardValues.Destroyed:
                            boardImages[x, y].Source = Images.Shit_destroyed;
                            break;
                        case BoardValues.Miss:
                            boardImages[x, y].Source = Images.Shit_bg_miss;
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

        private void HighlightElement(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            img.Opacity = 0.5;
            
        }

        private void UnhighlightElement(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            img.Opacity = 1;
        }

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
            int x = index / 10;
            int y = index % 10;

            // check if shit tried already
            if ((this.gamestate.Board2[x, y] == BoardValues.Destroyed) || (this.gamestate.Board2[x, y] == BoardValues.Miss))
            {
                return;
            }
            // Check if is shit
            if (this.gamestate.Board2[x, y] != BoardValues.Empty)
            {
                // Mark as clicked
                gamestate.Board2[x, y] = BoardValues.Destroyed;
                DrawBoard(this.boardImages2, this.gamestate.Board2, false);
            } else
            {
                // Mark as clicked
                gamestate.Board2[x, y] = BoardValues.Miss;
                DrawBoard(this.boardImages2, this.gamestate.Board2, false);
            }

            // Check if player won
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    // exist a shit
                    if (new[] { 1, 2, 3, 4, 5, 6 }.Contains((int)gamestate.Board2[i, j]))
                    {
                        goto continueGame1;
                    }
                }
            }

            GameStatusLabel.Content = "You won!";
            return;

        continueGame1:
            // Do computer turn
            GameStatusLabel.Content = "Wait for computer...";
            await Task.Delay(1000);
            int rnd_x = random.Next(0, cols);
            int rnd_y = random.Next(0, rows);

            // check if shit tried already
            while ((gamestate.Board1[rnd_x, rnd_y] == BoardValues.Destroyed) || (gamestate.Board1[rnd_x, rnd_y] == BoardValues.Miss))
            {
                rnd_x = random.Next(0, cols);
                rnd_y = random.Next(0, rows);
            }
            if (gamestate.Board1[rnd_x, rnd_y] != BoardValues.Empty) 
            {
                // Mark as clicked
                gamestate.Board1[rnd_x, rnd_y] = BoardValues.Destroyed;
                DrawBoard(this.boardImages1, this.gamestate.Board1, true);
            } else
            {
                // Mark as clicked
                gamestate.Board1[rnd_x, rnd_y] = BoardValues.Miss;
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
                        goto continueGame2;
                    }
                }
            }

            GameStatusLabel.Content = "You lost!";
            return;

        continueGame2:

            // Enable Click
            this.clickable = true;
            GameStatusLabel.Content = "Your turn";

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
