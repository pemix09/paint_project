﻿using System;
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
using System.Windows.Shapes;

namespace paint_project
{
    /// <summary>
    /// Logika interakcji dla klasy Transformations.xaml
    /// </summary>
    public partial class Transformations : Window
    {
        private TransformationActions selectedAction;
        private Polygon currentPolygon;
        private Polygon currentPolygonCopy;
        private SolidColorBrush linebrush = new SolidColorBrush() { Color = Colors.DarkCyan };
        private SolidColorBrush fill = new SolidColorBrush() { Color = Colors.Gray };
        private Point? startPoint;
        private Point? selectedPoint;
        private string filePath = $"{Directory.GetCurrentDirectory()}/save.json";
        public Transformations()
        {
            InitializeComponent();
        }

        private void SetAction(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            selectedAction = btn.Name switch
            {
                var s when s == "Draw" => TransformationActions.Draw,
                var s when s == "MoveMouse" => TransformationActions.MouseMove,
                var s when s == "MoveKeyboard" => TransformationActions.MoveKeyboard,
                var s when s == "Rotate" => TransformationActions.Rotate,
                var s when s == "ScaleBtn" => TransformationActions.Scale,
                _ => TransformationActions.NoAction
            };
            ChangeMenu(selectedAction);
        }

        private void ChangeMenu(TransformationActions selecteedCommand)
        {
            ActionType.Text = TranformationDictionary.ActionTypesDictionary[selecteedCommand];
        }

