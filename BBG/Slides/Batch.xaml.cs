using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BBG.Slides
{
    /// <summary>
    /// Batch.xaml 的交互逻辑
    /// </summary>
    public partial class Batch : Page
    {
        public Batch(AffairHandler affairHandler)
        {
            InitializeComponent();
            AffairHandler = affairHandler;
        }

        public AffairHandler AffairHandler { get; }

        BatchProcessing batchProcessing = new BatchProcessing();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AffairHandler.PageTo(2);
            AffairHandler.chooseColor.tmp_JumpToBatch = true;
        }

        private void cb2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void toggle_Click(object sender, RoutedEventArgs e)
        {
            //Already handled.
        }

        private void chooseFolder_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.CheckFileExists = true;
                ofd.CheckPathExists = true;
                ofd.Multiselect = true;
                ofd.Title = System.Windows.Application.Current.FindResource("batch_choose_imgs").ToString();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    batchProcessing.Init(ofd.FileNames);
                }
            }
        }

        private void Process_Click(object sender, RoutedEventArgs e)
        {
            batchProcessing.SetGenMode(toggle.IsChecked ?? false, true, toggle2.IsChecked ?? false);
            batchProcessing.LoadSettings(AffairHandler);
            batchProcessing.BindCtrls(overall_progress, progress_bar);
            batchProcessing.Process();
        }

        private void cb_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            int selectIndex = cb.SelectedIndex;
            if (selectIndex == -1)
            {
                batchProcessing.imageService.UseDither = true;
                batchProcessing.imageService.ditherType = 5;
            }
            else
            {
                if (selectIndex == 0)
                {
                    batchProcessing.imageService.UseDither = false;
                    batchProcessing.imageService.ditherType = -1;
                }
                else
                {
                    batchProcessing.imageService.UseDither = true;
                    batchProcessing.imageService.ditherType = selectIndex;
                }
            }
        }

        bool UseLab = true;
        private void cb2_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            int selectIndex = cb2.SelectedIndex;
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
            batchProcessing.UseLab = UseLab;
        }

        private void toggle2_Click(object sender, RoutedEventArgs e)
        {
            batchProcessing.UseMask = toggle2.IsChecked ?? false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                colorDialog.ShowHelp = true;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    SolidColorBrush solidColorBrush = new SolidColorBrush();
                    solidColorBrush.Color = Color.FromRgb(colorDialog.Color.R,colorDialog.Color.G,colorDialog.Color.B);
                    color_rec.Fill = solidColorBrush;
                    batchProcessing.r = colorDialog.Color.R;
                    batchProcessing.g = colorDialog.Color.G;
                    batchProcessing.b = colorDialog.Color.B;
                }
            }
        }
    }
}
