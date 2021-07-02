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
    /// ChooseColor.xaml 的交互逻辑
    /// </summary>
    public partial class ChooseColor : Page
    {
        public ChooseColor(AffairHandler affairHandler)
        {
            InitializeComponent();
            AffairHandler = affairHandler;
            for (int i = 0; i < affairHandler.BlockInfoManager.blockDatas_higher.Count; i++)
            {
                this.wp.Children.Add(new ColorDemo(affairHandler, i));
            }
        }

        public AffairHandler AffairHandler { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AffairHandler.PageTo(3);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Snackbar.MessageQueue.Enqueue(Application.Current.FindResource("color_hint1").ToString());
        }
    }
}
