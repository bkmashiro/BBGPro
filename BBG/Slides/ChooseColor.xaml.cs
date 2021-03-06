using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            affairHandler.chooseColor = this;
            Task task = new Task(() =>
            {
                for (int i = 0; i < affairHandler.BlockInfoManager.blockDatas_higher.Count; i++)
                {
                    try
                    {
                        wp.Dispatcher.Invoke(new Action(() =>
                        {
                            wp.Children.Add(new ColorDemo(affairHandler, i));
                        }), System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                    }
                    catch
                    {
                        //Do nothing
                    }
                }
            });
            task.Start();
        }

        public bool tmp_JumpToBatch = false;



        public AffairHandler AffairHandler { get; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tmp_JumpToBatch)
            {
                AffairHandler.PageTo(5);
                tmp_JumpToBatch = false;
                return;
            }
            AffairHandler.PageTo(3);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //Snackbar.MessageQueue.Enqueue(Application.Current.FindResource("color_hint1").ToString());
        }
    }
}
