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

namespace BBG
{
    /// <summary>
    /// ColorDemo.xaml 的交互逻辑
    /// </summary>
    public partial class ColorDemo : UserControl
    {
        public ColorDemo(AffairHandler affairHandler, int index)
        {
            InitializeComponent();
            AffairHandler = affairHandler;
            Index = index;

            var task1 = new Task(() =>
            {

                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    int counter = 0;
                    string dir = System.AppDomain.CurrentDomain.BaseDirectory;

                    foreach (var item in affairHandler.BlockInfoManager.blockDatas_flat[index].image)
                    {
                        Image image = new Image();
                        image.Source = new BitmapImage(new Uri(dir + @"blockdata/" + item, UriKind.Absolute));
                        System.Windows.Media.RenderOptions.SetBitmapScalingMode(image, System.Windows.Media.BitmapScalingMode.NearestNeighbor);
                        image.Tag = counter++;
                        image.MouseDown += Image_MouseDown;
                        Image_holder.Children.Add(image);
                    }
                    var c = affairHandler.BlockInfoManager.blockDatas_higher[index].RGBColor.ToColor();
                    this.color_name.Background = new SolidColorBrush(Color.FromRgb(c.R, c.G, c.B));
                    this.color_name.Text = ToHTMLColorCode(c);
                    this.cb.Click += Cb_Click;
                    demo_block.Source = new BitmapImage(new Uri(dir + @"blockdata/" + affairHandler.BlockInfoManager.blockDatas_flat[index].image[0], UriKind.Absolute));
                    System.Windows.Media.RenderOptions.SetBitmapScalingMode(demo_block, System.Windows.Media.BitmapScalingMode.NearestNeighbor);

                }));

            });
            task1.Start();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AffairHandler.BlockInfoManager.demoBlockIndex[Index] = int.Parse((sender as Image).Tag.ToString());
            this.demo_block.Source= new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"blockdata/" + AffairHandler.BlockInfoManager.blockDatas_flat[Index].image[int.Parse((sender as Image).Tag.ToString())], UriKind.Absolute));
        }

        bool ColorIsEnabled = true;
        private void Cb_Click(object sender, RoutedEventArgs e)
        {
            if (ColorIsEnabled)
            {
                g.Opacity = 0.5f;
                ColorIsEnabled = false;
                AffairHandler.BlockInfoManager.colorEnabled[Index] = false;
            }
            else
            {
                g.Opacity = 1.0f;
                ColorIsEnabled = true;
                AffairHandler.BlockInfoManager.colorEnabled[Index] = true;
            }
        }

        public AffairHandler AffairHandler { get; }
        public int Index { get; }

        private static string ToHTMLColorCode(System.Drawing.Color color) => System.Drawing.ColorTranslator.ToHtml(color);
    }
}
