using System;
using System.Windows;

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

            this.Loaded += (sender, e) =>
            {
                txtUrl.Text = Registry.GetStaticValue("Options", "Url");
                txtPath.Text = Registry.GetStaticValue("Options", "Path");

                if (String.IsNullOrEmpty(txtUrl.Text)) txtUrl.Text = "http://YourServer/ReportServer";
                if (String.IsNullOrEmpty(txtPath.Text)) txtPath.Text = "/YourFolder/PageName";
            };

            this.Closing += (sender, e) =>
            {
                Registry.SetStaticValue("Options", "Url", txtUrl.Text);
                Registry.SetStaticValue("Options", "Path", txtPath.Text);
            };
        }

        /// <summary>
        /// Display the report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDisplay_Click(object sender, RoutedEventArgs e)
        {
            ReportWindow report = new ReportWindow(new Uri(txtUrl.Text), txtPath.Text);            
            report.ShowDialog();
        }

        /// <summary>
        /// Save the report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
