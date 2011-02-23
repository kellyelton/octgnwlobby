﻿using System.Reflection;
using System.Windows;

namespace Octgn
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            clientVersionBlock.Text = Assembly.GetAssembly(typeof(AboutWindow)).GetName().Version.ToString();
            octgnVersionBlock.Text = OctgnApp.OctgnVersion.ToString();
        }

        private void GoToWebSite(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.octgn.net");
            e.Handled = true;
        }
    }
}
