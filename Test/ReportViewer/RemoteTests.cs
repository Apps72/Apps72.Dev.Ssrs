using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing.Printing;

namespace Apps72.Dev.Ssrs.ReportViewer.Tests
{
    [TestClass]
    public class RemoteTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var report = new ReportWindow();
            report.ReportServerUrl = new Uri("http://hiper.trasys.be/ReportServer");
            report.ReportFileName = "/MecarFtd/FiringRequest_EN";
            report.AddParameter("RequestId", "1", isVisible: false);

            report.Save("C:\\_Temp\\Test.pdf", ReportSaveFormat.Pdf);
            report.Print("Send To OneNote 2016", ReportOrientation.Landscape, Duplex.Default, false);
            report.ShowDialog();
        }
    }
}