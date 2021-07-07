using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BBG
{
    public class ImgPreview
    {
        BlockInfoManager BlockInfoManager;
        int width = 1;
        int height = 1;
        byte[,] result;
        ProgressBar ProgressBar;
        TextBlock TextBlock;

        public void Init(BlockInfoManager b, int w, int h, byte[,] byteResult)
        {
            BlockInfoManager = b;
            result = byteResult;
            width = w;
            height = h;
            //生成交叉数组
            blockCssByte = new byte[BlockInfoManager.colorEnabled.Length][];

            for (int j = 0; j < BlockInfoManager.colorEnabled.Length; j++)
            {
                if (BlockInfoManager.colorEnabled[j])
                {
                    blockCssByte[j] = new byte[16 * 16 * 3];
                    getBGRArrayForBlockBmp(new Bitmap(System.AppDomain.CurrentDomain.BaseDirectory + @"blockdata/" + BlockInfoManager.blockDatas_higher[j].image[BlockInfoManager.demoBlockIndex[j]]), 0, 0, 16, 16, blockCssByte[j], 0, 16);
                }
            }
        }

        public void BindProgress(ProgressBar pg, TextBlock tb)
        {
            ProgressBar = pg;
            TextBlock = tb;
        }
        int workMax;
        private void SetMaxProgress(int max)
        {
            workMax = max;
            ProgressBar.Dispatcher.Invoke(new Action(() =>
            {
                ProgressBar.Maximum = max;
            }));
            TextBlock.Dispatcher.Invoke(new Action(() =>
            {
                TextBlock.Text = string.Format(Application.Current.FindResource("preview_progress_txt").ToString(), Application.Current.FindResource("preview_progress_buffering").ToString());
            }));
        }

        private void UpdateProgress(int i)
        {
            ProgressBar.Dispatcher.Invoke(new Action(() =>
            {
                ProgressBar.Value = i;
            }));
            TextBlock.Dispatcher.Invoke(new Action(() =>
            {
                TextBlock.Text = string.Format(Application.Current.FindResource("preview_progress_copy").ToString(), (i / (float)workMax * 100).ToString("#0.0"));
            }));
        }
        //存储方块css的交叉数组
        byte[][] blockCssByte;

        public Bitmap GetBitamp()
        {
            if (BlockInfoManager == null)
            {
                return null;
            }
            //创建一维图像缓存，按照BGR存入
            byte[] buffer = new byte[width * height * 16 * 16 * 3];
            byte blockIndex = 0;
            int width2 = result.GetLength(0) * 16 * 3;
            //生成buffer
            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int z = 0; z < result.GetLength(1); z++)
                {
                    blockIndex = result[x, z];
                    for (int yy = 0; yy < 16; yy++)
                    {
                        Buffer.BlockCopy(blockCssByte[blockIndex], 48 * yy, buffer, (yy + z * 16) * width2 + x * 48, 48);
                    }
                }
            }

            SetMaxProgress(height * 16);

            return MySaveBMP(buffer, width * 16, height * 16);
        }
        public Bitmap MySaveBMP(byte[] buffer, int width, int height)
        {
            Bitmap b = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            Rectangle BoundsRect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = b.LockBits(BoundsRect,
                                            ImageLockMode.WriteOnly,
                                            b.PixelFormat);

            IntPtr ptr = bmpData.Scan0;

            // add back dummy bytes between lines, make each line be a multiple of 4 bytes
            int skipByte = bmpData.Stride - width * 3;
            byte[] newBuff = new byte[buffer.Length + skipByte * height];
            for (int j = 0; j < height; j++)
            {

                Buffer.BlockCopy(buffer, j * width * 3, newBuff, j * (width * 3 + skipByte), width * 3);
                UpdateProgress(j);
                //Console.WriteLine($"From{j * width * 3}to{j * (width * 3 + skipByte)}");
            }

            // fill in rgbValues
            Marshal.Copy(newBuff, 0, ptr, newBuff.Length);
            b.UnlockBits(bmpData);
            // b.Save(@"transformed.bmp", ImageFormat.Bmp);
            return b;
        }
        public void getBGRArrayForBlockBmp(Bitmap image, int startX, int startY, int w, int h, byte[] bgrArray, int offset, int scansize)
        {
            //const int PixelWidth = 3;
            const PixelFormat PixelFormat = PixelFormat.Format24bppRgb;

            if (image == null) throw new ArgumentNullException("image");
            if (bgrArray == null) throw new ArgumentNullException("rgbArray");
            if (startX < 0 || startX + w > image.Width) throw new ArgumentOutOfRangeException("startX");
            if (startY < 0 || startY + h > image.Height) throw new ArgumentOutOfRangeException("startY");
            if (w < 0 || w > scansize || w > image.Width) throw new ArgumentOutOfRangeException("w");
            if (h < 0 || (bgrArray.Length < offset + h * scansize) || h > image.Height) throw new ArgumentOutOfRangeException("h");

            BitmapData data = image.LockBits(new Rectangle(startX, startY, w, h), System.Drawing.Imaging.ImageLockMode.ReadOnly, PixelFormat);
            try
            {
                int pointer = 0;
                for (int scanline = 0; scanline < data.Height; scanline++)
                {
                    Marshal.Copy(data.Scan0 + (scanline * data.Stride), bgrArray, pointer, data.Stride);
                    pointer += data.Stride;
                }

            }
            finally
            {
                image.UnlockBits(data);
            }
        }

        public Bitmap GetMap(byte[,] h = null)
        {
            byte[][][] colorBuffer = new byte[3][][]; //3个高度
            colorBuffer[1] = new byte[BlockInfoManager.colorEnabled.Length][];//length个方块
            for (int i = 0; i < BlockInfoManager.colorEnabled.Length; i++)
            {
                if (BlockInfoManager.colorEnabled[i])
                {
                    colorBuffer[1][i] = new byte[3];
                    colorBuffer[1][i][0] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().B;
                    colorBuffer[1][i][1] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().G;
                    colorBuffer[1][i][2] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().R;
                }
            }

            byte[] buffer = new byte[width * height * 3];

            if (h == null)
            {
                //2D
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            buffer[x * height * 3 + y * 3 + channel] = colorBuffer[1][result[y, x]][channel];
                        }
                    }
                }

            }
            else
            {
                //3D
                //更高
                for (int i = 0; i < BlockInfoManager.colorEnabled.Length; i++)
                {
                    if (BlockInfoManager.colorEnabled[i])
                    {
                        colorBuffer[2][i] = new byte[3];
                        colorBuffer[2][i][0] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().B;
                        colorBuffer[2][i][1] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().G;
                        colorBuffer[2][i][2] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().R;
                    }
                }
                //更低
                for (int i = 0; i < BlockInfoManager.colorEnabled.Length; i++)
                {
                    if (BlockInfoManager.colorEnabled[i])
                    {
                        colorBuffer[0][i] = new byte[3];
                        colorBuffer[0][i][0] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().B;
                        colorBuffer[0][i][1] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().G;
                        colorBuffer[0][i][2] = BlockInfoManager.blockDatas_flat[i].RGBColor.ToColor().R;
                    }
                }

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int channel = 0; channel < 3; channel++)
                        {
                            buffer[x * height * 3 + height * 3 + channel] = colorBuffer[h[x,y]][result[x, y]][channel];
                        }
                    }
                }
            }

            return MySaveBMP(buffer, width, height);
        }
    }
}