        private void Canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedAction == TransformationActions.Draw || selectedAction == TransformationActions.Rotate || selectedAction == TransformationActions.Scale)
            {
                Mouse.Capture(canvas);
                Point pt = e.GetPosition(canvas);
                if (selectedAction == TransformationActions.Draw)
                    AddPointToPolygon(pt);
                else
                    PrepareRotation(pt);

                Mouse.Capture(null);
            }
        }

        private void PrepareRotation(Point pt)
        {
            selectedPoint = pt;
            if (selectedAction == TransformationActions.Rotate)
            {
                XRotate.Text = pt.X.ToString();
                YRotate.Text = pt.Y.ToString();
            }
            else
            {
                XScale.Text = pt.X.ToString();
                YScale.Text = pt.Y.ToString();
            }
        }

        private void AddPointToPolygon(Point pt)
        {
            if (currentPolygon == null)
                GetNewPolygon();
            currentPolygon.Points.Add(pt);
        }

        private void GetNewPolygon()
        {
            currentPolygon = new Polygon();
            currentPolygon.Fill = fill;
            currentPolygon.Stroke = linebrush;
            currentPolygon.StrokeThickness = 7;
            currentPolygon.MouseMove += new MouseEventHandler(Polygon_MouseMove);
            currentPolygon.MouseLeftButtonDown += new MouseButtonEventHandler(Polygon_MouseLeftButtonDown);
            currentPolygon.MouseLeftButtonUp += new MouseButtonEventHandler(Polygon_MouseLeftButtonUp);
            canvas.Children.Add(currentPolygon);
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            currentPolygon = null;
        }

        private void AddPoint(object sender, RoutedEventArgs e)
        {
            double x, y;
            if (!double.TryParse(X.Text, out x) || !double.TryParse(Y.Text, out y))
            {
                MessageBox.Show("Podaj poprawne wartośći", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            Point p = new Point { X = x, Y = y };
            if (currentPolygon == null)
                GetNewPolygon();
            currentPolygon.Points.Add(p);
        }
        private void Polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (selectedAction == TransformationActions.MouseMove)
            {
                var elem = (Polygon)sender;
                elem.CaptureMouse();
                currentPolygon = elem;
                currentPolygonCopy = new Polygon();
                currentPolygonCopy.Points = elem.Points;
                startPoint = e.GetPosition(canvas);
            }
        }

        private void Polygon_MouseMove(object sender, MouseEventArgs e)
        {
            if (startPoint == null || Mouse.Captured != sender)
                return;

            Point pt = e.GetPosition(canvas);
            if (selectedAction == TransformationActions.MouseMove)
            {
                var point = startPoint.Value;
                var deltaX = pt.X - point.X;
                var deltaY = pt.Y - point.Y;
                PointCollection newPoints = new();
                for (int i = 0; i < currentPolygonCopy.Points.Count; i++)
                {
                    newPoints.Add(new()
                    {
                        X = currentPolygonCopy.Points[i].X + deltaX,
                        Y = currentPolygonCopy.Points[i].Y + deltaY
                    });
                }
                currentPolygon.Points = newPoints;
            }
        }

        private void Polygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void Translate(object sender, RoutedEventArgs e)
        {
            if (selectedAction != TransformationActions.MoveKeyboard || currentPolygon == null)
                return;

            double x, y;
            if (!double.TryParse(XTranslate.Text, out x) || !double.TryParse(YTranslate.Text, out y))
            {
                MessageBox.Show("Podaj poprawne wartośći", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PointCollection newPoints = new();
            foreach (var point in currentPolygon.Points)
            {
                newPoints.Add(new()
                {
                    X = point.X + x,
                    Y = point.Y + y
                });
            }
            currentPolygon.Points = newPoints;
        }

        private void Rotation(object sender, RoutedEventArgs e)
        {
            if (selectedAction != TransformationActions.Rotate || currentPolygon == null)
                return;

            double x, y, angle;
            if (!double.TryParse(XRotate.Text, out x) || !double.TryParse(YRotate.Text, out y) || !double.TryParse(AngleRotate.Text, out angle))
            {
                MessageBox.Show("Podaj poprawne wartośći", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PointCollection newPoints = new();
            foreach (var point in currentPolygon.Points)
            {
                newPoints.Add(new()
                {
                    X = point.X.GetNewX(point.Y, angle, x, y),
                    Y = point.Y.GetNewY(point.X, angle, x, y)
                });
            }
            currentPolygon.Points = newPoints;
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if ((selectedAction != TransformationActions.Rotate && selectedAction != TransformationActions.Scale) || currentPolygon == null)
                return;

            double x, y, angle = 0.0, scale = 0.0;
            if (selectedAction == TransformationActions.Rotate && (!double.TryParse(XRotate.Text, out x) ||
                !double.TryParse(YRotate.Text, out y) || !double.TryParse(AngleRotate.Text, out angle)))
            {
                MessageBox.Show("Podaj poprawne wartośći", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!double.TryParse(XScale.Text, out x) || !double.TryParse(YScale.Text, out y) || !double.TryParse(ScaleScale.Text, out scale))
            {
                MessageBox.Show("Podaj poprawne wartośći", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PointCollection newPoints = new();
            if (e.Delta < 0)
            {
                angle = -angle;
                scale = 1 / scale;
            }
            foreach (var point in currentPolygon.Points)
            {
                newPoints.Add(new()
                {
                    X = selectedAction == TransformationActions.Rotate ? point.X.GetNewX(point.Y, angle, x, y) : point.X.ScaleX(scale, x),
                    Y = selectedAction == TransformationActions.Rotate ? point.Y.GetNewY(point.X, angle, x, y) : point.Y.ScaleY(scale, y)
                });
            }
            currentPolygon.Points = newPoints;
        }

        private void Scale(object sender, RoutedEventArgs e)
        {
            if (selectedAction != TransformationActions.Scale || currentPolygon == null)
                return;

            double x, y, scale;
            if (!double.TryParse(XScale.Text, out x) || !double.TryParse(YScale.Text, out y) || !double.TryParse(ScaleScale.Text, out scale))
            {
                MessageBox.Show("Podaj poprawne wartośći", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            PointCollection newPoints = new();
            foreach (var point in currentPolygon.Points)
            {
                newPoints.Add(new()
                {
                    X = point.X.ScaleX(scale, x),
                    Y = point.Y.ScaleY(scale, y)
                });
            }
            currentPolygon.Points = newPoints;
        }

        private void Serialize(object sender, RoutedEventArgs e)
        {
            var serialized = TransformationUtils.SerializePoints(currentPolygon.Points);
            using var stream = new StreamWriter(filePath);
            stream.Write(serialized);
        }

        private void DeserializeLast(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Brak zapisanych wartości", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using var stream = new StreamReader(filePath);
            var serialized = stream.ReadToEnd();
            var points = TransformationUtils.Deserialize(serialized);

            canvas.Children.Clear();
            currentPolygon = new Polygon { Points = new PointCollection(points), Stroke = linebrush, Fill = fill, StrokeThickness = 7 };
            canvas.Children.Add(currentPolygon);
        }
    }
}

