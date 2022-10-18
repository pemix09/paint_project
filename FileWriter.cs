using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace paint_project
{
    class FileWriter
    {
        ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
        Encoder myEncoder = Encoder.Quality;
        EncoderParameter myEncoderParameter;
        EncoderParameters myEncoderParameters = new EncoderParameters();

        public void SaveImage(BitmapImage _image, double _quality)
        {
            long quality = Convert.ToInt64(_quality);
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "JPEG kurde faja (*jpeg)|*.jpeg";
            if (dialog.ShowDialog() == true)
            {
                myEncoderParameter = new EncoderParameter(myEncoder, quality);
                myEncoderParameters.Param[0] = myEncoderParameter;
                Bitmap file = BitmapImage2Bitmap(_image);
                file.Save(dialog.FileName, myImageCodecInfo, myEncoderParameters);
            } 
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
