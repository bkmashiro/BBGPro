#define iIsDebug
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        string Culture = "zh-cn";
        //string Culture = "en";
        public MainWindow()
        {
            InitializeComponent();
            List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
            foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
            {
                dictionaryList.Add(dictionary);
            }
            string requestedCulture = string.Format(@"{0}.xaml", Culture);
            ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => !(d.Source is null) && d.Source.OriginalString.Equals(requestedCulture));
            if (resourceDictionary == null)
            {
                requestedCulture = @"zh-cn.xaml";
                resourceDictionary = dictionaryList.FirstOrDefault(d => d.Source.OriginalString.Equals(requestedCulture));
            }
            if (resourceDictionary != null)
            {
                Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            CheckUpdate();
        }
        AffairHandler affairHandler = new AffairHandler();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if IsDebug


            BlockInfoManager blockInfoManager = new BlockInfoManager();
            blockInfoManager.Init();
            blockInfoManager.DisableAllColor();
            blockInfoManager.AddColor(0);


            ImageService imageService = new ImageService();
            //imageService.LoadImage(@"test2.gif");
            imageService.LoadImage(@"C:\Users\Administrator\Desktop\blockdata\2 SAND\白桦原木（竖直）.bmp");
            //C:\Users\Administrator\Desktop\blockdata\2 SAND\白桦原木（竖直）.bmp

            MapGenerator mapGenerator = new MapGenerator();
            mapGenerator.Init(imageService.GetImageArray());
            mapGenerator.Generate(blockInfoManager.GetConciseBlockData3D());
            var p = mapGenerator.Get3DResult();

            Schematic schematic = new Schematic();
            //schematic.Read2D(p);
            schematic.Read3D(p.Item1, p.Item2);
            schematic.ReadBlockDatas(blockInfoManager.blockDatas_higher.ToArray());
            schematic.Save();
            Console.WriteLine();

#endif
            affairHandler.InitAll();
            affairHandler.BindTransitioner(MainTransitioner);
            Task task = new Task(() =>
            {
                App.Current.Dispatcher.Invoke(new Action(() =>
                {
                    pg0.Content = new Page1(affairHandler);
                    pg1.Content = new Slides.Input(affairHandler);
                    pg2.Content = new Slides.ChooseColor(affairHandler);
                    pg3.Content = new Slides.generateMap(affairHandler);
                    pg4.Content = new Slides.export(affairHandler);
                }));
            });
            task.Start();

            IsStartGenerated = true;
            IsChooseImageGenerated = true;
            IsChooseBlockGenerated = true;
            IsGenerateGenerated = true;
        }
        bool IsStartGenerated = false;
        private void guide_start_Click(object sender, RoutedEventArgs e)
        {
            if (IsStartGenerated)
            {
                affairHandler.PageTo(0);
            }
            else
            {
                pg0.Content = new Page1(affairHandler);
                affairHandler.PageTo(0);
                IsStartGenerated = true;
            }
            DrawerHost.IsLeftDrawerOpen = false;
        }
        bool IsChooseImageGenerated = false;
        private void guide_choose_image_btn_Click(object sender, RoutedEventArgs e)
        {
            if (IsChooseImageGenerated)
            {
                affairHandler.PageTo(1);
            }
            else
            {
                pg1.Content = new Slides.Input(affairHandler);
                affairHandler.PageTo(1);
                IsChooseImageGenerated = true;
            }
            DrawerHost.IsLeftDrawerOpen = false;
        }
        bool IsChooseBlockGenerated = false;
        private void guide_choose_block_btn_Click(object sender, RoutedEventArgs e)
        {
            if (IsChooseBlockGenerated)
            {
                affairHandler.PageTo(2);
            }
            else
            {
                pg2.Content = new Slides.ChooseColor(affairHandler);
                affairHandler.PageTo(2);
                IsChooseBlockGenerated = true;
            }
            DrawerHost.IsLeftDrawerOpen = false;
        }

        bool IsGenerateGenerated = false;
        private void guide_generate_btn_Click(object sender, RoutedEventArgs e)
        {
            if (IsGenerateGenerated)
            {
                affairHandler.PageTo(3);
            }
            else
            {
                pg3.Content = new Slides.generateMap(affairHandler);
                affairHandler.PageTo(3);
                IsGenerateGenerated = true;
            }
            DrawerHost.IsLeftDrawerOpen = false;
        }

        bool IsExportGenerated = false;
        private void guide_export_btn_Click(object sender, RoutedEventArgs e)
        {
            if (IsExportGenerated)
            {
                affairHandler.PageTo(4);
            }
            else
            {
                pg4.Content = new Slides.export(affairHandler);
                affairHandler.PageTo(4);
                IsExportGenerated = true;
            }
            DrawerHost.IsLeftDrawerOpen = false;
        }
        bool IsBatchGenerated = false;
        private void guide_batch_btn_Click(object sender, RoutedEventArgs e)
        {
            if (IsBatchGenerated)
            {
                affairHandler.PageTo(5);
            }
            else
            {
                pg5.Content = new Slides.Batch(affairHandler);
                affairHandler.PageTo(5);
                IsBatchGenerated = true;
            }
            DrawerHost.IsLeftDrawerOpen = false;
        }
        string Version = "1.0.0.0";
        int[] localVersion = { 1, 0, 0, 0 };

        private void CheckUpdate()
        {
            Task t = new Task(new Action(() =>
            {
                try
                {
                    string updateString = @"https://gitee.com/bakamashiro/bbg-pro-update/raw/master/bbgupdate.txt";
                    WebClient wc = new WebClient();
                    Stream s = wc.OpenRead(updateString);
                    StreamReader sr = new StreamReader(s, Encoding.UTF8);
                    string datas = sr.ReadToEnd();
                    string[] data = datas.Split('\r');
                    string ver = data[1].Trim().Split(':')[1].Trim();
                    string link = data[2].Trim().Split(':')[1].Trim();
                    string file = data[3].Trim().Split(':')[1].Trim();
                    string[] vers = ver.Split('.');
                    int[] verInts = new int[4];
                    for (int i = 0; i < 4; i++)
                    {
                        verInts[i] = int.Parse(vers[i]);
                    }
                    for (int i = 0; i < 4; i++)
                    {
                        if (localVersion[i] < verInts[i])
                        {
                            //Update
                            System.Windows.Forms.MessageBox.Show("BBG Pro惊现更新！点击主页面下方 “教程/更新” 按钮去更新！");
                            break;
                        }
                    }
                    Console.WriteLine();

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show($"看起来BBG Pro更新服务遇到了问题。Error:{ex.Message} 我们的主页：bakamashiro.xyz  Github:github.com/bkmashiro/BBGPro");
                    throw;
                }
            }));
            t.Start();
        }       
    }
}
