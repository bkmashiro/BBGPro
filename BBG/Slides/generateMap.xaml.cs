using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BBG.Slides
{
    /// <summary>
    /// generateMap.xaml 的交互逻辑
    /// </summary>
    public partial class generateMap : Page
    {
        public generateMap(AffairHandler affairHandler)
        {
            InitializeComponent();
            AffairHandler = affairHandler;
            Gen_progress_grid.Visibility = Visibility.Collapsed;
            progress_bar.IsIndeterminate = true;
            affairHandler.mapGenerator.BindProgress(progress_bar, txt_stage, txt_progress, affairHandler);
        }

        public delegate void Gen();
        Gen myGenerate;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!AffairHandler.ImageService.IsImgLoaded)
            {
                MessageBox.Show("未加载图片！");
                return;
            }
            AffairHandler.mapGenerator.Init(AffairHandler.ImageService.GetImageArray());
            Gen_progress_grid.Visibility = Visibility.Visible;

            if (Is3dEnabled)
            {//3d
                if (AffairHandler.isMultiThread)
                {
                    AffairHandler.mapGenerator.Multithread_Generate(AffairHandler.BlockInfoManager.GetConciseBlockData3D());
                }
                else
                {
                    AffairHandler.mapGenerator.Generate(AffairHandler.BlockInfoManager.GetConciseBlockData3D());
                }
            }
            else
            {//2d
                //AffairHandler.mapGenerator.Generate(AffairHandler.BlockInfoManager.GetConciseBlockData2D());
                if (AffairHandler.isMultiThread)
                {
                    AffairHandler.mapGenerator.Multithread_Generate(AffairHandler.BlockInfoManager.GetConciseBlockData2D());
                }
                else
                {
                    AffairHandler.mapGenerator.Generate(AffairHandler.BlockInfoManager.GetConciseBlockData2D());
                }
            }
        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectIndex = cb.SelectedIndex;
            if (selectIndex == -1)
            {
                AffairHandler.ImageService.UseDither = true;
                AffairHandler.ImageService.ditherType = 5;
            }
            else
            {
                if (selectIndex == 0)
                {
                    AffairHandler.ImageService.UseDither = false;
                    AffairHandler.ImageService.ditherType = -1;
                }
                else
                {
                    AffairHandler.ImageService.UseDither = true;
                    AffairHandler.ImageService.ditherType = selectIndex;
                }
            }

        }

        bool Is3dEnabled = false;

        public AffairHandler AffairHandler { get; }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (toggle.IsChecked ?? true)
            {
                txt_Is3dEnabled.Text = Application.Current.FindResource("gen_enable_3d").ToString();
                Is3dEnabled = true;
                AffairHandler.is3DMode = true;
            }
            else
            {
                txt_Is3dEnabled.Text = Application.Current.FindResource("gen_disable_3d").ToString();
                Is3dEnabled = false;
                AffairHandler.is3DMode = false;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            AffairHandler.PageTo(2);
        }

        bool UseLab = true;
        private void cb2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectIndex = cb2.SelectedIndex;
            string str = cb2.SelectedItem.ToString();
            if (selectIndex == -1)
            {
                //默认 lab
                UseLab = true;
            }
            else
            {
                if (selectIndex == 0)
                {
                    UseLab = true;
                }
                else
                {
                    UseLab = false;
                }
            }
        }

        private void toggle2_Click(object sender, RoutedEventArgs e)
        {
            if (toggle2.IsChecked ?? true)
            {
                AffairHandler.IgnoreTransprant = true;
            }
            else
            {
                AffairHandler.IgnoreTransprant = false;
            }
        }
    }
}
