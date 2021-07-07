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
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1(AffairHandler affairHandler)
        {
            InitializeComponent();
            AffairHandler = affairHandler;
        }

        public AffairHandler AffairHandler { get; }

        private void learn_more_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/bkmashiro/BBGPro");
        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            AffairHandler.PageTo(1);
        }

        bool tmp_lang = false;

        private void switch_language_Click(object sender, RoutedEventArgs e)
        {
            if (tmp_lang)
            {
                string Culture = @"zh-cn";
                List<ResourceDictionary> dictionaryList = new List<ResourceDictionary>();
                foreach (ResourceDictionary dictionary in Application.Current.Resources.MergedDictionaries)
                {
                    dictionaryList.Add(dictionary);
                }
                string requestedCulture = string.Format(@"{0}.xaml", Culture);
                ResourceDictionary resourceDictionary = dictionaryList.FirstOrDefault(d => !(d.Source is null) && d.Source.OriginalString.Equals(requestedCulture) );
                if (resourceDictionary == null)
                {
                    requestedCulture = @"zh-cn.xaml";
                    resourceDictionary = dictionaryList.FirstOrDefault(d => !(d.Source is null) && d.Source.OriginalString.Equals(requestedCulture));
                }
                if (resourceDictionary != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }
            }
            else
            {
                string Culture = @"en";
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
                    resourceDictionary = dictionaryList.FirstOrDefault(d => !(d.Source is null) && d.Source.OriginalString.Equals(requestedCulture));
                }
                if (resourceDictionary != null)
                {
                    Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                    Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
                }
            }
            tmp_lang = !tmp_lang;
        }
    }
}
