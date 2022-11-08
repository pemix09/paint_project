
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace paint_project
{
    public static class BitmapConverter2
    {
        private static Bitmap processPixels(Bitmap bitmap, Func<Color, Color> func)
        {
            Bitmap newBitmap = new Bitmap(bitmap);
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    var color = bitmap.GetPixel(i, j);
                    newBitmap.SetPixel(i, j, func(color));
                }
            }

            return newBitmap;
        }

        public static Bitmap Add(Bitmap bitmap, byte value)
        {
            return processPixels(bitmap, color =>
                Color.FromArgb(
                    Math.Min(color.R + value, 255),
                    Math.Min(color.G + value, 255),
                    Math.Min(color.B + value, 255)));
        }

        public static Bitmap Subtract(Bitmap bitmap, byte value)
        {
            return processPixels(bitmap, color =>
                Color.FromArgb(
                    Math.Max(color.R - value, 0),
                    Math.Max(color.G - value, 0),
                    Math.Max(color.B - value, 0)));
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

        public static Bitmap Multiply(Bitmap bitmap, double value)
        {
            return processPixels(bitmap, color =>
               Color.FromArgb(
                   Math.Min((int)(color.R * value), 255),
                   Math.Min((int)(color.G * value), 255),
                   Math.Min((int)(color.B * value), 255)));
        }

        public static Bitmap Divide(Bitmap bitmap, double value)
        {
            return processPixels(bitmap, color =>
                Color.FromArgb(
                    Math.Max((int)(color.R / value), 0),
                    Math.Max((int)(color.G / value), 0),
                    Math.Max((int)(color.B / value), 0)));
        }

        public static Bitmap ChangeBrightness(Bitmap bitmap, double valueInProcent)
        {
            double value = valueInProcent / 100;
                
            return processPixels(bitmap, color =>
                 Color.FromArgb(
                    Math.Min((int)(color.R * value), 255),
                    Math.Min((int)(color.G * value), 255),
                    Math.Min((int)(color.B * value), 255)));
        }

        public static Bitmap ConvertToGrayScaleAverage(Bitmap bitmap)
        {
            return processPixels(bitmap, color =>
            {
                var average = (color.R + color.G + color.B) / 3;
                return Color.FromArgb(average, average, average);
            });

        }

        /// <summary>
        ///     Y - birghtness
        ///     U = CB = blueness of the pixel
        ///     V = CR = redness of the pixel
        ///     If there are not that much of red and blue on the picture, it becomes green
        ///     We use it beacause color information is separated from grey scale information
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static Bitmap ConvertToGrayScaleYUV(Bitmap bitmap)
        {
            return processPixels(bitmap, color =>
            {
                var value = (byte)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B);
                return Color.FromArgb(value, value, value);
            });

        }

        public static Bitmap AverageFilter(Bitmap bitmap)
        {
            double[,] mask = new double[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    mask[i, j] = 1.0 / 9.0;
                }
            }

            var colors = getColors(bitmap);
            Bitmap newBitmap = new Bitmap(bitmap);

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    newBitmap.SetPixel(i, j, GetColorValueLinear(i, j, mask, colors));
                }
            }
            return newBitmap;
        }

        /// <summary>
        ///     Returns linear value of color, made with mask
        /// </summary>
        /// <param name="x">width of the mask</param>
        /// <param name="y">High of the mask</param>
        /// <param name="mask">Mask's value</param>
        /// <param name="colors">Colors of the pixel</param>
        /// <returns></returns>
        private static Color GetColorValueLinear(int x, int y, double[,] mask, Color[,] colors)
        {
            var R = 0;
            var G = 0;
            var B = 0;
            var maskI = 0;
            var maskJ = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                maskJ = 0;
                for (int j = y - 1; j <= y + 1; j++)
                {
                    //Every color is set accordingly to mask's value
                    R += (int)(colors[i, j].R * mask[maskI, maskJ]);
                    G += (int)(colors[i, j].G * mask[maskI, maskJ]);
                    B += (int)(colors[i, j].B * mask[maskI, maskJ]);
                    maskJ++;
                }
                maskI++;
            }
            //we have to make sure, if we're not out of range
            //for values smaller than 0 set 0 , for value bigger than 255 set 255
            //otherwise leave it as it is, % notation wouldn't work ex. 256%255 == 1
            R = R < 0 ? 0 : R > 255 ? 255 : R;
            G = G < 0 ? 0 : G > 255 ? 255 : G;
            B = B < 0 ? 0 : B > 255 ? 255 : B;
            return Color.FromArgb(R, G, B);
        }

        /// <summary>
        ///     Returns colors of the mask with alpha values,
        ///     it's made to not use bitmap but array of colors
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private static Color[,] getColors(Bitmap bitmap)
        {
            Color[,] colors = new Color[bitmap.Width, bitmap.Height];
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    colors[i, j] = bitmap.GetPixel(i, j);
                }
            }

            return colors;
        }

        public static Bitmap MedianFilter(Bitmap bitmap)
        {
            var colors = getColors(bitmap);
            Bitmap newBitmap = new Bitmap(bitmap);

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    newBitmap.SetPixel(i, j, GetColorValueMedian(i, j, colors));
                }
            }
            return newBitmap;
        }

        /// <summary>
        ///  Returns median value of colors in 3x3 mask
        /// </summary>
        /// <param name="x">x coordinte of pixel</param>
        /// <param name="y">y coordinate of pixel</param>
        /// <param name="colors">Colors array</param>
        /// <returns></returns>
        private static Color GetColorValueMedian(int x, int y, Color[,] colors)
        {
            int[] reds = new int[9];
            int[] greens = new int[9];
            int[] blues = new int[9];
            int iterator = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    reds[iterator] = colors[i, j].R;
                    greens[iterator] = colors[i, j].G;
                    blues[iterator] = colors[i, j].B;
                    iterator++;
                }
            }
            Array.Sort(reds);
            Array.Sort(greens);
            Array.Sort(blues);
            return Color.FromArgb(reds[5], greens[5], blues[5]);
        }

        /// <summary>
        ///     Detecs edges - when there is sudden change of colors, we can have X, Y, and XY wariant, which means 
        ///     there will be other masksW
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Bitmap SobelFilter(Bitmap bitmap)
        {
            var colors = getColors(bitmap);
            Bitmap newBitmap = new Bitmap(bitmap);
            double[,] mask = null;

            mask = GetSobelMask(SobelFilterVariantEnum.X);
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    newBitmap.SetPixel(i, j, GetColorValueLinear(i, j, mask, colors));
                }
            }

            colors = getColors(newBitmap);
            newBitmap = new Bitmap(newBitmap);
            mask = GetSobelMask(SobelFilterVariantEnum.Y);
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    newBitmap.SetPixel(i, j, GetColorValueLinear(i, j, mask, colors));
                }
            }
            return newBitmap;
        }

        internal static Bitmap HighPassFilter(Bitmap bitmap)
        {
            double[,] mask = new double[3, 3];

            mask[0, 0] = -1.0;
            mask[1, 0] = -1.0;
            mask[2, 0] = -1.0;
            mask[0, 1] = -1.0;
            mask[1, 1] = 9.0;
            mask[2, 1] = -1.0;
            mask[0, 2] = -1.0;
            mask[1, 2] = -1.0;
            mask[2, 2] = -1.0;

            var colors = getColors(bitmap);
            Bitmap newBitmap = new Bitmap(bitmap);

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    newBitmap.SetPixel(i, j, GetColorValueLinear(i, j, mask, colors));
                }
            }
            return newBitmap;
        }

        public static Bitmap GausianFilter(Bitmap bitmap)
        {
            double[,] mask = new double[3, 3];

            mask[0, 0] = 1 / 16.0;
            mask[1, 0] = 1 / 8.0;
            mask[2, 0] = 1 / 16.0;
            mask[0, 1] = 1 / 8.0;
            mask[1, 1] = 1 / 4.0;
            mask[2, 1] = 1 / 8.0;
            mask[0, 2] = 1 / 16.0;
            mask[1, 2] = 1 / 8.0;
            mask[2, 2] = 1 / 16.0;

            var colors = getColors(bitmap);
            Bitmap newBitmap = new Bitmap(bitmap);

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    newBitmap.SetPixel(i, j, GetColorValueLinear(i, j, mask, colors));
                }
            }
            return newBitmap;
        }

        private static double[,] GetSobelMask(SobelFilterVariantEnum x)
        {
            double[,] mask = new double[3, 3];
            switch (x)
            {
                case SobelFilterVariantEnum.X:
                    mask[0, 0] = -1.0;
                    mask[1, 0] = -2.0;
                    mask[2, 0] = -1.0;
                    mask[0, 1] = 0.0;
                    mask[1, 1] = 0.0;
                    mask[2, 1] = 0.0;
                    mask[0, 2] = 1.0;
                    mask[1, 2] = 2.0;
                    mask[2, 2] = 1.0;
                    return mask;
                case SobelFilterVariantEnum.Y:
                    mask[0, 0] = -1.0;
                    mask[0, 1] = -2.0;
                    mask[0, 2] = -1.0;
                    mask[1, 0] = 0.0;
                    mask[1, 1] = 0.0;
                    mask[1, 2] = 0.0;
                    mask[2, 0] = 1.0;
                    mask[2, 1] = 2.0;
                    mask[2, 2] = 1.0;
                    return mask;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
