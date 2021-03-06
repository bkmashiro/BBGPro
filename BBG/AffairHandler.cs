using BBG.Slides;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BBG
{
    public class AffairHandler
    {
        public BlockInfoManager BlockInfoManager;
        public ImageService ImageService;
        public MapGenerator mapGenerator;
        public ImgPreview ImgPreview;
        public Schematic Schematic;
        public Transitioner transitioner;

        public ChooseColor chooseColor;

        public bool is3DMode;
        public bool isMultiThread=true;
        public bool tmp_Jump2Batch = false;
        public bool IgnoreTransprant = false;
        public void InitInput()
        {
            BlockInfoManager = new BlockInfoManager();
            ImageService = new ImageService();
            mapGenerator = new MapGenerator();
        }
        public void InitOutput()
        {
            BlockInfoManager = new BlockInfoManager();
            ImageService = new ImageService();
            mapGenerator = new MapGenerator();
        }
        public void InitAll()
        {
            InitInput();
            InitOutput();
            BlockInfoManager.Init();
        }
        public void BindTransitioner(Transitioner trans)
        {
            this.transitioner = trans;
        }
        public void PageTo(int index)
        {
            if (transitioner != null)
            {
                transitioner.Dispatcher.Invoke(new Action(() =>
                {

                    transitioner.SelectedIndex = index;
                }));
            }
        }
        public void PreviousPage()
        {
            if (transitioner.SelectedIndex >= 1)
            {
                transitioner.SelectedIndex--;
            }
        }
    }
}
