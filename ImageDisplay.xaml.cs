using System;
using System.Collections.Generic;
using System.Drawing;
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
    /// Interaction logic for ImageDisplay.xaml
    /// </summary>
    public partial class ImageDisplay : Window
    {
        private System.Windows.Point origin;
        private System.Windows.Point start;
        private BitmapImage image;
        private bool loaded = false;
        public ImageDisplay(BitmapImage _image)
        {
            image = _image;
            InitializeComponent();
            ImageViewer.Source = image;
            loaded = true;
        }

        public void SaveImage(object sender, RoutedEventArgs e)
        {
            double quality = Quality.Value;
            FileWriter writer = new FileWriter();
            writer.SaveImage(image, quality);
        }

        public void SliderValueChanged(object sender, RoutedEventArgs e)
        {
            int quality = Convert.ToInt32(Quality.Value);
            QualityValueText.Text = $"Quality: {quality}";
        }


        public void ZoomValueChanged(object sender, RoutedEventArgs e)
        {
            if(loaded)
            {
                double zoom = ZoomSlider.Value;

                var matrix = Matrix.Identity;
                matrix.Scale(zoom, zoom);
                matrix.Translate(30, 60);
                ImageViewer.RenderTransform = new MatrixTransform(matrix);
                ZoomValue.Text = $"Zoom: {Convert.ToInt32(zoom)}";
            }
            
        }
    }
}
