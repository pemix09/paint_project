using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace paint_project
{
    /// <summary>
    /// Interaction logic for Filter.xaml
    /// </summary>
    public partial class Filters : Window
    {
        public Filters()
        {
            InitializeComponent();
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                Orginal.Source = new BitmapImage(new Uri(openFileDialog.FileName));
                AfterFiltering.Source = Orginal.Source;
            }
        }

        private void Add(object sender, RoutedEventArgs e)
        {
            byte value;
            if (!byte.TryParse(Adding.Text, out value))
            {
                MessageBox.Show("Bad value, can range from 0 to 255");
                Adding.Text = "0";
                return;
            }
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.Add(bitmap, value));
        }

        private void Subtract(object sender, RoutedEventArgs e)
        {
            byte value;
            if (!byte.TryParse(Subtracting.Text, out value))
            {
                MessageBox.Show("Bad value, can range from 0 to 255");
                Subtracting.Text = "0";
                return;
            }
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.Subtract(bitmap, value));
        }

        private void Multiply(object sender, RoutedEventArgs e)
        {
            double value;
            if (!double.TryParse(Multipling.Text, out value) || value > 255.0)
            {
                MessageBox.Show("Bad value, can range from 0 to 255");
                Multipling.Text = "0";
                return;
            }
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.Multiply(bitmap, value));
        }

        private void Divide(object sender, RoutedEventArgs e)
        {
            double value;
            if (!double.TryParse(Dividing.Text, out value) || value > 255.0 || value == 0.0)
            {
                MessageBox.Show("Bad value");
                Dividing.Text = "0";
                return;
            }
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.Divide(bitmap, value));
        }

        private void Brightness(object sender, RoutedEventArgs e)
        {
            double value;

            if (BrightnessChanging.Text.Contains('.'))
            {
                BrightnessChanging.Text = BrightnessChanging.Text.Replace('.', ',');
            }

            if (!double.TryParse(BrightnessChanging.Text, out value))
            {
                MessageBox.Show("Bad value");
                BrightnessChanging.Text = "100";
                return;
            }
            else if(value < 1 || value > 100)
            {
                MessageBox.Show("Value can range only from 1 to 100!");
                return;
            }
            
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.ChangeBrightness(bitmap, value));
        }

        private void ConvertToGrayScaleAverage(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.ConvertToGrayScaleAverage(bitmap));
        }

        private void ConvertToGrayScaleYUV(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.ConvertToGrayScaleYUV(bitmap));
        }

        private void AverageFilter(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.AverageFilter(bitmap));
        }
        private BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        private void Slider1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Orginal.LayoutTransform = new ScaleTransform(e.NewValue, e.NewValue);
        }

        private void Slider2_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            AfterFiltering.LayoutTransform = new ScaleTransform(e.NewValue, e.NewValue);
        }

        private void MedianFilter(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.MedianFilter(bitmap));
        }

        private void Sobel(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.SobelFilter(bitmap, SobelFilterVariantEnum.XY));
        }

        private void HighPassFilter(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.HighPassFilter(bitmap));
        }

        private void GausianFilter(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = BitmapConverter2.GetBitmapFromBitmapImage(image);
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitmapConverter2.GausianFilter(bitmap));
        }
    }
}
