using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBG
{
    class BatchProcessing
    {
        bool UseMask = false;
        bool Is3D = false;
        bool IsMultiThread = false;
        bool UseLab = true;

        int fileCounter = 0;
        string SavePath = "";
        string FileName = "batch_";

        List<string> imgs = new List<string>();

        BlockInfoManager BlockInfoManager;
        Schematic Schematic = new Schematic();
        MuteGenerator MapGenerator = new MuteGenerator();
        ImageService imageService = new ImageService();
        MaskOverride MaskOverride;

        public void Init(string folder)
        {

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
            for (int i = 0; i < imgs.Count; i++)
            {
                DoWork(imgs[i]);
            }
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

        private void Save()
        {
            var res = MapGenerator.result;
            Schematic.Read2D(res);
            if (UseMask)
            {
                MaskOverride.Init(res);
                Schematic.ReadMask(MaskOverride.GetMaskedByColor(255, 255, 255));
            }
            Schematic.Save((++fileCounter).ToString() + ".schem");
            //Schematic.Save($"{SavePath}{FileName}{fileCounter}.schem");
        }

    }
}
