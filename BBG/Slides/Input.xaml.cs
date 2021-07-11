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
            System.Array array = e.Data.GetData(System.Windows.DataFormats.FileDrop) as Array;
            string fileName = array.GetValue(0).ToString();
            myfileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    System.Drawing.Image im = System.Drawing.Image.FromStream(fs);
                    image.Source = new BitmapImage(new Uri(fileName, UriKind.Absolute));
                    filePath = fileName;
                    width = im.Width;
                    height = im.Height;
                    imageInfo.Text = $"{width}px*{height}px";
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"ERROR 读取图片的异常：{ex.Message} (兄啊，这是图片吗？)");
            }
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
                    string fileName = openFileDialog.FileName;
                    try
                    {
                        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                        {
                            System.Drawing.Image im = System.Drawing.Image.FromStream(fs);
                            image.Source = new BitmapImage(new Uri(fileName, UriKind.Absolute));
                            filePath = fileName;
                            myfileName = System.IO.Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                            width = im.Width;
                            height = im.Height;
                            imageInfo.Text = $"{width}px*{height}px";
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.Forms.MessageBox.Show($"ERROR 读取图片的异常：{ex.Message}");
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

        private void image_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                e.Effects = System.Windows.DragDropEffects.Link;
                drop_txt.Background = System.Windows.Media.Brushes.Transparent;
                drop_txt.Text = System.Windows.Application.Current.FindResource("input_is_a_image").ToString();
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
                drop_txt.Background = System.Windows.Media.Brushes.Red;
                drop_txt.Text = System.Windows.Application.Current.FindResource("input_not_a_image").ToString();
            }
        }

        private void image_DragLeave(object sender, System.Windows.DragEventArgs e)
        {
            drop_txt.Background = System.Windows.Media.Brushes.Transparent;
            drop_txt.Text = System.Windows.Application.Current.FindResource("input_drop").ToString();
        }
    }
}
