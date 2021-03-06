# SQL Server Reporting Services - Report Viewer

## Introduction

This C# library is a standalone WPF library helping developers to display, save and print, 
local or remote SQL Server Reports (including Microsoft ReportViewer libraries v12).

## Samples

### How to save and display a remote report.

```csharp
ReportWindow report = new ReportWindow(new Uri("http://localhost/ReportServer"), "/MyDocument/MyReport");    
report.Save("C:\\MyReport.pdf", ReportSaveFormat.Pdf);
report.ShowDialog();
```

### How to save and display a local Report.

```csharp
ReportWindow report = new ReportWindow("C:\\MyDefinition.rdlc", "DataSet1", data);
report.Save("C:\\MyReport.pdf", ReportSaveFormat.Pdf);
report.ShowDialog();
```

### How to display a local report, using multiple data sets.

```csharp
ReportWindow report = new ReportWindow();
        
report.ReportFileName = "C:\\MyReport.rdlc";
report.ReportDataSources = new ReportDataSource[1];
report.ReportDataSources[0] = new ReportDataSource();
report.ReportDataSources[0].Name = "DataSet1";
report.ReportDataSources[0].Value = data.ToArray();

report.ShowDialog();
```

## Change Logs

1.3 - Add **Duplex** and **IsColored** argument to print Recto/Verso and in color.

1.2 - Include **Print** and **Save** command for remote reports.

1.1 - All basic features.