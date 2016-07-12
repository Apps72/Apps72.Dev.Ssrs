using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Printing;

namespace Apps72.Dev.Ssrs.ReportViewer
{
    /// <summary>
    /// Display the Microsoft Report Viewer
    /// </summary>
    /// <example>
    /// <code>
    ///     ReportWindow report = new ReportWindow("C:\\MyReport.pdf", "DataSet1", data);
    ///     report.Save("C:\\MyReport.pdf", ReportSaveFormat.Pdf);
    /// </code>
    /// <code>
    ///     ReportWindow report = new ReportWindow("C:\\MyReport.pdf", "DataSet1", data) {Owner = this};
    ///     report.ShowDialog();
    /// </code>
    /// <code>
    ///     ReportWindow report = new ReportWindow() {Owner = this};
    ///             
    ///     report.ReportFileName = "C:\\MyReport.rdlc";
    ///     report.ReportDataSources = new ReportDataSource[1];
    ///     report.ReportDataSources[0] = new ReportDataSource();
    ///     report.ReportDataSources[0].Name = "DataSet1";
    ///     report.ReportDataSources[0].Value = data.ToArray();
    ///
    ///     report.ShowDialog();
    ///     report.Save("C:\\MyReport.pdf", ReportSaveFormat.Pdf);
    /// </code>
    /// </example>
    public partial class ReportWindow : Window
    {
        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the Report Viewer Windows
        /// </summary>
        public ReportWindow()
        {
            InitializeComponent();

            // Initializes the report processes
            this.Loaded += (sender, e) =>
            {
                reportControl.Refresh();
            };

            // Finalizes the report processes
            this.Unloaded += (sender, e) =>
            {
                reportControl.RollbackCulture();
            };
        }

        /// <summary>
        /// Initialize a new instance of Report Window
        /// </summary>
        /// <param name="reportFileName">Report File Name (full path)</param>
        /// <param name="dataSourceName">Data source name</param>
        /// <param name="dataSourceValue">Data content</param>
        public ReportWindow(string reportFileName, string dataSourceName, object dataSourceValue)
            : this()
        {
            reportControl.LoadReport(reportFileName, dataSourceName, dataSourceValue);
        }

        /// <summary>
        /// Initialize a new instance of Report Window
        /// </summary>
        /// <param name="serverUrl">Report Server URL (http://localhost/ReportServer)</param>
        /// <param name="reportPath">Report file name (/MyDocument/MyReport)</param>
        /// <param name="parameters">Parameters list</param>
        public ReportWindow(Uri serverUrl, string reportPath, ReportParameter[] parameters)
            : this()
        {
            reportControl.LoadReport(serverUrl, reportPath, parameters);
        }

        /// <summary>
        /// Initialize a new instance of Report Window
        /// </summary>
        /// <param name="serverUrl">Report Server URL (http://localhost/ReportServer)</param>
        /// <param name="reportPath">Report file name (/MyDocument/MyReport)</param>
        public ReportWindow(Uri serverUrl, string reportPath)
            : this()
        {
            reportControl.LoadReport(serverUrl, reportPath, null);
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets the Report filename. 
        /// In local mode, sets the full filename (C:\MyDocuments\MyReport.rdlc).
        /// In remote mode, sets the relative report url without extension (/MyDocument/MyReport).
        /// </summary>
        public string ReportFileName 
        {
            get
            {
                return reportControl.ReportFileName;
            }
            set
            {
                reportControl.ReportFileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the Report Server URL to display a remote report.
        /// Sets this property to null to activate the local render mode.
        /// </summary>
        public Uri ReportServerUrl
        {
            get
            {
                return reportControl.ReportServerUrl;
            }
            set 
            {
                reportControl.ReportServerUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets the DataSources items
        /// </summary>
        public ReportDataSource[] ReportDataSources 
        {
            get
            {
                return reportControl.ReportDataSources;
            }
            set 
            {
                reportControl.ReportDataSources = value;
            }
        }

        /// <summary>
        /// Gets or sets the Parameters items
        /// </summary>
        public ReportParameter[] ReportParameters
        {
            get
            {
                return reportControl.ReportParameters;
            }
            set
            {
                reportControl.ReportParameters = value;
            }
        }

        /// <summary>
        /// Gets all printers name (to use with the Print method) installed on this computer (local and network printers)
        /// </summary>
        public string[] InstalledPrinters
        {
            get
            {
                return reportControl.InstalledPrinters;
            }
        }

        /// <summary>
        /// Gets or sets True to display the LocalReportParametersWindow form if the RDLC constain a least one parameter.
        /// </summary>
        public ReportLocalParameterSource LocalParametersSource
        {
            get
            {
                return reportControl.LocalParametersSource;
            }
            set
            {
                reportControl.LocalParametersSource = value;
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Display the report
        /// </summary>
        public virtual void Refresh()
        {
            reportControl.Refresh();
        }

        /// <summary>
        /// Save the current report to the specified file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="format">File format</param>
        public virtual void Save(string fileName, ReportSaveFormat format)
        {
            reportControl.Save(fileName, format);
        }

        /// <summary>
        /// Display the Print window to select an installed printer, and print the current report.
        /// </summary>
        public virtual void PrintDialog()
        {
            reportControl.PrintDialog();
        }

        /// <summary>
        /// Print the current report to the specified printer
        /// </summary>
        /// <param name="printerName">Printer name (existing in Windows)</param>
        /// <param name="orientation">Type of paper orientation: landscape or portrait</param>
        public virtual void Print(string printerName, ReportOrientation orientation)
        {
            reportControl.Print(printerName, orientation);
        }


        #endregion

        #region DESTRUCTORS

        /// <summary>
        /// Finalizes the window
        /// </summary>
        ~ReportWindow()
        {
            reportControl.RollbackCulture();
        }

        #endregion       

    }
}
