using Colourful;
using Colourful.Conversion;
using Colourful.Difference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BBG
{
    public class MuteGenerator
    {
        /*-
         * 先Init
         * 然后调用generate
         * 使用GetResult获取输出
         * -*/

        delegate double errCalc<T, U>(T orginal, U sample);
        errCalc<RGBColor, ConciseBlockData2D> GetErr;
        errCalc<RGBColor, ConciseBlockData3D> GetErr2;
        public byte[,,] Mapping;//缓存
        public byte[,] result;
        public byte[,] height;
        int imgWidth;
        int imgLength;
        byte[,,] rgbArray;
        GenerateType generateType = GenerateType._2d;

        AffairHandler AffairHandler;
        public void MappingOverRide(byte[,,] b) => Mapping = b;

        public void Init(byte[,,] _rgbArray, bool useLab = true)
        {
            imgWidth = _rgbArray.GetLength(0);
            imgLength = _rgbArray.GetLength(1);
            nowProgress = 0;

            generateType = GenerateType._2d;
            if (Mapping == null)
            {
                Mapping = new byte[256, 256, 256];
            }
            result = new byte[imgWidth, imgLength];
            rgbArray = _rgbArray;
            if (useLab)
            {
                GetErr = LABDifference;
                GetErr2 = LABDifference;
            }
            else
            {
                GetErr = RGBDifference;
                GetErr2 = RGBDifference;
            }
        }
        public void Generate(ConciseBlockData2D[] blockDatas)
        {


            var task1 = new Task(() =>
            {
                int imgwidth = rgbArray.GetLength(0);
                int imgheight = rgbArray.GetLength(1);
                result = new byte[imgwidth, imgheight];
                double tmp_deltae = 0;
                byte tmp_id = 0;

                RGBColor tmpRGB;
                for (int x = 0; x < rgbArray.GetLength(0); x++)
                {
                    for (int y = 0; y < rgbArray.GetLength(1); y++)
                    {
                        byte cache = Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]];
                        if (cache != 0)
                        {//命中缓存
                            result[x, y] = cache;
                            continue;
                        }
                        else
                        {//未命中，计算最适合的方块 
                            double deltae = double.MaxValue;
                            tmpRGB = new RGBColor(System.Drawing.Color.FromArgb(rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]));
                            for (byte _id = 0; _id < blockDatas.Length; _id++)
                            {
                                tmp_deltae = GetErr(tmpRGB, blockDatas[_id]);
                                if (tmp_deltae < deltae)
                                {
                                    deltae = tmp_deltae;
                                    tmp_id = _id;
                                }
                            }
                            Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]] = (byte)(blockDatas[tmp_id].classId - 1);
                            result[x, y] = (byte)(blockDatas[tmp_id].classId - 1);
                        }
                    }
                }
                OnGenerateFinished();

            });
            task1.Start();
            task1.Wait();
        }
        public void Generate(ConciseBlockData3D[] blockDatas)
        {

            var task1 = new Task(() =>
            {
                if (Mapping == null)
                {
                    Mapping = new byte[256, 256, 256];
                }
                int imgwidth = rgbArray.GetLength(0);
                int imgheight = rgbArray.GetLength(1);
                height = new byte[imgheight, imgheight];
                result = new byte[imgwidth, imgheight];
                double tmp_deltae = 0;
                byte tmp_id = 0;

                RGBColor tmpRGB;
                for (int x = 0; x < rgbArray.GetLength(0); x++)
                {
                    for (int y = 0; y < rgbArray.GetLength(1); y++)
                    {
                        byte cache = Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]];
                        if (cache != 0)
                        {//命中缓存
                            result[x, y] = cache;
                            height[x, y] = (blockDatas[cache].height);
                            continue;
                        }
                        else
                        {//未命中，计算最适合方块
                            double deltae = double.MaxValue;
                            tmpRGB = new RGBColor(System.Drawing.Color.FromArgb(rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]));
                            for (byte _id = 0; _id < blockDatas.Length; _id++)
                            {
                                tmp_deltae = GetErr2(tmpRGB, blockDatas[_id]);
                                if (tmp_deltae < deltae)
                                {
                                    deltae = tmp_deltae;
                                    tmp_id = _id;
                                }
                            }
                            Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]] = (byte)(blockDatas[tmp_id].classId - 1);
                            result[x, y] = (byte)(blockDatas[tmp_id].classId - 1);
                            height[x, y] = (blockDatas[tmp_id].height);
                        }
                    }
                }
                OnGenerateFinished();
            });
            task1.Start();
            task1.Wait();

        }

        ConciseBlockData2D[] myblockDatas2D;
        ConciseBlockData3D[] myblockDatas3D;
        private readonly object locker = new object();
        public void Multithread_Generate(ConciseBlockData2D[] blockDatas)
        {
            myblockDatas2D = blockDatas;
            var task1 = new Task(() =>
            {
                int imgwidth = rgbArray.GetLength(0);
                int imgheight = rgbArray.GetLength(1);
                result = new byte[imgwidth, imgheight];

                ThreadPool.SetMaxThreads(16, 16);
                for (int x = 0; x < rgbArray.GetLength(0); x++)
                {
                    ThreadPool.QueueUserWorkItem(WorkALine2D, x);
                }
                lock (locker)
                {
                    while (nowProgress != imgwidth)
                    {
                        Monitor.Wait(locker);//等待
                    }
                }
            });
            task1.Start();
            task1.Wait();

        }
        public void Multithread_Generate(ConciseBlockData3D[] blockDatas)
        {
            myblockDatas3D = blockDatas;

            var task1 = new Task(() =>
            {
                int imgwidth = rgbArray.GetLength(0);
                int imgheight = rgbArray.GetLength(1);
                result = new byte[imgwidth, imgheight];
                height = new byte[imgheight, imgheight];

                ThreadPool.SetMaxThreads(16, 16);
                for (int x = 0; x < rgbArray.GetLength(0); x++)
                {
                    ThreadPool.QueueUserWorkItem(WorkALine3D, x);
                }
                lock (locker)
                {
                    while (nowProgress != imgwidth)
                    {
                        Monitor.Wait(locker);//等待
                    }
                }
            });
            task1.Start();
            task1.Wait();
        }

        private static readonly object objlock = new object();
        private static readonly object objlock2 = new object();

        SpinLock _spinLock = new SpinLock();
        SpinLock _spinLock2 = new SpinLock();
        int nowProgress;
        private void WorkALine2D(object o)
        {
            bool _lock = false;
            bool _lock2 = false;
            int x = (int)o;
            RGBColor tmpRGB;
            double tmp_deltae = 0;
            byte tmp_id = 0;
            for (int y = 0; y < rgbArray.GetLength(1); y++)
            {
                byte cache = Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]];
                if (cache != 0)
                {
                    //命中缓存
                    result[x, y] = cache;
                    _spinLock.Enter(ref _lock);
                    _spinLock.Exit();
                    _lock = false;
                    continue;
                }
                else
                {//未命中，计算最适合的方块 
                    double deltae = double.MaxValue;
                    tmpRGB = new RGBColor(System.Drawing.Color.FromArgb(rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]));
                    for (byte _id = 0; _id < myblockDatas2D.Length; _id++)
                    {
                        tmp_deltae = GetErr(tmpRGB, myblockDatas2D[_id]);
                        if (tmp_deltae < deltae)
                        {
                            deltae = tmp_deltae;
                            tmp_id = _id;
                        }
                    }
                    Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]] = (byte)(myblockDatas2D[tmp_id].classId - 1);
                    result[x, y] = (byte)(myblockDatas2D[tmp_id].classId - 1);
                    _spinLock2.Enter(ref _lock2);
                    _spinLock2.Exit();
                    _lock2 = false;
                }
            }
            lock (locker)
            {
                ++nowProgress;
                Monitor.Pulse(locker);
            }
        }
        private void WorkALine3D(object o)
        {
            int x = (int)o;
            RGBColor tmpRGB;
            double tmp_deltae = 0;
            byte tmp_id = 0;
            for (int y = 0; y < rgbArray.GetLength(1); y++)
            {
                byte cache = Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]];
                if (cache != 0)
                {//命中缓存
                    result[x, y] = cache;
                    height[x, y] = (myblockDatas3D[cache].height);
                    continue;
                }
                else
                {//未命中，计算最适合的方块 
                    double deltae = double.MaxValue;
                    tmpRGB = new RGBColor(System.Drawing.Color.FromArgb(rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]));

                    for (byte _id = 0; _id < myblockDatas3D.Length; _id++)
                    {
                        tmp_deltae = GetErr2(tmpRGB, myblockDatas3D[_id]);
                        if (tmp_deltae < deltae)
                        {
                            deltae = tmp_deltae;
                            tmp_id = _id;
                        }
                    }
                    Mapping[rgbArray[x, y, 0], rgbArray[x, y, 1], rgbArray[x, y, 2]] = (byte)(myblockDatas3D[tmp_id].classId - 1);
                    result[x, y] = (byte)(myblockDatas3D[tmp_id].classId - 1);
                    height[x, y] = (myblockDatas3D[tmp_id].height);
                }
            }
            lock (locker)
            {
                ++nowProgress;
                Monitor.Pulse(locker);
            }
        }

        private void OnGenerateFinished()
        {

        }
        public byte[,] Get2DResult() => result;
        public (byte[,], byte[,]) Get3DResult() => (result, height);

        private double RGBDifference(RGBColor color0, ConciseBlockData2D color1)
        {
            return Math.Pow(color0.R - color1.rGBColor.R, 2) + Math.Pow(color0.G - color1.rGBColor.G, 2) + Math.Pow(color0.B - color1.rGBColor.B, 2);
        }
        private double RGBDifference(RGBColor color0, ConciseBlockData3D color1)
        {
            return Math.Pow(color0.R - color1.rGBColor.R, 2) + Math.Pow(color0.G - color1.rGBColor.G, 2) + Math.Pow(color0.B - color1.rGBColor.B, 2);
        }
        public static ColourfulConverter converter = new ColourfulConverter { WhitePoint = Illuminants.D65 };
        CIEDE2000ColorDifference cIEDE2000ColorDifference = new CIEDE2000ColorDifference();
        private double LABDifference(RGBColor color0, ConciseBlockData2D color1)
        {
            return cIEDE2000ColorDifference.ComputeDifference(converter.ToLab(color0), color1.labColor);
        }
        private double LABDifference(RGBColor color0, ConciseBlockData3D color1)
        {
            return cIEDE2000ColorDifference.ComputeDifference(converter.ToLab(color0), color1.labcolor);
        }
        enum GenerateType
        {
            _2d,
            _3d,
        }
    }
}
