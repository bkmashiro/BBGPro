using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BBG
{
    class BatchProcessing
    {
        public bool UseMask = false;
        public bool Is3D = false;
        public bool IsMultiThread = true;
        public bool UseLab = true;

        int fileCounter = 0;
        string SavePath = "";
        string FileName = "batch_";

        int MaxWork = 0;
        int nowWork = 0;

        List<string> imgs = new List<string>();

        BlockInfoManager BlockInfoManager;
        Schematic Schematic = new Schematic();
        MuteGenerator MapGenerator = new MuteGenerator();
        public ImageService imageService = new ImageService();
        MaskOverride MaskOverride;

        TextBlock overall_txt;
        ProgressBar overall_progress;

        public void Init(string folder)
        {

        }
        public void BindCtrls(TextBlock t_overall, ProgressBar p_overall)
        {
            overall_txt = t_overall;
            overall_progress = p_overall;
        }
        private void SetTask(int max)
        {
            MaxWork = max;
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                overall_progress.Visibility = Visibility.Visible;
                overall_progress.Maximum = max;
                overall_progress.Value = 0;
            }));
        }
        private void TaskFinished()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                overall_progress.Visibility = Visibility.Hidden;
                overall_progress.Value = 0;
            }));
        }
        private void UpdateTask(int progress)
        {
            Task t = new Task(new Action(() =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    overall_progress.Value = progress;
                    overall_txt.Text = string.Format(
                        Application.Current.FindResource("batch_overall_progress").ToString(),
                        (nowWork / (float)MaxWork * 100).ToString("#0.00"),
                        nowWork,
                        MaxWork
                        );
                }));
            }));
            t.Start();
        }

        public void LoadSettings(AffairHandler af)
        {
            this.BlockInfoManager = af.BlockInfoManager;
            Schematic.ReadBlockDatas(BlockInfoManager.GetBlockDatas2D());
            MaskOverride = new MaskOverride(imageService);
        }

        public void Init(IEnumerable<string> files)
        {
            imgs = files.ToList();
        }

        public void SetGenMode(bool Is3D, bool MultiThread, bool mask = false)
        {
            if (Is3D)
            {
                if (MultiThread)
                {
                    DoWork = DoWork3D_multiThread;
                }
                else
                {
                    DoWork = DoWork3D;
                }
            }
            else
            {
                if (MultiThread)
                {
                    DoWork = DoWork2D_multiThread;
                }
                else
                {
                    DoWork = DoWork2D;
                }
            }
            if (Is3D && UseMask)
            {
                System.Windows.Forms.MessageBox.Show("不支持三维模式下使用蒙版！蒙版已被强制禁用。");
                UseMask = false;
                return;
            }
            UseMask = mask;
        }

        delegate void Work<T>(T orginal);
        Work<string> DoWork;
        public void Process()
        {
            SetGenMode(Is3D, IsMultiThread, UseMask);
            SetTask(imgs.Count);
            Task task = new Task(new Action(() =>
            {
                for (int i = 0; i < imgs.Count; i++)
                {
                    DoWork(imgs[i]);
                    nowWork = i + 1;
                    UpdateTask(i + 1);
                }
                Task.Delay(50000);
                TaskFinished();
            }));
            task.Start();
        }
        private void DoWork2D(string img)
        {
            imageService.LoadImage(img);
            MapGenerator.Init(imageService.GetImageArray(), UseLab);
            MapGenerator.Generate(BlockInfoManager.GetConciseBlockData2D());
            Save();

        }

        private void DoWork3D(string img)
        {
            imageService.LoadImage(img);
            MapGenerator.Init(imageService.GetImageArray(), UseLab);
            MapGenerator.Generate(BlockInfoManager.GetConciseBlockData3D());
            Save();

        }

        private void DoWork2D_multiThread(string img)
        {
            imageService.LoadImage(img);
            MapGenerator.Init(imageService.GetImageArray(), UseLab);
            MapGenerator.Multithread_Generate(BlockInfoManager.GetConciseBlockData2D());
            Save();

        }

        private void DoWork3D_multiThread(string img)
        {
            imageService.LoadImage(img);
            MapGenerator.Init(imageService.GetImageArray(), UseLab);
            MapGenerator.Multithread_Generate(BlockInfoManager.GetConciseBlockData3D());
            Save();
        }
        public  byte r=0;
        public  byte g=0;
        public  byte b=0;
        private void Save()
        {
            var res = MapGenerator.result;
            Schematic.Read2D(res);
            if (UseMask)
            {
                MaskOverride.Init(res);
                Schematic.ReadMask(MaskOverride.GetMaskedByColor(r, g, b));
            }
            Schematic.Save((++fileCounter).ToString() + ".schem");
            //Schematic.Save($"{SavePath}{FileName}{fileCounter}.schem");
        }

    }
}
