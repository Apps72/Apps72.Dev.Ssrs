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
using System.Windows.Controls;
using Apps72.Dev.Ssrs.ReportViewer.Local;
using System.Collections.Specialized;
using System.Globalization;

namespace Apps72.Dev.Ssrs.ReportViewer
{
    /// <summary>
    /// Display the Microsoft Report Viewer
    /// </summary>
    /// <example>
    /// <code>
    ///     report.LoadReport("C:\\MyReport.rdlc", "DataSet1", data);
    ///     report.Save("C:\\MyReport.pdf", ReportSaveFormat.Pdf);
    /// </code>
    /// <code>
    ///     report.DisplayReport("C:\\MyReport.rdlc", "DataSet1", data);
    /// </code>
    /// <code>
    ///     report.ReportFileName = "C:\\MyReport.rdlc";
    ///     report.ReportDataSources = new ReportDataSource[1];
    ///     report.ReportDataSources[0] = new ReportDataSource();
    ///     report.ReportDataSources[0].Name = "DataSet1";
    ///     report.ReportDataSources[0].Value = data.ToArray();
    ///     report.Save("C:\\MyReport.pdf", ReportSaveFormat.Pdf);
    /// </code>
    /// <code>
    ///     report.ReportServerUrl = new Uri("http://localhost/ReportServer");
    ///     report.ReportFileName = "/MyDocument/MyReport";
    ///     report.Refresh();
    /// </code>
    /// </example>
    public partial class ReportControl : UserControl
    {
        #region DECLARATIONS

