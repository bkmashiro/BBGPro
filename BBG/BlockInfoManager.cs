using Colourful;
using Colourful.Conversion;
using System;
using System.Collections.Generic;
using System.IO;

namespace BBG
{
   public class BlockInfoManager
    {
        public bool[] colorEnabled;
        int colorCount;
        int now_colorCount;
        public int[] demoBlockIndex;

        public List<BlockData> blockDatas_higher = new List<BlockData>();//更高的方块信息
        public List<BlockData> blockDatas_flat = new List<BlockData>();//齐平的方块信息
        public List<BlockData> blockDatas_lower = new List<BlockData>();//更低的方块信息

        public void Init()
        {
            int color_cnt = 0;
            float multiplier_flat = 0.86274f;
            float multiplier_lower = 0.70588f;
            List<BlockData> tmp = new List<BlockData>();

            try
            {
                StreamReader F = new StreamReader("BlockData.txt");
                string data = F.ReadToEnd();
                ColourfulConverter converter = new ColourfulConverter { WhitePoint = Illuminants.D65 };

                foreach (var blockclass in data.Trim().Split('\n'))
                {
                    if (blockclass == string.Empty)
                    {
                        continue;
                    }
                    ++color_cnt;
                    string[] dts = blockclass.Trim().Split(',');
                    if (dts.Length == 7)
                    {
                        RGBColor RGB = new RGBColor(System.Drawing.Color.FromArgb(255, int.Parse(dts[2]), int.Parse(dts[3]), int.Parse(dts[4])));
                        LabColor lab = converter.ToLab(RGB);
                        BlockData blockData = new BlockData(dts[5], 0, lab, RGB, dts[6].TrimEnd(';').Split(';'), byte.Parse(dts[0]), 2);
                        tmp.Add(blockData);
                    }
                }
                colorCount = color_cnt;
                now_colorCount = color_cnt;
                colorEnabled = new bool[color_cnt];
                for (int i = 0; i < colorEnabled.Length; i++)
                {
                    colorEnabled[i] = true;
                }
                demoBlockIndex = new int[color_cnt];
                blockDatas_higher = tmp;
                foreach (var item in blockDatas_higher)
                {
                    RGBColor RGB = RgbMulitply(multiplier_flat, item.RGBColor);
                    LabColor lab = converter.ToLab(RGB);
                    BlockData blockData = new BlockData(item.name, item.metadata, lab, RGB, item.image, item.classid, 1);
                    blockDatas_flat.Add(blockData);
                    RGBColor RGB2 = RgbMulitply(multiplier_lower, item.RGBColor);
                    LabColor lab2 = converter.ToLab(RGB);
                    BlockData blockData2 = new BlockData(item.name, item.metadata, lab, RGB, item.image, item.classid, 0);
                    blockDatas_lower.Add(blockData);
                }
            }
            catch (Exception ex)
            {
                //TODO
                throw;
            }

        }
        public void DisableAllColor()
        {
            for (int i = 0; i < colorEnabled.Length; i++)
            {
                colorEnabled[i] = false;
            }
            now_colorCount = 0;
        }
        public void EnableAllColor()
        {
            for (int i = 0; i < colorEnabled.Length; i++)
            {
                colorEnabled[i] = true;
            }
            now_colorCount = colorCount;
        }
        public bool RemoveColor(int index)
        {
            bool bo = false;
            if (colorEnabled[index])
            {
                colorEnabled[index] = false;
                --now_colorCount;
                bo = true;
            }
            return bo;
        }
        public bool AddColor(int index)
        {
            bool bo = false;
            if (!colorEnabled[index])
            {
                colorEnabled[index] = true;
                ++now_colorCount;
                bo = true;
            }
            return bo;
        }
        public void ReverseColor(int index)
        {
            if (colorEnabled[index])
            {
                colorEnabled[index] = false;
            }
            else
            {
                colorEnabled[index] = true;
            }
        }

        public BlockData[] GetBlockDatas2D()
        {
            BlockData[] blockDatas = new BlockData[colorCount];
            for (int i = 0; i < blockDatas_higher.Count; i++)
            {
                if (colorEnabled[i])
                {
                    blockDatas[i] = blockDatas_flat[i];
                }
            }
            return blockDatas;
        }

        public BlockData[] GetBlockDatas3D()
        {
            List<BlockData> blockDatas = new List<BlockData>();
            blockDatas.AddRange(blockDatas_higher);
            blockDatas.AddRange(blockDatas_flat);
            blockDatas.AddRange(blockDatas_lower);
            return blockDatas.ToArray();
        }

        public ConciseBlockData2D[] GetConciseBlockData2D()
        {
            ConciseBlockData2D[] conciseBlockData2Ds = new ConciseBlockData2D[colorCount];
            for (int i = 0; i < blockDatas_higher.Count; i++)
            {
                if (colorEnabled[i])
                {
                    conciseBlockData2Ds[i] = new ConciseBlockData2D(blockDatas_flat[i].classid, blockDatas_flat[i].RGBColor, blockDatas_flat[i].LabColor);
                }
            }
            return conciseBlockData2Ds;
        }

        public ConciseBlockData3D[] GetConciseBlockData3D()
        {
            List<ConciseBlockData3D> conciseBlockData3Ds = new List<ConciseBlockData3D>();
            for (int i = 0; i < blockDatas_higher.Count; i++)
            {
                if (colorEnabled[i])
                {
                    conciseBlockData3Ds.Add(GetConciseBlock(blockDatas_flat[i]));
                    conciseBlockData3Ds.Add(GetConciseBlock(blockDatas_higher[i]));
                    conciseBlockData3Ds.Add(GetConciseBlock(blockDatas_lower[i]));
                }
            }
            return conciseBlockData3Ds.ToArray();
        }

        private RGBColor RgbMulitply(double rate, RGBColor rgb)
        {
            return new RGBColor(rgb.R * rate, rgb.G * rate, rgb.B * rate);
        }

        private ConciseBlockData3D GetConciseBlock(BlockData blockData) =>
            new ConciseBlockData3D(blockData.classid, blockData.RGBColor, blockData.LabColor, blockData.height);


    }
}
