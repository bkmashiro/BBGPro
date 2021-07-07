using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// export.xaml 的交互逻辑
    /// </summary>
    public partial class export : Page
    {
        public export(AffairHandler affairHandler)
        {
            InitializeComponent();
            AffairHandler = affairHandler;
            progress_grid.Visibility = Visibility.Collapsed;
        }

        public AffairHandler AffairHandler { get; }

        private void block_preview_btn_Click(object sender, RoutedEventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "保存文件";
                sfd.DefaultExt = "bmp";
                sfd.Filter = "图像文件|*.bmp";

                sfd.FileName = AffairHandler.ImageService.imageName + "_block";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    progress_grid.Visibility = Visibility.Visible;

                    progress_txt.Dispatcher.Invoke(new Action(() =>
                    {
                        progress_txt.Text = System.Windows.Application.Current.FindResource("preview_progress_saving").ToString();
                    }));
                    Task t = new Task(new Action(() =>
                    {
                        string savePath = sfd.FileName;
                        ImgPreview imgPreview = new ImgPreview();
                        imgPreview.BindProgress(this.progress_bar, this.progress_txt);
                        imgPreview.Init(AffairHandler.BlockInfoManager, AffairHandler.ImageService.width, AffairHandler.ImageService.length, AffairHandler.mapGenerator.result);
                        imgPreview.GetBitamp().Save(savePath);
                        progress_txt.Dispatcher.Invoke(new Action(() =>
                        {
                            progress_txt.Text = System.Windows.Application.Current.FindResource("preview_progress_finished").ToString();
                        }));
                        VanishWait(3000);
                    }));
                    t.Start();
                }
            }
        }

        private void VanishWait(int t)
        {
            Task t2 = new Task(() =>
            {
                Thread.Sleep(t);
                progress_grid.Dispatcher.Invoke(new Action(() =>
                {
                    progress_grid.Visibility = Visibility.Hidden;
                }));
                progress_txt.Dispatcher.Invoke(new Action(() =>
                {
                    progress_txt.Text = "";
                }));
            });
            t2.Start();
        }

        private void TextTo(string text)
        {
            progress_grid.Dispatcher.Invoke(new Action(() =>
            {
                progress_grid.Visibility = Visibility.Visible;
            }));
            progress_txt.Dispatcher.Invoke(new Action(() =>
            {
                progress_txt.Text = text;
            }));
        }

        private void map_preview_btn_Click(object sender, RoutedEventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "保存文件";
                sfd.DefaultExt = "bmp";
                sfd.Filter = "图像文件|*.bmp";

                sfd.FileName = AffairHandler.ImageService.imageName + "_map";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    TextTo(System.Windows.Application.Current.FindResource("preview_progress_saving").ToString());
                    ImgPreview imgPreview = new ImgPreview();
                    imgPreview.BindProgress(this.progress_bar, this.progress_txt);
                    imgPreview.Init(AffairHandler.BlockInfoManager, AffairHandler.ImageService.width, AffairHandler.ImageService.length, AffairHandler.mapGenerator.result);
                    if (AffairHandler.is3DMode)
                    {
                        imgPreview.GetMap(AffairHandler.mapGenerator.height).Save(sfd.FileName);
                    }
                    else
                    {
                        imgPreview.GetMap().Save(sfd.FileName);
                    }
                    TextTo(System.Windows.Application.Current.FindResource("preview_progress_finished").ToString());
                    VanishWait(3000);
                }
            }
        }

        private void schematic_btn_Click(object sender, RoutedEventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "保存文件";
                sfd.DefaultExt = "schem";
                sfd.FileName = AffairHandler.ImageService.imageName;
                sfd.Filter = "原理图文件|*.schem";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Task task = new Task(new Action(() =>
                    {
                        TextTo(System.Windows.Application.Current.FindResource("preview_progress_saving").ToString());
                        foreach (var item in sfd.FileName)
                        {
                            if (item < 128)
                            {
                                System.Windows.Forms.MessageBox.Show(System.Windows.Application.Current.FindResource("exoprt_warn_ascii").ToString());
                                return;
                            }
                        }

                        Schematic schematic = new Schematic();
                        schematic.ReadBlockDatas(AffairHandler.BlockInfoManager.blockDatas_higher.ToArray());
                        if (AffairHandler.is3DMode)
                        {
                            var p = AffairHandler.mapGenerator.Get3DResult();
                            schematic.Read3D(p.Item1, p.Item2);
                        }
                        else
                        {
                            schematic.Read2D(AffairHandler.mapGenerator.Get2DResult());
                        }
                        schematic.Save(sfd.FileName);
                        TextTo(System.Windows.Application.Current.FindResource("preview_progress_finished").ToString());
                        VanishWait(3000);
                    }));
                    task.Start();
                }
            }

        }

        private void bks_btn_Click(object sender, RoutedEventArgs e)
        {
            if (AffairHandler.is3DMode)
            {
                System.Windows.MessageBox.Show("目前不支持3D地图画导出！");
                return;
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "保存文件";
                sfd.DefaultExt = "bks";
                sfd.FileName = AffairHandler.ImageService.imageName;
                sfd.Filter = "BKS文件|*.bks";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Task task = new Task(new Action(() =>
                    {
                        TextTo(System.Windows.Application.Current.FindResource("preview_progress_saving").ToString());
                        BKS bKS = new BKS();
                        
                        bKS.Init(AffairHandler.ImageService.imageName, AffairHandler.ImageService.width, 1, AffairHandler.ImageService.length, AffairHandler.BlockInfoManager.blockDatas_higher.ToArray(), AffairHandler.mapGenerator.blockUsage, AffairHandler.mapGenerator.result);
                        bKS.G(sfd.FileName);
                        TextTo(System.Windows.Application.Current.FindResource("preview_progress_finished").ToString());
                        VanishWait(3000);
                    }));
                    task.Start();
                }
            }



        }

        private void csv_btn_Click(object sender, RoutedEventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "保存文件";
                sfd.DefaultExt = "csv";
                sfd.FileName = AffairHandler.ImageService.imageName;
                sfd.Filter = "csv文件|*.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    Task task = new Task(new Action(() =>
                    {
                        TextTo(System.Windows.Application.Current.FindResource("preview_progress_saving").ToString());
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < AffairHandler.mapGenerator.result.GetLength(0); i++)
                        {
                            for (int j = 0; j < AffairHandler.mapGenerator.result.GetLength(1); j++)
                            {
                                sb.Append($"{AffairHandler.BlockInfoManager.blockDatas_higher[AffairHandler.mapGenerator.result[i,j]].name},");
                            }
                            sb.Append(Environment.NewLine);
                        }
                        StreamWriter writer = new StreamWriter(sfd.FileName); //文件的保存路径
                        writer.Write(sb.ToString());
                        writer.Flush();
                        writer.Close();
                        TextTo(System.Windows.Application.Current.FindResource("preview_progress_finished").ToString());
                        VanishWait(3000);
                    }));
                    task.Start();
                }
            }
        }

        private void butto_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void count_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();


            ConciseBlockData2D[] conciseBlockData2D = AffairHandler.BlockInfoManager.GetConciseBlockData2D();
            List<byte> names = new List<byte>();
            List<int> counts = new List<int>();
            for (int i = 0; i < AffairHandler.mapGenerator.blockUsage.Length; i++)
            {
                if (AffairHandler.mapGenerator.blockUsage[i] != 0)
                {
                    if (!names.Contains(conciseBlockData2D[i].classId))
                    {
                        names.Add(conciseBlockData2D[i].classId);
                        counts.Add(AffairHandler.mapGenerator.blockUsage[i]);
                    }
                    else
                    {
                        counts[names.Find(o => o == conciseBlockData2D[i].classId)] += AffairHandler.mapGenerator.blockUsage[i];
                    }
                }
            }
            for (int i = 0; i < names.Count; i++)
            {
                sb.Append($"{AffairHandler.BlockInfoManager.blockDatas_higher[names[i] - 1].name}:\r");
                sb2.Append($"{counts[i]}\r");
            }


            cnt_txt.Text = sb.ToString();
            cnt_txt2.Text = sb2.ToString();
        }
    }
}
