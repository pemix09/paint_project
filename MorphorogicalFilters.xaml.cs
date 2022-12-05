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
using System.Windows.Shapes;
using static paint_project.BitMapFilter;


namespace paint_project
{
    /// <summary>
    /// Logika interakcji dla klasy MorphorogicalFilters.xaml
    /// </summary>
    public partial class MorphorogicalFilters : Window
    {
        public MorphorogicalFilters()
        {
            InitializeComponent();
        }

        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif|PNG Image|*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                var bitmapImage = new BitmapImage(new Uri(openFileDialog.FileName));
                var bitmap = BitMapFilter.ThresholdByValue(GetBitmapFromBitmapImage(bitmapImage), 123);
                Orginal.Source = ConvertBitmapToBitmapImage(bitmap);
                AfterFiltering.Source = Orginal.Source;
            }
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

        public static Bitmap GetBitmapFromBitmapImage(BitmapImage image)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(image));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
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

        private void Handle_Dilation(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = GetBitmapFromBitmapImage(image);
            Scale2.Value = 1;
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitMapFilter.Dilation(bitmap));
        }

        private void Handle_Erosion(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = GetBitmapFromBitmapImage(image);
            Scale2.Value = 1;
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitMapFilter.Erosion(bitmap));
        }

        private void Handle_Opening(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = GetBitmapFromBitmapImage(image);
            Scale2.Value = 1;
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitMapFilter.Opening(bitmap));
        }

        private void Handle_Closing(object sender, RoutedEventArgs e)
        {
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = GetBitmapFromBitmapImage(image);
            Scale2.Value = 1;
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitMapFilter.GetClosing(bitmap));
        }

        private void Handle_HitOrMiss(object sender, RoutedEventArgs e)
        {
            byte[,] mask;
            if (!GetMask(out mask))
            {
                MessageBox.Show("podano zle wartosci");
                return;
            }
            BitmapImage image = Orginal.Source as BitmapImage;
            Bitmap bitmap = GetBitmapFromBitmapImage(image);
            Scale2.Value = 1;
            AfterFiltering.Source = ConvertBitmapToBitmapImage(BitMapFilter.HitOrMiss(bitmap, mask));
        }

        private bool GetMask(out byte[,] mask)
        {
            mask = new byte[3, 3];
            foreach (var tb in FindVisualChildren<TextBox>(this).Where(x => x.Name.StartsWith("Mask")))
            {
                int x = int.Parse(tb.Name[5].ToString());
                int y = int.Parse(tb.Name[7].ToString());
                if (string.IsNullOrEmpty(tb.Text))
                    mask[x, y] = 2; //bit nieznaczacy
                else
                {
                    byte val;
                    if (!byte.TryParse(tb.Text, out val))
                        return false;
                    if (val != 0 && val != 1)
                        return false;
                    mask[x, y] = val;
                }
            }
            return true;
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    var child = VisualTreeHelper.GetChild(depObj, i);

                    if (child != null && child is T)
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                        yield return childOfChild;
                }
            }
        }
    }
}
