using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace paint_project
{
    class FileReader
    {
        public BitmapImage ReadImageFIle()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image files (*.p3; *p6; *jpg)|*.jpg;*.p3;*.p6";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath);

                if(fileExtension == ".jpg")
                {
                    return ReadJPG(filePath);
                }
                else if(fileExtension == ".p3")
                {
                    return ReadP3(filePath);
                }
                else if(fileExtension == ".p6")
                {
                    return ReadP6(filePath);
                }
                else
                {
                    throw new Exception("Given image is not supported!");
                }
            }
            return null;
        }

        private BitmapImage ReadJPG(string _filePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(_filePath);
            bitmap.EndInit();

            return bitmap;
        }

        private BitmapImage ReadP3(string _filePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(_filePath);
            bitmap.EndInit();

            return bitmap;
        }

        private BitmapImage ReadP6(string _filePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(_filePath);
            bitmap.EndInit();

            return bitmap;
        }
    }
}
