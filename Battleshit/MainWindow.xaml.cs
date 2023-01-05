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

        private readonly Dictionary<BoardValues, ImageSource> boardValToImage = new()
        {
            { BoardValues.Empty, Images.Shit_bg },
            { BoardValues.Destroyed, Images.Shit_bg },
            { BoardValues.Miss, Images.Shit_bg_miss},
            { BoardValues.Head_x, Images.Shit_head }
        };

        private bool clickable = true;

        public MainWindow()
        {
            InitializeComponent();
            gamestate = new Gamestate(rows, cols);
            boardImages1 = SetupBoard(Board1, gamestate.Board1, true);
            boardImages2 = SetupBoard(Board2, gamestate.Board2, false);

            GameStatusLabel.Content = "Your turn to fire!";

            // false: player 1, true: player 2
            /*while (gamestate.who_won == null)
            {
                // player 1 turn
                if (!gamestate.whos_turn)
                {
                    // check if player 1 lost
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if (((int)gamestate.Board1[i, j] != 0) && ((int)gamestate.Board1[i, j] != 6)) // its a shit
                            {
                                goto continueGame1;
                            }
                        }
                    }
                    gamestate.who_won = true;
                    goto CheckWon;

                continueGame1:
                    GameStatusLabel.Content = "Player 1 turn: pick a coordinate to attack";
                    gamestate.whos_turn = !gamestate.whos_turn;
                }
                else // player 2 turn
                {
                    // check if player 2 lost
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < cols; j++)
                        {
                            if (((int)gamestate.Board2[i, j] != 0) && ((int)gamestate.Board2[i, j] != 6)) // its a shit
                            {
                                goto continueGame2;
                            }
                        }
                    }
                    gamestate.who_won = false;
                    goto CheckWon;

                continueGame2:
                    GameStatusLabel.Content = "Player 2 turn: pick a coordinate to attack";
                    gamestate.whos_turn = !gamestate.whos_turn;
                }

                gamestate.who_won = false;
            }



        CheckWon:
            // check who won
            if (!(bool)gamestate.who_won)
            {
                Debug.WriteLine("Player 1 Won!");
            }
            else
            {
                Debug.WriteLine("Player 2 Won!");
            }*/
        }
        
        public Image[,] SetupBoard(UniformGrid Board, BoardValues[,] boardValues, bool isPlayerBoard)
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
                        case 0:
                            image.Source = Images.Shit_bg;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case (BoardValues)1:
                            image.Source = Images.Shit_head;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case (BoardValues)2:
                            image.Source = Images.Shit_head;
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = rotateTransform;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case (BoardValues)3:
                            image.Source = Images.Shit_body;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case (BoardValues)4:
                            image.Source = Images.Shit_body;
                            image.RenderTransformOrigin = new Point(0.5, 0.5);
                            image.RenderTransform = rotateTransform;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case (BoardValues)5:
                            image.Source = Images.Shit_tail;
                            images[x, y] = image;
                            Board.Children.Add(image);
                            break;
                        case (BoardValues)6:
                            image.Source = Images.Shit_tail;
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

        private void DrawBoard(Image[,] boardImages, BoardValues[,] boardValues)
        {
            RotateTransform rotateTransform = new RotateTransform(90);

            for (int x = 0; x < rows; x++)
            {
                for (int y = 0; y < cols; y++)
                {
                    switch (boardValues[x, y])
                    {
                        case 0:
                            boardImages[x, y].Source = Images.Shit_bg;
                            break;
                        case (BoardValues)1:
                            boardImages[x, y].Source = Images.Shit_head;
                            break;
                        case (BoardValues)2:
                            boardImages[x, y].Source = Images.Shit_head;
                            boardImages[x, y].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[x, y].RenderTransform = rotateTransform;
                            break;
                        case (BoardValues)3:
                            boardImages[x, y].Source = Images.Shit_body;
                            break;
                        case (BoardValues)4:
                            boardImages[x, y].Source = Images.Shit_body;
                            boardImages[x, y].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[x, y].RenderTransform = rotateTransform;
                            break;
                        case (BoardValues)5:
                            boardImages[x, y].Source = Images.Shit_tail;
                            break;
                        case (BoardValues)6:
                            boardImages[x, y].Source = Images.Shit_tail;
                            boardImages[x, y].RenderTransformOrigin = new Point(0.5, 0.5);
                            boardImages[x, y].RenderTransform = rotateTransform;
                            break;
                        case (BoardValues)7:
                            boardImages[x, y].Source = Images.Shit_bg_miss;
                            break;
                        case (BoardValues)8:
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
            // Disable click
            if (!clickable)
            {
                return;
            }

            clickable = false;

            Image img = sender as Image;
            UniformGrid parent = (UniformGrid)img.Parent;
            
            // get index of img in board
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
            clickable = true;
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
