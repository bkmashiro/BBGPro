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
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BlockInfoManager blockInfoManager = new BlockInfoManager();
            blockInfoManager.Init();
            ImageService imageService = new ImageService();
            imageService.LoadImage(@"test2.gif");

            MapGenerator mapGenerator = new MapGenerator();
            mapGenerator.Init(imageService.GetImageArray());
            mapGenerator.Generate(blockInfoManager.GetConciseBlockData2D());
            var p =mapGenerator.Get2DResult();
            Console.WriteLine();
        }
    }
}
