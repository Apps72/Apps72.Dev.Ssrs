using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Apps72.Dev.Ssrs.ReportViewer.Tests
{
    [TestClass]
    public class RemoteTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var report = new ReportWindow();
            report.ReportServerUrl = new Uri("http://localhost/ReportServer");
            report.ReportFileName = "/MyFolder/MyReport";
            report.AddParameter("Id", "1", false);

            report.Save("Test.pdf", ReportSaveFormat.Pdf);
            report.Print("Microsoft Print to PDF", ReportOrientation.Landscape);
        }
    }
}