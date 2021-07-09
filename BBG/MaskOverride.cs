using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBG
{
    class MaskOverride
    {
        byte[,] original;
        ImageService imageService;
        public MaskOverride(ImageService im)
        {
            this.imageService = im;
        }
        public void Init(byte[,] o) => this.original = o;
        public bool[,] GetMaskedByColor(byte r, byte g, byte b)
        {
            var rgbArray = imageService.GetImageArray();
            bool[,] bo = new bool[rgbArray.GetLength(0), rgbArray.GetLength(1)];
            for (int x = 0; x < rgbArray.GetLength(0); x++)
            {
                for (int y = 0; y < rgbArray.GetLength(1); y++)
                {
                    if (rgbArray[x, y, 0] == b && rgbArray[x, y, 1] == g && rgbArray[x, y, 2] == r)
                    {
                        bo[x, y] = true;
                    }
                }
            }
            return bo;
        }

       

    }
}
