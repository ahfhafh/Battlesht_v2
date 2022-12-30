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

/*            Print2DArray(boardValues);*/
                
            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    Image image = new Image();

                    if (boardValues[x, y] != 0)    // if there is a shit
                    {
                        Debug.WriteLine(x + " " + y);
                        // determine if shit is horizontal or vertical
                        if ((y != cols - 1) && (y != 0))   // not at left and right edges
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
                    else
                    {
                        image.Source = Images.Shit_bg;
                        images[x, y] = image;
                        Board.Children.Add(image);
                    }
                }
            }
   
            return images;
        }

        public static void Print2DArray<T>(T[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    System.Diagnostics.Debug.Write(matrix[i, j] + "\t");
                }
                System.Diagnostics.Debug.WriteLine("|");
            }
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
