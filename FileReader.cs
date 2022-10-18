using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace paint_project
{
    public class FileReader
    {
        public BitmapImage ReadImageFIle()
        {
            var fileContent = string.Empty;
            var filePath = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Image files (*.p3; *p6; *jpg)|*.jpg;*.p3;*.p6 | All files (*.*)|*.*";
            openFileDialog.FilterIndex = 2;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
                string fileExtension = Path.GetExtension(filePath);

                if(fileExtension == ".jpg" || fileExtension == ".jpeg")
                {
                    return ReadJPG(filePath);
                }
                else if(fileExtension == ".ppm")
                {
                    return ReadPPM(filePath);
                }
                else
                {
                    throw new Exception("Given image format is not supported!");
                }
            }
            throw new Exception("User clicked cancel button");
        }

        private BitmapImage ReadJPG(string _filePath)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(_filePath);
            bitmap.EndInit();

            return bitmap;
        }

        private BitmapImage ReadP3(string imagePath)
        {
            FileStream file = new FileStream(imagePath, FileMode.Open);
            Bitmap image = null;
            int w;
            int h;
            int maxColor;
            double multiplier;
            image = Setup(file, out w, out h, out maxColor, out multiplier);
            int x = 0, y = 0;
            int[] colors = new int[3];
            int modulo = 0;
            var bufferSize = 1024 * 5000;
            var buffer = new byte[bufferSize];

            StringBuilder s = new StringBuilder();
            while (true)
            {
                var i = file.Read(buffer, 0, bufferSize);
                if (i == 0)
                    break;

                bool comment = false;
                for (int j = 0; j < i; j++)
                {
                    var b = buffer[j];
                    if (comment)
                    {
                        if (b == '\n')
                            comment = false;
                        continue;
                    }
                    if (b == '#')
                    {
                        comment = true;
                    }
                    else if (b != '\n' && b != ' ' && b != '\t')
                    {
                        s.Append((char)b);
                    }
                    else if (s.Length > 0)
                    {
                        var value = s.ToString();
                        s.Clear();
                        int d;
                        if (Int32.TryParse(value, out d))
                        {
                            colors[modulo] = (int)(d * multiplier);
                            modulo = (modulo + 1) % 3;
                            if (modulo == 0)
                            {
                                Color c = Color.FromArgb(colors[0], colors[1], colors[2]);
                                image.SetPixel(x, y, c);
                                x++;
                                if (x >= w)
                                {
                                    x = 0;
                                    y++;
                                }
                            }
                        }
                        else
                            throw new ArgumentException("bad value");
                    }
                }
            }
            return BitmapConverter.Bitmap2BitmapImage(image);
        }

        private BitmapImage ReadP6(string imagePath)
        {
            FileStream file = new FileStream(imagePath, FileMode.Open);
            Bitmap image = null;
            int w;
            int h;
            int maxColor;
            double multiplier;
            image = Setup(file, out w, out h, out maxColor, out multiplier);
            int x = 0, y = 0;
            int[] colors = new int[3];
            int modulo = 0;
            var bufferSize = 1024 * 5000;
            var buffer = new byte[bufferSize];

            while (true)
            {
                var i = file.Read(buffer, 0, bufferSize);
                if (i == 0)
                    break;

                for (int j = 0; j < i; j++)
                {
                    colors[modulo] = (int)(buffer[j] * multiplier);
                    modulo = (modulo + 1) % 3;
                    if (modulo == 0)
                    {
                        Color c = Color.FromArgb(colors[0], colors[1], colors[2]);
                        image.SetPixel(x, y, c);
                        x++;
                        if (x >= w)
                        {
                            x = 0;
                            y++;
                            if (y == h)
                                break;
                        }
                    }
                }
            }
            return BitmapConverter.Bitmap2BitmapImage(image);
        }

        private BitmapImage ReadPPM(string _filePath)
        {
            string[] image = System.IO.File.ReadAllLines(_filePath);
            string[] imageWithoutComments = image.Where(line => !line.StartsWith('#')).ToArray();

            string version = imageWithoutComments[0].Split()[0];

            if(version == "P3")
            {
                return ReadP3(_filePath);
            }
            else if(version == "P6")
            {
                return ReadP6(_filePath);
            }

            return null;
            
        }

        private Bitmap Setup(FileStream file, out int w, out int h, out int maxColor, out double multiplier)
        {
            //skip version
            file.ReadByte();
            file.ReadByte();
            file.ReadByte();

            List<char> tempData = new();
            bool setup = false;
            w = h = maxColor = 0;
            multiplier = 0.0;
            Bitmap image = null;
            while (!setup)
            {
                char temp = (char)file.ReadByte();
                if (temp == '#')
                {
                    while (temp != '\n')
                    {
                        temp = (char)file.ReadByte();
                    }
                }
                else if (temp != ' ' && temp != '\t' && temp != '\n')
                {
                    tempData.Add(temp);
                }
                else if (tempData.Count > 0)
                {
                    var valueString = GetStringFromEnumerable(tempData);
                    int value;
                    if (!int.TryParse(valueString, out value))
                    {
                        throw new ArgumentException("bad value");
                    }
                    if (w == 0)
                    {
                        w = value;
                    }
                    else if (h == 0)
                    {
                        h = value;
                        image = new Bitmap(w, h);
                    }
                    else if (maxColor == 0)
                    {
                        maxColor = value;
                        setup = true;
                        multiplier = (double)255 / maxColor;
                    }
                    tempData = new();
                }
            }

            return image;
        }

        private string GetStringFromEnumerable(IEnumerable<char> value)
        {
            StringBuilder builder = new();
            foreach (var c in value)
            {
                builder.Append(c);
            }
            return builder.ToString();
        }
    }
}
