using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace paint_project
{
    /// <summary>
    /// Logika interakcji dla klasy RGB_Cube.xaml
    /// </summary>
    public partial class RGB_Cube : Window
    {
        private byte R, G, B = 0;
        private byte BLACK, CYAN, MAGENTA, YELLOW = 0;
        private bool connect = false;
        private bool isHandled = false;
        public RGB_Cube()
        {
            InitializeComponent();
            ColorRGBCube();
        }

        private void ConnectCheckBox_Click(object sender, RoutedEventArgs e)
        {
            connect = (sender as CheckBox).IsChecked.Value;
        }


        private void RSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isHandled)
                return;
            R = (byte)e.NewValue;
            RTextBox.Text = R.ToString();
            FillRGBRectangle();
        }

        private void GSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isHandled)
                return;
            G = (byte)e.NewValue;
            GTextBox.Text = G.ToString();
            FillRGBRectangle();
        }

        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isHandled)
                return;
            B = (byte)e.NewValue;
            BTextBox.Text = B.ToString();
            FillRGBRectangle();
        }

        private void RTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isHandled)
                return;
            byte value;
            if (!byte.TryParse((sender as TextBox).Text, out value))
            {
                R = 0;
                RSlider.Value = 0;
                return;
            }
            RSlider.Value = value;
            R = value;
            FillRGBRectangle();
        }

        private void GTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isHandled)
                return;
            byte value;
            if (!byte.TryParse((sender as TextBox).Text, out value))
            {
                G = 0;
                GSlider.Value = 0;
                return;
            }
            GSlider.Value = value;
            G = value;
            FillRGBRectangle();
        }

        private void BTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isHandled)
                return;
            byte value;
            if (!byte.TryParse((sender as TextBox).Text, out value))
            {
                B = 0;
                BSlider.Value = 0;
                return;
            }
            BSlider.Value = value;
            B = value;
            FillRGBRectangle();
        }

        private void FillRGBRectangle()
        {
            isHandled = true;
            RGBRectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(R, G, B));
            if (connect)
            {
                ConvertRGBToCMYKAndFill();
            }
            isHandled = false;
        }

        private void ConvertRGBToCMYKAndFill()
        {
            CMYKRectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(R, G, B));
            double r = R / 255.0;
            double g = G / 255.0;
            double b = B / 255.0;

            BLACK = (byte)(Math.Min(1 - r, Math.Min(1 - g, 1 - b)) * 100);
            double black = BLACK / 100.0;
            CYAN = BLACK == 100 ? (byte)0 : (byte)(((1 - r - black) / (1 - black)) * 100);
            MAGENTA = BLACK == 100 ? (byte)0 : (byte)(((1 - g - black) / (1 - black)) * 100);
            YELLOW = BLACK == 100 ? (byte)0 : (byte)(((1 - b - black) / (1 - black)) * 100);

            BlackSlider.Value = BLACK;
            CyanSlider.Value = CYAN;
            MagentaSlider.Value = MAGENTA;
            YellowSlider.Value = YELLOW;
            BlackTextBox.Text = BLACK.ToString();
            CyanTextBox.Text = CYAN.ToString();
            MagentaTextBox.Text = MAGENTA.ToString();
            YellowTextBox.Text = YELLOW.ToString();
        }

        private void BlackSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isHandled)
                return;
            BLACK = (byte)e.NewValue;
            BlackTextBox.Text = BLACK.ToString();
            ConvertCMYK_TO_RGB_andFill();
        }

        private void CyanSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isHandled)
                return;
            CYAN = (byte)e.NewValue;
            CyanTextBox.Text = CYAN.ToString();
            ConvertCMYK_TO_RGB_andFill();
        }

        private void MagentaSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isHandled)
                return;
            MAGENTA = (byte)e.NewValue;
            MagentaTextBox.Text = MAGENTA.ToString();
            ConvertCMYK_TO_RGB_andFill();
        }

        private void YellowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isHandled)
                return;
            YELLOW = (byte)e.NewValue;
            YellowTextBox.Text = YELLOW.ToString();
            ConvertCMYK_TO_RGB_andFill();
        }

        private void BlackTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isHandled)
                return;
            byte value;
            if (!byte.TryParse((sender as TextBox).Text, out value) || value > 100)
            {

                BLACK = 0;
                BlackSlider.Value = 0;
                return;
            }
            BlackSlider.Value = value;
            BLACK = value;
            ConvertCMYK_TO_RGB_andFill();
        }

        private void CyanTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isHandled)
                return;
            byte value;
            if (!byte.TryParse((sender as TextBox).Text, out value) || value > 100)
            {
                CYAN = 0;
                CyanSlider.Value = 0;
                return;
            }
            CyanSlider.Value = value;
            CYAN = value;
            ConvertCMYK_TO_RGB_andFill();
        }

        private void MagentaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isHandled)
                return;
            byte value;
            if (!byte.TryParse((sender as TextBox).Text, out value) || value > 100)
            {
                MAGENTA = 0;
                MagentaSlider.Value = 0;
                return;
            }
            MagentaSlider.Value = value;
            MAGENTA = value;
            ConvertCMYK_TO_RGB_andFill();
        }

        private void YellowTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (isHandled)
                return;
            byte value;
            if (!byte.TryParse((sender as TextBox).Text, out value) || value > 100)
            {
                YELLOW = 0;
                YellowSlider.Value = 0;
                return;
            }
            YellowSlider.Value = value;
            YELLOW = value;
            ConvertCMYK_TO_RGB_andFill();
        }

        private void ConvertCMYK_TO_RGB_andFill()
        {
            isHandled = true;
            double cyan = CYAN / 100.0;
            double black = BLACK / 100.0;
            double magenta = MAGENTA / 100.0;
            double yellow = YELLOW / 100.0;

            byte _R = (byte)((1 - Math.Min(1, cyan * (1 - black) + black)) * 255);
            byte _G = (byte)((1 - Math.Min(1, magenta * (1 - black) + black)) * 255);
            byte _B = (byte)((1 - Math.Min(1, yellow * (1 - black) + black)) * 255);

            CMYKRectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_R, _G, _B));

            if (connect)
            {
                R = _R;
                G = _G;
                B = _B;
                RSlider.Value = R;
                GSlider.Value = G;
                BSlider.Value = B;
                RTextBox.Text = R.ToString();
                GTextBox.Text = G.ToString();
                BTextBox.Text = B.ToString();
                RGBRectangle.Fill = new SolidColorBrush(System.Windows.Media.Color.FromRgb(_R, _G, _B));
            }
            isHandled = false;
        }

        private void ColorRGBCube()
        {
            Bitmap bitmap = new Bitmap(256, 256);
            for (byte i = 0; i < 255; i++)
                for (byte j = 0; j < 255; j++)
                {
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb((byte)(255 - i), 0, (byte)(255 - j)));
                }

            BitmapImage image = BitmapConverter.Bitmap2BitmapImage(bitmap);
            ImageBrush brush = new ImageBrush(image);
            FirstWall.Material = new DiffuseMaterial(brush);
            FirstWall.BackMaterial = new DiffuseMaterial(brush);

            bitmap = new Bitmap(256, 256);
            for (byte i = 0; i < 255; i++)
                for (byte j = 0; j < 255; j++)
                {
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb((byte)(255 - i), (byte)(255 - j), 0));
                }

            image = BitmapConverter.Bitmap2BitmapImage(bitmap);
            brush = new ImageBrush(image);
            SecondWall.Material = new DiffuseMaterial(brush);
            SecondWall.BackMaterial = new DiffuseMaterial(brush);

            bitmap = new Bitmap(256, 256);
            for (byte i = 0; i < 255; i++)
                for (byte j = 0; j < 255; j++)
                {
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(0, j, i));
                }

            image = BitmapConverter.Bitmap2BitmapImage(bitmap);
            brush = new ImageBrush(image);
            DownWall.Material = new DiffuseMaterial(brush);
            DownWall.BackMaterial = new DiffuseMaterial(brush);

            bitmap = new Bitmap(256, 256);
            for (byte i = 0; i < 255; i++)
                for (byte j = 0; j < 255; j++)
                {
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(255, j, i));
                }

            image = BitmapConverter.Bitmap2BitmapImage(bitmap);
            brush = new ImageBrush(image);
            UpWall.Material = new DiffuseMaterial(brush);
            UpWall.BackMaterial = new DiffuseMaterial(brush);

            bitmap = new Bitmap(256, 256);
            for (byte i = 0; i < 255; i++)
                for (byte j = 0; j < 255; j++)
                {
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(255 - i, 255, 255 - j));
                }

            image = BitmapConverter.Bitmap2BitmapImage(bitmap);
            brush = new ImageBrush(image);
            ThirdWall.Material = new DiffuseMaterial(brush);
            ThirdWall.BackMaterial = new DiffuseMaterial(brush);

            bitmap = new Bitmap(256, 256);
            for (byte i = 0; i < 255; i++)
                for (byte j = 0; j < 255; j++)
                {
                    bitmap.SetPixel(i, j, System.Drawing.Color.FromArgb(i, j, 255));
                }

            image = BitmapConverter.Bitmap2BitmapImage(bitmap);
            brush = new ImageBrush(image);
            FourthWall.Material = new DiffuseMaterial(brush);
            FourthWall.BackMaterial = new DiffuseMaterial(brush);

        }
    }
}
