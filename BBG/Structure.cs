using Colourful;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBG
{
    public struct BlockData
    {
        public readonly string name;//形如 namespace:blockname 的方块唯一id
        public readonly byte metadata;//附加值
        public readonly LabColor LabColor;//Lab颜色
        public readonly RGBColor RGBColor;//RGB颜色
        public readonly string[] image;//方块图片
        public readonly byte classid;//方块图片
        public readonly byte height;
        public bool enabled;

        public BlockData(string namae, byte meta, LabColor lab, RGBColor rGB, string[] img, byte cls, byte h, bool b = false)
        {
            name = namae;
            metadata = meta;
            LabColor = lab;
            RGBColor = rGB;
            image = img;
            classid = cls;
            height = h;
            enabled = b;
        }
    }


    public struct ConciseBlockData2D
    {
        public readonly byte classId;
        public readonly RGBColor rGBColor;
        public readonly LabColor labColor;

        public ConciseBlockData2D(byte classId, RGBColor rGBColor, LabColor labColor)
        {
            this.classId = classId;
            this.rGBColor = rGBColor;
            this.labColor = labColor;
        }
    }

    public struct ConciseBlockData3D
    {
        public readonly byte classId;
        public readonly RGBColor rGBColor;
        public readonly LabColor labcolor;
        public readonly byte height;

        public ConciseBlockData3D(byte classId, RGBColor rGBColor, LabColor labcolor, byte height)
        {
            this.classId = classId;
            this.rGBColor = rGBColor;
            this.labcolor = labcolor;
            this.height = height;
        }
    }
}
