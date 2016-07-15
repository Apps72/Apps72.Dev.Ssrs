# SQL Server Reporting Services - Report Viewer

## Introduction

This C# library is a WPF library helping developers to display, save and print, local or remote SQL Server Reports.

## Samples

### How to display a remote report.

    ReportWindow report = new ReportWindow(new Uri("http://localhost/ReportServer"), "/MyDocument/MyReport");    
    report.ShowDialog();

### How to save and display a local Report.

    ReportWindow report = new ReportWindow("C:\\MyReport.pdf", "DataSet1", data);
    report.Save("C:\\MyReport.pdf", ReportSaveFormat.Pdf);
    report.ShowDialog();

### How to display a local report, using multiple data sets.

    ReportWindow report = new ReportWindow();
        
    report.ReportFileName = "C:\\MyReport.rdlc";
    report.ReportDataSources = new ReportDataSource[1];
    report.ReportDataSources[0] = new ReportDataSource();
    report.ReportDataSources[0].Name = "DataSet1";
    report.ReportDataSources[0].Value = data.ToArray();

    report.ShowDialog();