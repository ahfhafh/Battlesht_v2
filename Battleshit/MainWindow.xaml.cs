using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

        public MainWindow()
        {
            InitializeComponent();
            gamestate = new Gamestate(rows, cols);
            boardImages1 = SetupBoard(Board1, gamestate.Board1);
            boardImages2 = SetupBoard(Board2, gamestate.Board2);
        }
        
        public Image[,] SetupBoard(System.Windows.Controls.Primitives.UniformGrid Board, BoardValues[,] boardValues)
        {
            Image[,] images = new Image[rows, cols];
            Board.Rows = rows;
            Board.Columns = cols;

            RotateTransform rotateTransform = new RotateTransform(90);

            Print2DArray(boardValues);
                
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    Image image = new Image();

                    if (!isPlayerBoard) // allow click on enemy board
                    {
                        image.MouseEnter += new MouseEventHandler(highlightElement);
                        image.MouseLeave += new MouseEventHandler(unhighlightElement);
                        image.MouseDown += playerTurnClick;
                    }

                    switch(boardValues[x, y])
                        {
                            if ((boardValues[x, y] == boardValues[x, y + 1]) && (boardValues[x, y] != boardValues[x, y - 1]))     // if shit is horizontal and head
                            {
                                image.Source = Images.Shit_head;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            } else if ((boardValues[x, y] == boardValues[x, y - 1]) && (boardValues[x, y] != boardValues[x, y + 1]))    // if shit is horizontal and tail
                            {
                                image.Source = Images.Shit_tail;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            } else if ((boardValues[x, y] == boardValues[x, y - 1]) && (boardValues[x, y] == boardValues[x, y + 1]))    // if shit is horizontal and body
                            {
                                image.Source = Images.Shit_body;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            }
                        } else if (y == 0) // at left edge
                        {
                            if (boardValues[x, y] == boardValues[x, y + 1])   // if shit is horizontal and head
                            {
                                image.Source = Images.Shit_head;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            }

                        } else if (y == cols - 1) // at right edge 
                        {
                            if (boardValues[x, y] == boardValues[x, y - 1])   // if shit is horizontal and tail
                            {
                                image.Source = Images.Shit_tail;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            }
                        }
                        if ((x != rows - 1) && (x != 0))   // not at top and bottom edges
                        {
                            if ((boardValues[x, y] == boardValues[x + 1, y]) && (boardValues[x, y] != boardValues[x - 1, y]))     // if shit is vertical and head
                            {
                                image.Source = Images.Shit_head;
                                image.RenderTransformOrigin = new Point(0.5, 0.5);
                                image.RenderTransform = rotateTransform;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            }
                            else if ((boardValues[x, y] == boardValues[x - 1, y]) && (boardValues[x, y] != boardValues[x + 1, y]))    // if shit is vertical and tail
                            {
                                image.Source = Images.Shit_tail;
                                image.RenderTransformOrigin = new Point(0.5, 0.5);
                                image.RenderTransform = rotateTransform;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            }
                            else if ((boardValues[x, y] == boardValues[x - 1, y]) && (boardValues[x, y] == boardValues[x + 1, y]))    // if shit is vertical and body
                            {
                                image.Source = Images.Shit_body;
                                image.RenderTransformOrigin = new Point(0.5, 0.5);
                                image.RenderTransform = rotateTransform;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            }
                        }
                        else if (x == 0) // at top edge
                        {
                            if (boardValues[x, y] == boardValues[x + 1, y])   // if shit is vertical and head
                            {
                                image.Source = Images.Shit_head;
                                image.RenderTransformOrigin = new Point(0.5, 0.5);
                                image.RenderTransform = rotateTransform;
                                images[x, y] = image;
                                Board.Children.Add(image);
                            }

                        }
                        else if (x == rows - 1) // at botom edge 
                        {
                            if (boardValues[x, y] == boardValues[x - 1, y])   // if shit is vertical and tail
                            {
                                image.Source = Images.Shit_tail;
                                image.RenderTransformOrigin = new Point(0.5, 0.5);
                                image.RenderTransform = rotateTransform;
                                images[x, y] = image;
                                Board.Children.Add(image);
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

        private void highlightElement(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            img.Opacity = 0.5;
            
            }
   
        private void unhighlightElement(object sender, MouseEventArgs e)
        {
            Image img = sender as Image;
            img.Opacity = 1;
        }

        private async void playerTurnClick(object sender, RoutedEventArgs e)
        {
            Image img = sender as Image;
            UniformGrid parent = (UniformGrid)img.Parent;

            // Disable click


            int index = parent.Children.IndexOf(img);
            int x = index / 10;
            int y = index % 10;

            // check if shit tried already
            if ((gamestate.Board2[x, y] == BoardValues.Destroyed) || (gamestate.Board2[x, y] == BoardValues.Miss))
            {
                return;
            }
            // Check if is shit
            if (img.Source != Images.Shit_bg)
        {
                // Mark as clicked
                gamestate.Board2[x, y] = BoardValues.Destroyed;
                DrawBoard(boardImages2, gamestate.Board2);
            } else
            {
                // Mark as clicked
                gamestate.Board2[x, y] = BoardValues.Miss;
                DrawBoard(boardImages2, gamestate.Board2);
            }

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
                DrawBoard(boardImages1, gamestate.Board1);
            } else
            {
                // Mark as clicked
                gamestate.Board1[rnd_x, rnd_y] = BoardValues.Miss;
                DrawBoard(boardImages1, gamestate.Board1);
            }

            // Enable Click
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
