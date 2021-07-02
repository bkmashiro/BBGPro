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
    public class ImageService
    {
        string imagePath;
        Bitmap bitmap;
        byte[,,] imgByte;
        public bool IsImgLoaded = false;
        public bool UseDither = true;
        public int ditherType = -1;
        public void LoadImage(string path)
        {
            try
            {
                imagePath = path;
                bitmap = new Bitmap(path);
                IsImgLoaded = true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"图像读取错误：{ex.Message}");
                throw;
            }

        }
        public void LoadImage(Bitmap bmp)
        {
            bitmap = new Bitmap(bmp);
            IsImgLoaded = true;
        }
        public byte[,,] GetImageArray()
        {
            if (File.Exists(imagePath))
            {
                if (UseDither)
                {
                    System.Drawing.Bitmap ditheredBmp = new System.Drawing.Bitmap(bitmap.Width,bitmap.Height);
                    DitherService ditherService = new DitherService();
                    switch (ditherType)
                    {
                        case -1: ditheredBmp = bitmap; break;
                        case 1: ditheredBmp = ditherService.DoBurkesDithering(bitmap); break;
                        case 2: ditheredBmp = ditherService.FakeDithering(bitmap); break;
                        case 3: ditheredBmp = ditherService.FloydSteinbergDithering(bitmap); break;
                        case 4: ditheredBmp = ditherService.JarvisJudiceNinkeDithering(bitmap); break;
                        case 5: ditheredBmp = ditherService.SierraDithering(bitmap); break;
                        case 6: ditheredBmp = ditherService.SierraLiteDithering(bitmap); break;
                        case 7: ditheredBmp = ditherService.SierraTwoRowDithering(bitmap); break;
                        case 8: ditheredBmp = ditherService.StuckiDithering(bitmap); break;

                        default:
                            ditheredBmp = ditherService.DoAtkinsonDithering(bitmap);
                            break;
                    }
                    imgByte = new byte[bitmap.Width, bitmap.Height, 3];
                    GetRGBArray(ditheredBmp, 0, 0, bitmap.Width, bitmap.Height, imgByte, 0, bitmap.Width);
                    return imgByte;
                }
                else
                {
                    imgByte = new byte[bitmap.Width, bitmap.Height, 3];
                    GetRGBArray(bitmap, 0, 0, bitmap.Width, bitmap.Height, imgByte, 0, bitmap.Width);
                    return imgByte;
                }
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
