using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace BBG
{
    class ImageService
    {
        string imagePath;
        Bitmap bitmap;
        byte[,,] imgByte;
        public void LoadImage(string path)
        {
            imagePath = path;
            bitmap = new Bitmap(path);
        }
        public void LoadImage(Bitmap bmp)
        {
            bitmap = new Bitmap(bmp);
        }
        public byte[,,] GetImageArray()
        {
            if (File.Exists(imagePath))
            {
                imgByte = new byte[bitmap.Width, bitmap.Height, 3];
                GetRGBArray(bitmap, 0, 0, bitmap.Width, bitmap.Height, imgByte, 0, bitmap.Width);
                return imgByte;
            }
            else
            {
                return null;
                throw new ArgumentNullException("image is null");
            }
        }
        public void GetRGBArray(Bitmap image, int startX, int startY, int w, int h, byte[,,] rgbArray, int offset, int scansize)
        {
            const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format24bppRgb;

            if (image == null) throw new ArgumentNullException("image");
            if (rgbArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            if (h < 0 || (rgbArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);
            try
            {
                byte[] pixelData = new Byte[data.Stride];
                for (int scanline = 0; scanline < data.Height; scanline++)
                {
                    Marshal.Copy(data.Scan0 + (scanline * data.Stride), pixelData, 0, data.Stride);
                    for (int pixeloffset = 0; pixeloffset < data.Width; pixeloffset++)
                    {
                        rgbArray[pixeloffset, scanline, 0] = ((byte)(pixelData[pixeloffset * PixelWidth + 2]));
                        rgbArray[pixeloffset, scanline, 1] = ((byte)(pixelData[pixeloffset * PixelWidth + 1]));
                        rgbArray[pixeloffset, scanline, 2] = ((byte)(pixelData[pixeloffset * PixelWidth]));
                    }
                }
            }
            finally
            {
                image.UnlockBits(data);
            }
        }

    }
}
