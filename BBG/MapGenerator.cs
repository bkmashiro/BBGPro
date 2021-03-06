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
    public class MapGenerator
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
        ProgressBar progressBar;
        TextBlock tb1;
        TextBlock tb2;
        int workMax = -1;
        int nowProgress = 0;
        int hitcnt = -1;
        int pixelTotal = -1;
        string str1;
        string str2;
        string str3;
        string str4;
        string str5;
        public int[] blockUsage;
        AffairHandler AffairHandler;
        public void MappingOverRide(byte[,,] b) => Mapping = b;
        public void BindProgress(ProgressBar p, TextBlock t1, TextBlock t2, AffairHandler affair)
        {
            AffairHandler = affair;
            progressBar = p;
            tb1 = t1;
            tb2 = t2;
        }
        private void SetProgress(int max)
        {
            str1 = Application.Current.FindResource("gen_stage").ToString();
            str2 = Application.Current.FindResource("gen_progress").ToString();
            str3 = Application.Current.FindResource("gen_stage_0").ToString();
            str4 = Application.Current.FindResource("gen_stage_1").ToString();
            str5 = Application.Current.FindResource("gen_stage_finished").ToString();
            tb1.Dispatcher.Invoke(new Action(() =>
            {
                tb1.Text = string.Format(str1, str3);
            }));
            if (progressBar != null)
            {
                progressBar.Dispatcher.Invoke(new Action(() =>
                {
                    progressBar.Maximum = max;
                }));
            }
            Task t = new Task(new Action(() =>
            {
                while (nowProgress != workMax)
                {
                    UpdateProgress(nowProgress);
                    Thread.Sleep(100);
                }
            }));
            t.Start();
        }
        private void UpdateProgress(int i)
        {
            if (progressBar != null)
            {
                progressBar.Dispatcher.Invoke(new Action(() =>
                {
                    progressBar.Value = i;
                }));
                tb2.Dispatcher.Invoke(new Action(() =>
                {
                    tb2.Text = string.Format(str2, (((float)i) / workMax * 100).ToString("#0.0"), i + 1, workMax);
                }));
            }
        }
        Stopwatch stopwatch = new Stopwatch();
        private void IncProgress()
        {

            ++nowProgress;
            if (nowProgress == workMax)
            {
                OnGenerateFinished();
                stopwatch.Stop();
                Console.WriteLine(stopwatch.ElapsedMilliseconds);
                nowProgress = 0;
            }
        }
        public void Init(byte[,,] _rgbArray, bool useLab = true)
        {
            imgWidth = _rgbArray.GetLength(0);
            workMax = imgWidth;
            SetProgress(workMax);
            imgLength = _rgbArray.GetLength(1);
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
            tb1.Dispatcher.Invoke(new Action(() =>
            {
                tb1.Text = String.Format(str1, str4);
            }));
            blockUsage = new int[AffairHandler.BlockInfoManager.blockDatas_higher.Count];

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
                            ++blockUsage[cache];
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
                            ++blockUsage[result[x, y]];
                        }
                    }
                    UpdateProgress(x);
                }
                OnGenerateFinished();

            });
            task1.Start();
        }
        public void Generate(ConciseBlockData3D[] blockDatas)
        {
            tb1.Dispatcher.Invoke(new Action(() =>
            {
                tb1.Text = String.Format(str1, str4);
            }));
            blockUsage = new int[AffairHandler.BlockInfoManager.blockDatas_higher.Count];
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
                            ++blockUsage[cache];
                            continue;
                        }
                        else
                        {//未命中，计算最适合的方块 
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
                            ++blockUsage[result[x, y]];
                        }
                    }
                    UpdateProgress(x);
                }
                OnGenerateFinished();
            });
            task1.Start();
        }

        ConciseBlockData2D[] myblockDatas2D;
        ConciseBlockData3D[] myblockDatas3D;

        public void Multithread_Generate(ConciseBlockData2D[] blockDatas)
        {

            tb1.Dispatcher.Invoke(new Action(() =>
            {
                tb1.Text = string.Format(str1, str4);
            }));
            myblockDatas2D = blockDatas;
            blockUsage = new int[AffairHandler.BlockInfoManager.blockDatas_higher.Count];
            stopwatch.Start();
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
            });
            task1.Start();
        }
        public void Multithread_Generate(ConciseBlockData3D[] blockDatas)
        {
            tb1.Dispatcher.Invoke(new Action(() =>
            {
                tb1.Text = String.Format(str1, str4);
            }));
            myblockDatas3D = blockDatas;
            blockUsage = new int[AffairHandler.BlockInfoManager.blockDatas_higher.Count];

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
            });
            task1.Start();
        }

        private static readonly object objlock = new object();
        private static readonly object objlock2 = new object();

        SpinLock _spinLock = new SpinLock();
        SpinLock _spinLock2 = new SpinLock();
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
                    ++blockUsage[cache];
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
                    ++blockUsage[result[x, y]];
                    _spinLock2.Exit();
                    _lock2 = false;
                }
            }
            lock (objlock)
            {
                IncProgress();
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
                    lock (objlock2)
                    {
                        ++blockUsage[cache];
                    }
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
                    lock (objlock2)
                    {
                        ++blockUsage[result[x, y]];
                    }
                }
            }
            lock (objlock)
            {
                IncProgress();
            }
        }

        private void OnGenerateFinished()
        {
            if (AffairHandler.IgnoreTransprant)
            {

            }
            tb1.Dispatcher.Invoke(new Action(() =>
            {
                tb1.Text = string.Format(str1, str5);
            }));
            AffairHandler.PageTo(4);
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
