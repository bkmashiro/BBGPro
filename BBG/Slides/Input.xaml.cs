using System;
using System.Collections.Generic;
using System.IO;
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
    /// Input.xaml 的交互逻辑
    /// </summary>
    public partial class Input : Page
    {
        public AffairHandler AffairHandler { get; }

        public Input(AffairHandler affairHandler)
        {
            InitializeComponent();
            AffairHandler = affairHandler;
        }

        private void Rectangle_Drop(object sender, System.Windows.DragEventArgs e)
        {

        }

        private void image_Drop(object sender, System.Windows.DragEventArgs e)
        {

        }

        private void disposeImg_Click(object sender, RoutedEventArgs e)
        {
            height = -1;
            width = -1;

            filePath = string.Empty;
            myfileName = string.Empty;
            image.Source = null;
        }
        string filePath;
        string myfileName;
        int width;
        int height;

        private void AddImg_Click(object sender, RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "图像文件(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|所有文件(*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = openFileDialog.FileName;
                    myfileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName) ;
                    try
                    {
                        image.Source = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            System.Drawing.Image image = System.Drawing.Image.FromStream(fs);
                            width = image.Width;
                            height = image.Height;
                            imageInfo.Text = $"{width}px*{height}px";
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"ERROR 读取图片的异常：{ex.Message}");
                        throw;
                    }
                }
            }
        }

        private void input_ok_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(filePath) && filePath != string.Empty)
            {
                AffairHandler.ImageService.LoadImage(filePath);
                AffairHandler.ImageService.imageName = myfileName;
                AffairHandler.PageTo(2);
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请正确选择图片！");
            }

        }
    }
}
