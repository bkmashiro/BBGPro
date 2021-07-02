using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBG
{
    public class DitherService
    {
        public Bitmap DoAtkinsonDithering(Bitmap b)
        {
            AtkinsonDitheringRGBByte atkinson = new AtkinsonDitheringRGBByte(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        public Bitmap DoBurkesDithering(Bitmap b)
        {
            BurkesDitheringRGBByte atkinson = new BurkesDitheringRGBByte(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        public Bitmap FakeDithering(Bitmap b)
        {
            FakeDithering atkinson = new FakeDithering(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        public Bitmap FloydSteinbergDithering(Bitmap b)
        {
            FloydSteinbergDithering atkinson = new FloydSteinbergDithering(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        public Bitmap JarvisJudiceNinkeDithering(Bitmap b)
        {
            JarvisJudiceNinkeDithering atkinson = new JarvisJudiceNinkeDithering(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        public Bitmap SierraDithering(Bitmap b)
        {
            SierraDithering atkinson = new SierraDithering(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        public Bitmap SierraLiteDithering(Bitmap b)
        {
            SierraLiteDithering atkinson = new SierraLiteDithering(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        public Bitmap SierraTwoRowDithering(Bitmap b)
        {
            SierraTwoRowDithering atkinson = new SierraTwoRowDithering(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        public Bitmap StuckiDithering(Bitmap b)
        {
            StuckiDithering atkinson = new StuckiDithering(TrueColorBytesToWebSafeColorBytes);
            using (var image = new Bitmap(b))
            {
                byte[,,] bytes = ReadBitmapToColorBytes(image);
                TempByteImageFormat temp = new TempByteImageFormat(bytes);
                temp = (TempByteImageFormat)atkinson.DoDithering(temp);
                WriteToBitmap(image, temp.GetPixelChannels);
                return image.Clone(new Rectangle(new Point(0, 0), new Size(image.Size.Width, image.Size.Height)), System.Drawing.Imaging.PixelFormat.Format16bppRgb565);
            }
        }
        private static object[] TrueColorBytesToWebSafeColorBytes(object[] input)
        {
            object[] returnArray = new object[input.Length];
            for (int i = 0; i < returnArray.Length; i++)
            {
                returnArray[i] = (byte)(Math.Round((byte)input[i] / 51.0) * 51);
            }

            return returnArray;
        }

        private static byte[,,] ReadBitmapToColorBytes(Bitmap bitmap)
        {
            byte[,,] returnValue = new byte[bitmap.Width, bitmap.Height, 3];
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    returnValue[x, y, 0] = color.R;
                    returnValue[x, y, 1] = color.G;
                    returnValue[x, y, 2] = color.B;
                }
            }
            return returnValue;
        }
        private static void WriteToBitmap(Bitmap bitmap, Func<int, int, object[]> reader)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    object[] read = reader(x, y);
                    Color color = Color.FromArgb((byte)read[0], (byte)read[1], (byte)read[2]);
                    bitmap.SetPixel(x, y, color);
                }
            }
        }
    }
}