        private dynamic rptViewer;
        private bool _isReportGenerated = false;
        private int _currentPageIndex = 0;
        private IList<Stream> _streams = null;
        private List<string> _streamsFileName = null;
        private System.Globalization.CultureInfo _currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        private System.Globalization.CultureInfo _currentUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of the Report Viewer Control
        /// </summary>
        public ReportControl() : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Report Viewer Control
        /// </summary>
        /// <param name="userName">SQL Server Username to execute SQL Queries</param>
        /// <param name="password">SQL Server Password to execute SQL Queries</param>
        public ReportControl(string userName, string password)
        {
            InitializeComponent();

            // Default value
            this.ReportViewerWinFormsAssemblyName = "Microsoft.ReportViewer.WinForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";
            this.UserName = userName;
            this.Password = password;
            this.LocalParametersSource = ReportLocalParameterSource.None;
            this.ReportServerUrl = null;

            // Creates an instance of ReportViewer Assembly
            this.ReportInstance = new ReportInstance(this);

            // Create the ReportViewer Object
            rptViewer = this.ReportInstance.NewReportViewer;

            // Sets the report viewer to this window
            WindowsFormsHost windowsFormsHost = new WindowsFormsHost();
            windowsFormsHost.Child = rptViewer;
            this.Content = windowsFormsHost;
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets or sets the default user used in the ConnectionString.
        /// If this property is set (not Empty) and if the ConnectionString is in Integrated Security (Trusted Mode), 
        /// then the ConnectionString will constain "User ID=[UserName];Password=[Password]".
        /// In other case, this property will be ignored.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the default password used in the ConnectionString
        /// If this property is set (not Empty) and if the ConnectionString is in Integrated Security (Trusted Mode), 
        /// then the ConnectionString will constain "User ID=[UserName];Password=[Password]".
        /// In other case, this property will be ignored.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the Identifier if Microsoft.ReportViewer.WinForms assembly.
        /// Exammple: "Microsoft.ReportViewer.WinForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91"
        /// </summary>
        public string ReportViewerWinFormsAssemblyName { get; set; }

        /// <summary>
        /// Gets or sets the Report filename. 
        /// In local mode, sets the full filename (C:\MyDocuments\MyReport.rdlc).
        /// In remote mode, sets the relative report url without extension (/MyDocument/MyReport).
        /// </summary>
        public string ReportFileName { get; set; }

        /// <summary>
        /// Gets or sets the Report Server URL to display a remote report.
        /// Sets this property to null to activate the local render mode.
        /// </summary>
        /// <example>
        /// MyReportControl.ReportServerUrl = new Uri("http://localhost/ReportServer");
        /// </example>
        public Uri ReportServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the DataSources items
        /// </summary>
        public ReportDataSource[] ReportDataSources { get; set; }

        /// <summary>
        /// Gets or sets the Parameters items
        /// </summary>
        public ReportParameter[] ReportParameters { get; set; }

        /// <summary>
        /// Gets all printers name (to use with the Print method) installed on this computer (local and network printers)
        /// </summary>
        public string[] InstalledPrinters
        {
            get
            {
                return PrinterSettings.InstalledPrinters.Cast<string>().ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the type of parameters content
        /// </summary>
        public ReportLocalParameterSource LocalParametersSource { get; set; }

        /// <summary>
        /// Gets a ReportViewer instance to used sub classes and methods
        /// </summary>
        internal ReportInstance ReportInstance { get; private set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Initializes parameters to display a local report
        /// </summary>
        /// <param name="reportFileName">Report File Name (full path)</param>
        /// <param name="dataSources">Data sources list</param>
        /// <param name="parameters">Parameters list</param>
        protected virtual void InitializeReport(string reportFileName, ReportDataSource[] dataSources, ReportParameter[] parameters)
        {
            // Sets properties
            this.ReportServerUrl = null;
            this.ReportFileName = reportFileName;
            this.ReportDataSources = ReportDataSources;
            this.ReportParameters = parameters;
        }

        /// <summary>
        /// Initializes parameters to display a local report
        /// </summary>
        /// <param name="reportFileName">Report File Name (full path)</param>
        /// <param name="dataSourceName">Data source name</param>
        /// <param name="dataSourceValue">Data content</param>
        protected virtual void InitializeReport(string reportFileName, string dataSourceName, object dataSourceValue)
        {
            List<ReportDataSource> data = new List<ReportDataSource>();
            data.Add(new ReportDataSource() { Name = dataSourceName, Value = dataSourceValue });

            // Sets properties
            this.ReportServerUrl = null;
            this.ReportFileName = reportFileName;
            this.ReportDataSources = data.ToArray();
            this.ReportParameters = null;
        }

        /// <summary>
        /// Initializes parameters to display a remote report
        /// </summary>
        /// <param name="serverUrl">Report Server URL (http://localhost/ReportServer)</param>
        /// <param name="reportPath">Report file name</param>
        /// <param name="parameters">Parameters list</param>
        protected virtual void InitializeReport(Uri serverUrl, string reportPath, ReportParameter[] parameters)
        {
            // Sets properties
            this.ReportServerUrl = serverUrl;
            this.ReportFileName = reportPath;
            this.ReportDataSources = null;
            this.ReportParameters = parameters;
        }

        /// <summary>
        /// Display the specified local report.
        /// </summary>
        /// <param name="reportFileName">Report File Name (full path)</param>
        /// <param name="dataSourceName">Data source name</param>
        /// <param name="dataSourceValue">Data content</param>
        public virtual void DisplayReport(string reportFileName, string dataSourceName, object dataSourceValue)
        {
            LoadReport(reportFileName, dataSourceName, dataSourceName);
            Refresh();
        }

        /// <summary>
        /// Display the specified remote report.
        /// </summary>
        /// <param name="serverUrl">Report Server URL (http://localhost/ReportServer)</param>
        /// <param name="reportPath">Report file name</param>
        /// <param name="parameters">Parameters list</param>
        public virtual void DisplayReport(Uri serverUrl, string reportPath, ReportParameter[] parameters)
        {
            LoadReport(serverUrl, reportPath, parameters);
            Refresh();
        }

        /// <summary>
        /// Load the specified local report in memory.
        /// Use Refresh method to display this report.
        /// </summary>
        /// <param name="reportFileName">Report File Name (full path)</param>
        /// <param name="dataSourceName">Data source name</param>
        /// <param name="dataSourceValue">Data content</param>
        public virtual void LoadReport(string reportFileName, string dataSourceName, object dataSourceValue)
        {
            InitializeReport(reportFileName, dataSourceName, dataSourceValue);
        }

        /// <summary>
        /// Load the specified remote report in memory.
        /// Use Refresh method to display this report.
        /// </summary>
        /// <param name="serverUrl">Report Server URL (http://localhost/ReportServer)</param>
        /// <param name="reportPath">Report file name</param>
        /// <param name="parameters">Parameters list</param>
        public virtual void LoadReport(Uri serverUrl, string reportPath, ReportParameter[] parameters)
        {
            InitializeReport(serverUrl, reportPath, parameters);
        }

        /// <summary>
        /// Display the report
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public virtual void Refresh()
        {
            #region LOCAL MODE

            if (this.ReportServerUrl == null)
            {
                if (DisplayLocalParameters())
                {
                    _isReportGenerated = false;

                    // Report parameters
                    rptViewer.LocalReport.ReportPath = this.ReportFileName;
                    rptViewer.LocalReport.Refresh();

                    // Data Source
                    if (this.ReportDataSources != null)
                    {
                        foreach (ReportDataSource source in this.ReportDataSources)
                        {
                            rptViewer.LocalReport.DataSources.Add(source.GetMicrosoftReportDataSource(this));
                        }
                    }

                    // Parameters
                    if (this.ReportParameters == null)
                    {
                        rptViewer.LocalReport.SetParameters(this.ReportInstance.NewReportParameterCollection);
                    }
                    else
                    {
                        // Usage of the NewReportParameterCollection to create the ReportParameter collection, 
                        // standard IEnumerable list does not work, and errors are not send back as we are using a dynamic keyword.
                        dynamic parameters = this.ReportInstance.NewReportParameterCollection;
                        foreach (ReportParameter p in this.ReportParameters)
                        {
                            parameters.Add(p.GetMicrosoftReportParameter(this));
                        }
                        rptViewer.LocalReport.SetParameters(parameters);

                    }

                    // Report configuration for local mode
                    rptViewer.ProcessingMode = this.ReportInstance.GetProcessingMode("Local");       // Microsoft.Reporting.WinForms.ProcessingMode.Local;
                    rptViewer.SetDisplayMode(this.ReportInstance.GetDisplayMode("PrintLayout"));     // Microsoft.Reporting.WinForms.DisplayMode.PrintLayout
                    rptViewer.ZoomMode = this.ReportInstance.GetZoomMode("PageWidth");               // Microsoft.Reporting.WinForms.ZoomMode.PageWidth            

                    rptViewer.RefreshReport();

                    _isReportGenerated = true;
                }
            }
            #endregion

            #region REMOTE MODE

            else
            {
                _isReportGenerated = false;

                // Report configuration for remote mode
                rptViewer.ProcessingMode = this.ReportInstance.GetProcessingMode("Remote");      // Microsoft.Reporting.WinForms.ProcessingMode.Remote;
                rptViewer.SetDisplayMode(this.ReportInstance.GetDisplayMode("PrintLayout"));     // Microsoft.Reporting.WinForms.DisplayMode.PrintLayout
                rptViewer.ZoomMode = this.ReportInstance.GetZoomMode("PageWidth");               // Microsoft.Reporting.WinForms.ZoomMode.PageWidth            

                // Report Source
                rptViewer.ServerReport.ReportServerUrl = this.ReportServerUrl;
                rptViewer.ServerReport.ReportPath = this.ReportFileName;

                // Credentials
                if (!String.IsNullOrEmpty(this.UserName))
                {
                    rptViewer.ServerReport.ReportServerCredentials.NetworkCredentials = new System.Net.NetworkCredential(this.UserName, this.Password);
                }

                // Parameters
                if (this.ReportParameters != null)
                {
                    dynamic parameters = this.ReportInstance.NewReportParameterList;
                    foreach (ReportParameter p in this.ReportParameters)
                    {
                        parameters.Add(p.GetMicrosoftReportParameter(this));
                    }
                    rptViewer.ServerReport.SetParameters(parameters);
                }

                rptViewer.RefreshReport();

                _isReportGenerated = true;
            }

            #endregion
        }

        /// <summary>
        /// Display the Local Report Parameters window to allow the user to fill these parameters
        /// </summary>
        /// <returns>
        /// Returns True if the Report Drawing process can continue. 
        /// Returns False if the user click on Cancel button.
        /// </returns>
        public virtual bool DisplayLocalParameters()
        {
            if (this.ReportServerUrl == null)
            {
                if (this.LocalParametersSource != ReportLocalParameterSource.None)
                {
                    // Check if the localreport contains some parameters
                    Local.LocalReportParametersWindow winParameters = new Local.LocalReportParametersWindow(this.ReportFileName, this.UserName, this.Password);
                    if (winParameters.LocalReport.Parameters.Count() > 0)
                    {
                        // Only show parameters dialog when we will use them 
                        if (this.LocalParametersSource == ReportLocalParameterSource.ParametersOnlyInLocalFile || this.LocalParametersSource == ReportLocalParameterSource.BothInLocalFile)
                        {

                            #region ShowDialog

                            if (winParameters.ShowDialog() == false)
                            {
                                return false;
                            }

                            // Retreive local properties and replace
                            LocalReportParameter[] localParameters = winParameters.LocalReport.Parameters;
                            this.ReportParameters = new ReportParameter[winParameters.LocalReport.Parameters.Count()];
                            int index = 0;
                            foreach (LocalReportParameter localParameter in localParameters)
                            {
                                ReportParameter parameter = this.ReportParameters.Where(x => (x != null) && x.Name == localParameter.Name).FirstOrDefault();

                                if (parameter == null)
                                {
                                    parameter = new ReportParameter();

                                    parameter.Values = new System.Collections.Specialized.StringCollection();
                                    parameter.Values.Add(Convert.ToString(localParameter.Values));
                                    parameter.Name = localParameter.Name;
                                    parameter.IsVisible = true;

                                    this.ReportParameters[index] = parameter;
                                    ++index;
                                }
                                else
                                {
                                    parameter.Values = new System.Collections.Specialized.StringCollection();
                                    parameter.Values.Add(Convert.ToString(localParameter.Values));
                                    parameter.Name = localParameter.Name;
                                }
                            }


                            if (this.LocalParametersSource == ReportLocalParameterSource.BothInLocalFile)
                            {
                                string connectionString = winParameters.LocalReport.DataSources[0].ConnectionString;
                                string commandText = winParameters.LocalReport.DataSources[0].CommandText;
                                string dataSetName = winParameters.LocalReport.DataSources[0].DataSetName;

                                // Retrieve all data from the SQL Database
                                using (var cmd = new SqlDatabaseCommand(connectionString))
                                {
                                    cmd.CommandText = commandText;

                                    foreach (LocalReportParameter localParameter in localParameters)
                                    {
                                        cmd.Parameters.AddWithValue(string.Format("@{0}", localParameter.Name), localParameter.Values);
                                    }

                                    System.Data.DataTable results = cmd.ExecuteTable();

                                    this.ReportDataSources = new ReportDataSource[1];
                                    this.ReportDataSources[0] = new ReportDataSource();
                                    this.ReportDataSources[0].Name = dataSetName;
                                    this.ReportDataSources[0].Value = results;
                                }

                            }

                            #endregion

                        }
                        else if (this.LocalParametersSource == ReportLocalParameterSource.DataSourceOnlyInLocalFile)
                        {
                            // Replace localParameter values by code
                            LocalReportParameter[] localParameters = winParameters.LocalReport.Parameters;
                            foreach (LocalReportParameter localParameter in localParameters)
                            {
                                ReportParameter parameter = this.ReportParameters.Where(x => x.Name == localParameter.Name).FirstOrDefault();
                                localParameter.Values = parameter.Values[0];
                            }

                            string connectionString = winParameters.LocalReport.DataSources[0].ConnectionString;
                            string commandText = winParameters.LocalReport.DataSources[0].CommandText;
                            string dataSetName = winParameters.LocalReport.DataSources[0].DataSetName;

                            using (var cmd = new SqlDatabaseCommand(connectionString, commandText))
                            {
                                foreach (LocalReportParameter localParameter in localParameters)
                                {
                                    cmd.Parameters.AddWithValue(string.Format("@{0}", localParameter.Name), localParameter.Values);
                                }

                                System.Data.DataTable results = cmd.ExecuteTable();

                                this.ReportDataSources = new ReportDataSource[1];
                                this.ReportDataSources[0] = new ReportDataSource();
                                this.ReportDataSources[0].Name = dataSetName;
                                this.ReportDataSources[0].Value = results;
                            }

                        }

                    }
                    else if (this.LocalParametersSource == ReportLocalParameterSource.DataSourceOnlyInLocalFile)
                    {
                        string connectionString = winParameters.LocalReport.DataSources[0].ConnectionString;
                        string commandText = winParameters.LocalReport.DataSources[0].CommandText;
                        string dataSetName = winParameters.LocalReport.DataSources[0].DataSetName;

                        using (var cmd = new SqlDatabaseCommand(connectionString, commandText))
                        {
                            System.Data.DataTable results = cmd.ExecuteTable();

                            this.ReportDataSources = new ReportDataSource[1];
                            this.ReportDataSources[0] = new ReportDataSource();
                            this.ReportDataSources[0].Name = dataSetName;
                            this.ReportDataSources[0].Value = results;
                        }
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Save the current report to the specified file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="format">File format</param>
        public virtual void Save(string fileName, ReportSaveFormat format)
        {
            if (rptViewer == null)
                throw new NullReferenceException("You must override this class and call the method InitializeReport first.");

            if (!_isReportGenerated)
                Refresh();

            string formatCode = string.Empty;

            switch (format)
            {
                case ReportSaveFormat.Excel:
                    formatCode = "Excel";
                    break;
                case ReportSaveFormat.Word:
                    formatCode = "Word";
                    break;
                case ReportSaveFormat.Pdf:
                    formatCode = "PDF";
                    break;
                default:
                    throw new ArgumentException("The format parameter must be specified.");
            }

            byte[] bytes;
            if (this.ReportServerUrl != null)
                bytes = rptViewer.ServerReport.Render(formatCode); 
            else
                bytes = rptViewer.LocalReport.Render(formatCode);

            FileStream fs = new FileStream(fileName, FileMode.Create);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();

            RollbackCulture();
        }

        /// <summary>
        /// Display the Print window to select an installed printer, and print the current report.
        /// </summary>
        public virtual void PrintDialog()
        {
            if (rptViewer == null)
                throw new NullReferenceException("You must override this class and call the method InitializeReport first.");

            if (!_isReportGenerated)
                Refresh();

            rptViewer.PrintDialog();

            RollbackCulture();
        }

        /// <summary>
        /// Print the current report to the specified printer
        /// </summary>
        /// <param name="printerName">Printer name (existing in Windows)</param>
        /// <param name="orientation">Type of paper orientation: landscape or portrait</param>
        public virtual void Print(string printerName, ReportOrientation orientation)
        {
            Print(printerName, orientation, Duplex.Default, true);
        }

        /// <summary>
        /// Print the current report to the specified printer
        /// </summary>
        /// <param name="printerName">Printer name (existing in Windows)</param>
        /// <param name="orientation">Type of paper orientation: landscape or portrait</param>
        /// <param name="duplex">Recto / Verso mode</param>
        /// <param name="isColored">True to print in color, False to print in gray</param>
        public virtual void Print(string printerName, ReportOrientation orientation, Duplex duplex, bool isColored)
        {
            if (rptViewer == null)
                throw new ArgumentException("You must override this class and call the method InitializeReport first.");

            //if (this.ReportServerUrl != null)
            //    throw new NotImplementedException("This method is not yet implemented for remote reports.");

            if (!_isReportGenerated)
                Refresh();

            // Report Generation
            if (this.ReportServerUrl != null)
                Export(rptViewer.ServerReport, orientation, ExportFormat.Emf);
            else
                Export(rptViewer.LocalReport, orientation, ExportFormat.Emf);

            // Printing
            _currentPageIndex = 0;
            PrintAllPages(printerName, orientation, duplex, isColored);

            // Stream closing
            if (_streams != null)
            {
                foreach (Stream stream in _streams)
                    stream.Close();
                _streams = null;
            }

            // Delete all temporary files
            foreach (string file in _streamsFileName)
            {
                if (System.IO.File.Exists(file))
                    System.IO.File.Delete(file);
            }

            RollbackCulture();
        }

        #endregion

        #region PRIVATES

        /// <summary>
        /// Replace the Application Cutlure to previous values to correct a bug in ReportViewer 10.0
        /// </summary>
        internal void RollbackCulture()
        {
            //System.Threading.Thread.CurrentThread.CurrentCulture = _currentCulture;
            //System.Threading.Thread.CurrentThread.CurrentUICulture = _currentUICulture;
        }

        /// <summary>
        /// Content report generation (one stream by page)
        /// </summary>
        /// <param name="name">Simple file name</param>
        /// <param name="fileNameExtension">Extension file name</param>
        /// <param name="encoding">Type of encoding</param>
        /// <param name="mimeType">Type of file (mime code)</param>
        /// <param name="willSeek"></param>
        /// <returns>Report stream</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "encoding"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "willSeek"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "mimeType")]
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            string tempPath = System.IO.Path.GetTempPath();
            if (!tempPath.EndsWith("\\")) tempPath += "\\";
            string filename = tempPath + name + "." + fileNameExtension;
            _streamsFileName.Add(filename);
            Stream stream = new FileStream(filename, FileMode.Create);
            _streams.Add(stream);
            return stream;
        }

        /// <summary>
        /// Exports the report to an EMF file
        /// </summary>
        /// <param name="report">local report reference</param>
        /// <param name="orientation">Landscape or portrait</param>
        /// <param name="format">Type of format</param>
        private void Export(object report, ReportOrientation orientation, ExportFormat format)
        {
            StringBuilder deviceInfo = new StringBuilder();
            deviceInfo.Append("<DeviceInfo>");

            switch (format)
            {
                case ExportFormat.Emf:
                    deviceInfo.Append("  <OutputFormat>EMF</OutputFormat>");
                    break;
                case ExportFormat.Tiff:
                    deviceInfo.Append("  <OutputFormat>TIFF</OutputFormat>");
                    deviceInfo.Append("  <DpiX>300</DpiX><DpiY>300</DpiY>");
                    deviceInfo.Append("  <PrintDpiX>300</PrintDpiX><PrintDpiY>300</PrintDpiY>");
                    break;
                case ExportFormat.Pdf:
                    deviceInfo.Append("  <OutputFormat>PDF</OutputFormat>");
                    break;

            }

            // Paper Size
            dynamic rpt = report;
            dynamic paperSize = rpt.GetDefaultPageSettings().PaperSize;
            dynamic margins = rpt.GetDefaultPageSettings().Margins;

            deviceInfo.Append($" <StartPage>0</StartPage>");
            deviceInfo.Append($" <EndPage>0</EndPage>");
            deviceInfo.Append($" <MarginTop>{ToInches(margins.Top)}</MarginTop>");
            deviceInfo.Append($" <MarginLeft>{ToInches(margins.Left)}</MarginLeft>");
            deviceInfo.Append($" <MarginRight>{ToInches(margins.Right)}</MarginRight>");
            deviceInfo.Append($" <MarginBottom>{ToInches(margins.Bottom)}</MarginBottom>");

            if (orientation == ReportOrientation.Landscape)
            {
                deviceInfo.Append($" <PageHeight>{ToInches(paperSize.Width)}</PageHeight>");
                deviceInfo.Append($" <PageWidth>{ToInches(paperSize.Height)}</PageWidth> ");
            }
            else
            {
                deviceInfo.Append($" <PageHeight>{ToInches(paperSize.Height)}</PageHeight>");
                deviceInfo.Append($" <PageWidth>{ToInches(paperSize.Width)}</PageWidth> ");
            }

            deviceInfo.Append("</DeviceInfo>");

            // Format
            string formatCode = format == ExportFormat.Pdf ? "PDF" : "Image";

            _streams = new List<Stream>();
            _streamsFileName = new List<string>();

            // Export a Local Report
            if (this.ReportServerUrl == null)
            {
                this.ReportInstance.InvokeReportRender(report, formatCode, deviceInfo.ToString(), "CreateStream");

                foreach (Stream stream in _streams)
                    stream.Position = 0;
            }

            // Export a Remote Report
            // https://github.com/juahidma/petnet-web/blob/master/PETNET/PETNET-INT/Clases/ReportPrintDocument.cs
            else
            {
                Stream pageStream = this.ReportInstance.InvokeReportServerRender(report, formatCode, deviceInfo.ToString(), isFirstPage: true);

                // The server returns an empty stream when moving beyond the last page.
                while (pageStream.Length > 0)
                {
                    _streams.Add(pageStream);

                    pageStream = this.ReportInstance.InvokeReportServerRender(report, formatCode, deviceInfo.ToString(), isFirstPage: false);
                }
            }
        }

        /// <summary>
        /// Capture an event to print the specified page in argument e
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Metafile pageImage = new Metafile(_streams[_currentPageIndex]);
            e.Graphics.DrawImage(pageImage, e.PageBounds);
            _currentPageIndex++;
            e.HasMorePages = (_currentPageIndex < _streams.Count);
        }

        /// <summary>
        /// Print all pages to the specified printer
        /// </summary>
        /// <param name="printerName">Printer name as know in Windows</param>    
        /// <param name="orientation">Type of orientation: landscape or portrait</param>
        private void PrintAllPages(string printerName, ReportOrientation orientation, Duplex duplex, bool isColored)
        {
            if (_streams == null || _streams.Count == 0)
                return;

            PrintDocument printDoc = new PrintDocument();
            printDoc.DefaultPageSettings.Landscape = (orientation == ReportOrientation.Landscape);
            printDoc.PrinterSettings.PrinterName = printerName;
            if (printDoc.PrinterSettings.SupportsColor)
            {
                printDoc.DefaultPageSettings.Color = isColored;
                printDoc.PrinterSettings.DefaultPageSettings.Color = isColored;
            }
            if (printDoc.PrinterSettings.CanDuplex)
            {
                printDoc.PrinterSettings.Duplex = duplex;
            }

            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new ArgumentException("This printer \"" + printerName + "\" is not available. Please, check you window configurations.");
            }

            printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
            printDoc.Print();
        }

        private static string ToInches(int hundrethsOfInch)
        {
            double inches = hundrethsOfInch / 100.0;
            return inches.ToString(CultureInfo.InvariantCulture) + "in";
        }

        #endregion

        #region DESTRUCTORS

        /// <summary>
        /// Finalizes the window
        /// </summary>
        ~ReportControl()
        {
            RollbackCulture();
        }

        #endregion

        #region ENUMERATIONS

        /// <summary>
        /// Exportation format
        /// </summary>
        private enum ExportFormat
        {
            Emf,
            Tiff,
            Pdf
        }

        #endregion
    }
}
