using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using System.Windows.Threading;

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
                loadImgs(index);

                var c = affairHandler.BlockInfoManager.blockDatas_higher[index].RGBColor.ToColor();
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    color_name.Background = new SolidColorBrush(Color.FromRgb(c.R, c.G, c.B));
                    color_name.Text = ToHTMLColorCode(c);
                    cb.Click += Cb_Click;
                    demo_block.Source = new BitmapImage(new Uri(dir + @"blockdata/" + affairHandler.BlockInfoManager.blockDatas_flat[index].image[0], UriKind.Absolute));
                    RenderOptions.SetBitmapScalingMode(demo_block, BitmapScalingMode.NearestNeighbor);
                }));
            });
            task1.Start();
        }


        int imgLoaded;
        List<string> imageName;
        ObservableCollection<string> strs = new ObservableCollection<string>();
        private void loadImgs(int index)
        {
            imageName = loadDir(index);
            strs.Clear();
            imgLoaded = 0;
            //Image_holder.ItemsSource = strs;
            Image_holder.Dispatcher.BeginInvoke(DispatcherPriority.Background, new AddItemDelegate(AddImage));
        }

        private delegate void AddItemDelegate();


        private List<string> loadDir(int index)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"blockdata\";
            List<string> strs = new List<string>();
            if (Directory.Exists(dir))
            {
                foreach (var item in AffairHandler.BlockInfoManager.blockDatas_flat[index].image)
                {
                    strs.Add(dir + item);
                }
            }
            return strs;
        }

        string dir = AppDomain.CurrentDomain.BaseDirectory;
        int counter = 0;


        private void AddImage()
        {
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(imageName[imgLoaded]));
            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            image.Tag = counter++;
            image.MouseDown += Image_MouseDown;
            Image_holder.Children.Add(image);

            if (imgLoaded < imageName.Count - 1)
            {
                strs.Add(imageName[imgLoaded++]);
                Image_holder.Dispatcher.BeginInvoke(DispatcherPriority.Background, new AddItemDelegate(AddImage));
            }
        }


        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AffairHandler.BlockInfoManager.demoBlockIndex[Index] = int.Parse((sender as Image).Tag.ToString());
            this.demo_block.Source = new BitmapImage(new Uri(System.AppDomain.CurrentDomain.BaseDirectory + @"blockdata/" + AffairHandler.BlockInfoManager.blockDatas_flat[Index].image[int.Parse((sender as Image).Tag.ToString())], UriKind.Absolute));
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
