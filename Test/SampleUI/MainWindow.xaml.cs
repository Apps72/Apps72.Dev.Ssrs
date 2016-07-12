using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Apps72.Dev.Ssrs.ReportViewer.SampleUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnDisplay_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow report = new ReportWindow(new Uri(txtUrl.Text), txtPath.Text);            
            report.ShowDialog();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "Document"; 
            dlg.DefaultExt = ".pdf"; 
            dlg.Filter = "PDF documents (.pdf)|*.pdf";

            if (dlg.ShowDialog() == true)
            {
                ReportWindow report = new ReportWindow(new Uri(txtUrl.Text), txtPath.Text);
                report.Save(dlg.FileName, ReportSaveFormat.Pdf);
            }
        }
    }
}
