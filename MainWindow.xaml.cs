using paint_project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace zad1___paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Point startPosition, endPosition;
        public int chosenTool = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void draw_Click(object sender, RoutedEventArgs e)
        {
            chosenTool = 0;
        }

        private void rectangle_Click(object sender, RoutedEventArgs e)
        {
            chosenTool = 1;
        }

        private void line_Click(object sender, RoutedEventArgs e)
        {
            chosenTool = 2;
        }
        private void LoadImage(object sender, RoutedEventArgs e)
        {
            try
            {
                FileReader reader = new FileReader();
                BitmapImage image = reader.ReadImageFIle();
                ImageDisplay imageWindow = new ImageDisplay(image);
                imageWindow.Show();
            }
            catch(Exception ex)
            {
                ErrorDisplay error = new ErrorDisplay(ex.Message);
                error.Show();
            }
        }
        private void SaveAsJPGFile(object sender, RoutedEventArgs e)
        {
            chosenTool = 6;
        }
        private object? figure;

        private void InitEventOnCanvas(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(canvasBoard);
            startPosition = e.GetPosition(canvasBoard);

            switch(chosenTool)
            {
                case 0:
                    //startPosition = e.GetPosition(canvasBoard);
                    Line tempPaint = CreateDrawing(startPosition, startPosition);
                    canvasBoard.Children.Add(tempPaint);
                    figure = tempPaint;
                    break;

                case 1:
                    Rectangle temp = new Rectangle
                    {
                        Width = 0,
                        Height = 0,
                        Fill = Brushes.Black
                    };
                    temp.SetCurrentValue(Canvas.TopProperty, startPosition.Y);
                    temp.SetCurrentValue(Canvas.LeftProperty, startPosition.X);
                    canvasBoard.Children.Add(temp);
                    figure = temp;
                    break;

                case 2:
                    Line tempLine = new Line
                    {
                        Stroke = Brushes.HotPink,
                        StrokeThickness = 3
                    };
                    tempLine.X1 = startPosition.X;
                    tempLine.Y1 = startPosition.Y;
                    tempLine.X2 = startPosition.X;
                    tempLine.Y2 = startPosition.Y;
                    canvasBoard.Children.Add(tempLine);
                    figure = tempLine;
                    break;

                default: break;
            }
        }

        private void ProcessingEventOnCanvas(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (figure is null)
                    return;
                endPosition = e.GetPosition(canvasBoard);
                switch(chosenTool)
                {
                    case 0:
                        canvasBoard.Children.Add(CreateDrawing(startPosition, endPosition));
                        startPosition = e.GetPosition(canvasBoard);
                        break;

                    case 1:
                        CreatingRectangle((Rectangle)figure, startPosition, endPosition);
                        break;

                    case 2:
                        CreatingLine((Line)figure, endPosition);
                        break;

                    default:
                        break;
                }
            }
        }

        private void EndEventOnCanvas(object sender, MouseButtonEventArgs e)
        {
            endPosition = e.GetPosition(canvasBoard);
            if (figure is null)
                return;
            switch (chosenTool)
            {
                case 0:
                    canvasBoard.Children.Add(CreateDrawing(startPosition, endPosition));
                    startPosition = e.GetPosition(canvasBoard);
                    break;

                case 1:
                    figure = CreatingRectangle((Rectangle)figure, startPosition, endPosition);
                    break;

                case 2:
                    figure = CreatingLine((Line)figure, endPosition);
                    break;

                default:
                    break;
            }
            figure = null;
            Mouse.Capture(null);
        }

        static private Line CreateDrawing(Point start, Point end)
        {
            Line line = new Line
            {
                Stroke = Brushes.DeepPink,
                StrokeThickness = 2
            };

            line.X1 = start.X;
            line.Y1 = start.Y;
            line.X2 = end.X;
            line.Y2 = end.Y;

            return line;
        }

        static private Rectangle CreatingRectangle(Rectangle item, Point start, Point end)
        {
            item.Width = Math.Abs(end.X - start.X);
            item.Height = Math.Abs(end.Y - start.Y);
            item.SetValue(Canvas.LeftProperty, Math.Min(start.X, end.X));
            item.SetValue(Canvas.TopProperty, Math.Min(start.Y, end.Y));
            return item;
        }

        static private Line CreatingLine(Line item, Point end)
        {
            item.X2 = end.X;
            item.Y2 = end.Y;
            return item;
        }
    }
}
