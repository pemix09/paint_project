using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;


namespace paint_project
{
    /// <summary>
    /// Logika interakcji dla klasy Bezier.xaml
    /// </summary>
    public partial class Bezier : Window
    {
        private List<Point> points = new();
        private SolidColorBrush brush = new SolidColorBrush() { Color = Colors.DarkCyan };
        private SolidColorBrush lineBrush = new SolidColorBrush() { Color = Colors.Magenta };
        private double ACCURACY = 0.01;
        private ActionEnum selectedAction;
        private Ellipse selectedPoint;
        private int selectedPointFromListId;
        public Bezier()
        {
            InitializeComponent();
        }

        private void SetAction(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            selectedAction = btn.Name switch
            {
                var s when s == "DrawPoints" => ActionEnum.DrawPoints,
                var s when s == "MoveMouse" => ActionEnum.MoveMouse,
                var s when s == "MoveKeyboard" => ActionEnum.MoveKeyboard,
                _ => ActionEnum.NoAction
            };
            ChangeMenu(selectedAction);
        }

        private void ChangeMenu(ActionEnum selecteedCommand)
        {
            ActionType.Text = Dictionary.ActionTypesDictionary[selecteedCommand];
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedAction == ActionEnum.DrawPoints)
            {
                Mouse.Capture(canvas);
                Point pt = e.GetPosition(canvas);
                points.Add(pt);
                canvas.Children.Add(GetPoint(pt));
                if (points.Count > 1)
                    DrawBeziere();
                Mouse.Capture(null);
            }
        }

        private void DrawBeziere()
        {
            List<Shape> linesToRemove = new();
            foreach (var item in canvas.Children)
            {
                if (item is Line)
                    linesToRemove.Add(item as Shape);
            };
            linesToRemove.ForEach(line => canvas.Children.Remove(line));

            Point startPoint = points[0];
            for (double i = 0.0; i <= 1.0; i += ACCURACY)
            {
                double x = 0.0, y = 0.0;
                var degree = points.Count;
                for (int j = 0; j < degree; j++)
                {
                    x += MathsUtils.nCk(degree - 1, j) * Math.Pow(i, j) * Math.Pow(1 - i, degree - 1 - j) * points[j].X;
                    y += MathsUtils.nCk(degree - 1, j) * Math.Pow(i, j) * Math.Pow(1 - i, degree - 1 - j) * points[j].Y;
                }
                Point point = new Point { X = x, Y = y };
                canvas.Children.Add(GetLine(startPoint, point));
                startPoint = point;
            }
        }

        private Line GetLine(Point p1, Point p2)
        {
            return new Line { X1 = p1.X, X2 = p2.X, Y1 = p1.Y, Y2 = p2.Y, Stroke = lineBrush, StrokeThickness = 3 };
        }

        private Ellipse GetPoint(Point pt)
        {
            var point = new Ellipse() { Height = 10, Width = 10, };
            point.Stroke = brush;
            point.StrokeThickness = 5;
            point.SetValue(Canvas.TopProperty, pt.Y - 5);
            point.SetValue(Canvas.LeftProperty, pt.X - 5);
            point.MouseLeftButtonUp += new MouseButtonEventHandler(Point_MouseLeftButtonUp);
            point.MouseLeftButtonDown += new MouseButtonEventHandler(Point_MouseLeftButtonDown);
            point.MouseMove += new MouseEventHandler(Point_MouseMove);
            return point;
        }

        private void Point_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedAction == ActionEnum.MoveMouse || selectedAction == ActionEnum.MoveKeyboard)
            {
                var elem = (Ellipse)sender;
                elem.CaptureMouse();
                selectedPoint = elem;
                var x = selectedPoint.GetValue(Canvas.LeftProperty) as double?;
                var y = selectedPoint.GetValue(Canvas.TopProperty) as double?;
                selectedPointFromListId = points.FindIndex(p => p.X == x + 5 && p.Y == y + 5);
                if (selectedAction == ActionEnum.MoveKeyboard)
                {
                    X.Text = (x + 5).ToString();
                    Y.Text = (y + 5).ToString();
                }
            }
        }

        private void Point_MouseMove(object sender, MouseEventArgs e)
        {
            if (selectedPoint == null || Mouse.Captured != sender)
                return;

            Point pt = e.GetPosition(canvas);
            if (selectedAction == ActionEnum.MoveMouse && selectedPointFromListId != -1)
            {
                points[selectedPointFromListId] = new Point { X = pt.X, Y = pt.Y };
                selectedPoint.SetValue(Canvas.TopProperty, pt.Y - 5);
                selectedPoint.SetValue(Canvas.LeftProperty, pt.X - 5);
                DrawBeziere();
            }
        }

        private void Point_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
            if (selectedAction != ActionEnum.MoveKeyboard)
                selectedPoint = null;

        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            points.Clear();
        }

        private void AddOrMovePoint(object sender, RoutedEventArgs e)
        {
            double x, y;
            if (!double.TryParse(X.Text, out x) || !double.TryParse(Y.Text, out y))
            {
                MessageBox.Show("Incorrect values", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (selectedAction != ActionEnum.MoveKeyboard)
            {
                Point p = new Point { X = x, Y = y };
                points.Add(p);
                canvas.Children.Add(GetPoint(p));
            }
            else
            {
                if (selectedPoint != null)
                {
                    points[selectedPointFromListId] = new Point { X = x, Y = y };
                    selectedPoint.SetValue(Canvas.TopProperty, y - 5);
                    selectedPoint.SetValue(Canvas.LeftProperty, x - 5);
                }
            }
            DrawBeziere();
        }
    }
}
