using Cyotek.Data.Nbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBG
{
   public class Schematic
    {
        List<string> blockPalette = new List<string>();
        List<byte> blockMetadata = new List<byte>();
        Dictionary<string, byte> blockNameToID = new Dictionary<string, byte>();
        int mapper = 0;
        byte[,,] myBlockData;
        int height = 1;
        int width = 1;
        int length = 1;
        int WEoffset_x = 0;
        int WEoffset_y = 0;
        int WEoffset_z = 0;
        int offset_x = 0;
        int offset_y = 0;
        int offset_z = 0;
        NbtDocument document;
        TagCompound schematic;
        TagCompound Palette;
        TagCompound Metadata;
        TagByteArray BlockData;
        TagIntArray Offset;
        TagList list;
        public void Generate()
        {
            //blockPalette.Add("minecraft:iron_block");
            //blockMetadata.Add(0);

            /*-
                {[Ilnt: PaletteMax=24]}
                {[Compound: Palette](24 items)}
                {[lnt: Version=2]}
                {[Short: Length=1]}
                {[Compound: Metadata] (3 items)}
                {[Short: Height=1]}
                {[Int: DataVersion=2586]}
                {[ByteArray: BlockData](24 items)}
                {[List: BlockEntities] (O items)}
                {[Short: Width=24]}
                {[IntArray: Offset](3 items)}
             * -*/
            string[] _palette = this.blockPalette.Distinct().ToArray();
            document = new NbtDocument();
            schematic = document.DocumentRoot;
            schematic.Name = "Schematic";
            #region PaletteMax

            //{[Ilnt: PaletteMax=24]}
            schematic.Value.Add("PaletteMax", _palette.Length);
            #endregion
            #region Palette

            //{[Compound: Palette](24 items)}
            Palette = (TagCompound)schematic.Value.Add("Palette", TagType.Compound);
            int cnt = 0;

            foreach (var item in _palette)
            {
                Palette.Value.Add(item, (int)cnt++);
            }

            System.Console.WriteLine();
            #endregion
            #region Version

            //{[lnt: Version=2]}
            schematic.Value.Add("Version", (int)2);
            #endregion
            #region Length

            //{[Short: Length=1]}
            schematic.Value.Add("Length", (short)length);
            #endregion
            #region Metadata

            //{[Compound: Metadata] (3 items)}
            Metadata = (TagCompound)schematic.Value.Add("Metadata", TagType.Compound);
            Metadata.Value.Add("WEOffsetX", WEoffset_x);
            Metadata.Value.Add("WEOffsetY", WEoffset_y);
            Metadata.Value.Add("WEOffsetZ", WEoffset_z);
            #endregion
            #region Height
            ////{[Short: Height= 1]}
            schematic.Value.Add("Height", (short)height);
            #endregion
            #region DataVersion
            ////{[Int: DataVersion= 2586]}
            schematic.Value.Add("DataVersion", (int)2586);

            #endregion
            #region BlockData
            ////{[ByteArray: BlockData](24 items)}
            ///

            //TO DO tREED_修改！
            byte[] buf = Flatten(myBlockData);

            System.Console.WriteLine();
            BlockData = new TagByteArray("BlockData", buf);
            schematic.Value.Add(BlockData);
            #endregion
            #region BlockEntities
            list = (TagList)schematic.Value.Add("BlockEntities", TagType.List, TagType.Compound);
            #endregion
            #region Width
            ////{[Short: Width= 24]}
            schematic.Value.Add("Width", (short)width);
            #endregion
            #region Offset
            int[] intArray = { offset_x, offset_y, offset_z };
            Offset = new TagIntArray("Offset", intArray);
            schematic.Value.Add(Offset);
            #endregion



            System.Console.WriteLine("生成成功！");
        }

        public void Save()
        {
            Generate();
            document.Save(@"D:\newbddld\1.16.5-fabric\1.16.5-fabric\1.16.5\[光影版本]XPlus 2.0 for 1.16.5整合包-fabric-20210127\.minecraft\config\worldedit\schematics\ts.schem");
        }

        public void Save(string path)
        {
            if (document != null)
            {
                if (Directory.Exists(path))
                {
                    Generate();
                    document.Save(path);
                }
            }
            else
            {
                throw new ArgumentNullException("未生成Schematic数据。");
            }
        }

        private byte[] Flatten(byte[,,] ids)
        {
            byte[] b = new byte[ids.Length];
            int cnt = 0;
            for (int y = 0; y < ids.GetLength(1); y++)
            {
                for (int z = 0; z < ids.GetLength(2); z++)
                {
                    for (int x = 0; x < ids.GetLength(0); x++)
                    {
                        b[cnt++] = ids[x, y, z];
                    }
                }
            }
            return b;
        }

        public void Init(int x, int y, int z)
        {
            myBlockData = new byte[x, y, z];
            height = y;
            width = x;
            length = z;
            Console.WriteLine($"init:{x}X,{y}Y,{z}Z");
        }

        public bool ReadBlockDatas(BlockData[] blockData)
        {
            bool bo = true;
            this.blockPalette.Add("minecraft:air");
            this.blockMetadata.Add(0);
            foreach (var item in blockData)
            {
                this.blockPalette.Add(item.name);
                this.blockMetadata.Add(0);
            }
            return bo;
        }
        public void Setblock(int id, int x, int y, int z)
        {
            myBlockData[x, y, z] = (byte)id;
        }
        public void Read3D(byte[,] id, byte[,] height)
        {
            int maxY = 0;
            int minY = 0;

            int[,] intHeight = new int[height.GetLength(0), height.GetLength(1)];
            for (int x = 0; x < height.GetLength(0); x++)
            {
                for (int z = 0; z < height.GetLength(1); z++)
                {
                    intHeight[x, z] = height[x, z] - 1;
                }
            }

            for (int x = 0; x < height.GetLength(0); x++)
            {
                for (int z = 1; z < height.GetLength(1); z++)
                {
                    intHeight[x, z] += (intHeight[x, z - 1]);
                }
            }
            for (int x = 0; x < intHeight.GetLength(0); x++)
            {
                for (int y = 0; y < intHeight.GetLength(1); y++)
                {
                    if (intHeight[x, y] > maxY)
                    {
                        maxY = intHeight[x, y];
                    }
                    if (intHeight[x, y] < minY)
                    {
                        minY = intHeight[x, y];
                    }
                }
            }
            Init(id.GetLength(0), maxY - minY + 1, id.GetLength(1));
            for (int x = 0; x < id.GetLength(0); x++)
            {
                for (int z = 0; z < id.GetLength(1); z++)
                {
                    Setblock(id[x, z] + 1, x, intHeight[x, z] - minY, z);
                }
            }
        }
        public void Read2D(byte[,] id)
        {

            Init(id.GetLength(0), 1, id.GetLength(1));
            for (int x = 0; x < id.GetLength(0); x++)
            {
                for (int z = 0; z < id.GetLength(1); z++)
                {
                    Setblock(id[x, z] + 1, x, 0, z);
                }
            }
        }

      
    }
}
