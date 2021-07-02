﻿using System;
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

        }

        private void start_Click(object sender, RoutedEventArgs e)
        {
            AffairHandler.PageTo(1);
        }
    }
}
