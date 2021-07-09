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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            BatchProcessing batchProcessing = new BatchProcessing();
            batchProcessing.Init(new string[] { "mask_test.png" });
            batchProcessing.SetGenMode(false, true, true);
            batchProcessing.LoadSettings(AffairHandler);
            batchProcessing.Process();
        }

        private void cb2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
