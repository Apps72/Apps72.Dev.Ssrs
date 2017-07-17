using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Collections.Specialized;

namespace Apps72.Dev.Ssrs.ReportViewer
{
    /// <summary>
    /// Class to manage a Report Viewer instance dynamically
    /// </summary>
    internal class ReportInstance
    {
        private ReportControl _owner = null;

        /// <summary>
        /// Initializes a new instance of the Report Instance Manager
        /// </summary>
        internal ReportInstance(ReportControl owner)
        {           
            _owner = owner;

            FileInfo currentFileName = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Microsoft.ReportViewer.WinForms.dll"));
            FileInfo binFileName = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin\\Microsoft.ReportViewer.WinForms.dll"));
            string gacAssemblyName = "Microsoft.ReportViewer.WinForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91";

            // Load the ReportViewer Assembly from the current folder
            if (currentFileName.Exists)
            {
                this.Assembly = Assembly.LoadFrom(currentFileName.FullName);
            }
            
            // Load the ReportViewer Assembly from the bin folder
            else if (binFileName.Exists)
            {
                this.Assembly = Assembly.LoadFrom(binFileName.FullName);
            }

            // Load the ReportViewer Assembly from the Global Assembly Cache
            else
            {
                AssemblyName asm = new AssemblyName(gacAssemblyName);
                this.Assembly = Assembly.Load(asm);
            }
        }

        /// <summary>
        /// Gets a reference to ReportViewer assembly loaded from disk
        /// </summary>
        internal Assembly Assembly { get; private set; }

        /// <summary>
        /// Gets a reference to the class specified
        /// </summary>
        /// <param name="name">Name of class (ex. Microsoft.Reporting.WinForms.ReportViewer)</param>
        /// <returns>Reference to the specified class</returns>
        internal Type GetType(string name)
        {
            return this.Assembly.GetType(name);
        }

        /// <summary>
        /// Gets a reference to an object from specified type
        /// </summary>
        /// <param name="name">Name of class (ex. Microsoft.Reporting.WinForms.ReportViewer)</param>
        /// <returns>Object instancied</returns>
        internal dynamic GetNewInstance(string name)
        {
            return Activator.CreateInstance(this.GetType(name));            
        }

        /// <summary>
        /// Gets a reference to an object from specified generic type of specified generic type.
        /// </summary>
        /// <param name="name">Name of generic class (ex.System.Collections.Generic.List)</param>
        /// <param name="generic">Name of sub class (ex. Microsoft.Reporting.WinForms.ReportParameter)</param>
        /// <returns>Object instancied</returns>
        internal dynamic GetNewInstance(string name, string generic)
        {           
            Type genericType = this.GetType(generic);
            Type nameType = Type.GetType(name + "`1").MakeGenericType(genericType);
            return Activator.CreateInstance(nameType);
        }

        /// <summary>
        /// Returns a new instance of ReportViewer class
        /// </summary>
        internal dynamic NewReportViewer
        {
            get
            {
                return this.GetNewInstance("Microsoft.Reporting.WinForms.ReportViewer");
            }
        }

        /// <summary>
        /// Returns a new instance of ReportParameterCollection class
        /// </summary>
        /// <returns>ReportParameterCollection</returns>
        internal dynamic NewReportParameterCollection
        {
            get
            {
                return this.GetNewInstance("Microsoft.Reporting.WinForms.ReportParameterCollection");
            }
        }

        /// <summary>
        /// Returns a new instance of ReportParameter class
        /// </summary>
        /// <returns>ReportParameter</returns>
        internal dynamic NewReportParameter
        {
            get
            {
                return this.GetNewInstance("Microsoft.Reporting.WinForms.ReportParameter");
            }
        }


        /// <summary>
        /// Returns a new instance of ReportParameter class
        /// </summary>
        /// <returns>ReportParameter</returns>
        internal dynamic NewReportParameterList
        {
            get
            {
                return this.GetNewInstance("System.Collections.Generic.List", "Microsoft.Reporting.WinForms.ReportParameter");
            }
        }

        /// <summary>
        /// Returns a new instance of ReportDataSource class
        /// </summary>
        /// <returns>ReportDataSource</returns>
        internal dynamic NewReportDataSource
        {
            get
            {
                return this.GetNewInstance("Microsoft.Reporting.WinForms.ReportDataSource");
            }
        }

        /// <summary>
        /// Return an item from ProcessingMode enumeration
        /// </summary>
        /// <param name="key">Key of enumeration value</param>
        /// <returns></returns>
        internal dynamic GetProcessingMode(string key)
        {
            return Enum.Parse(this.GetType("Microsoft.Reporting.WinForms.ProcessingMode"), key);
        }

        /// <summary>
        /// Return an item from DisplayMode enumeration
        /// </summary>
        /// <param name="key">Key of enumeration value</param>
        /// <returns></returns>
        internal dynamic GetDisplayMode(string key)
        {
            return Enum.Parse(this.GetType("Microsoft.Reporting.WinForms.DisplayMode"), key);
        }

        /// <summary>
        /// Return an item from ZoomMode enumeration
        /// </summary>
        /// <param name="key">Key of enumeration value</param>
        /// <returns></returns>
        internal dynamic GetZoomMode(string key)
        {
            return Enum.Parse(this.GetType("Microsoft.Reporting.WinForms.ZoomMode"), key);
        }


        /// <summary>
        /// Execute the LocalReport.Render method
        /// </summary>
        /// <param name="report"></param>
        /// <param name="format"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="callBackMethodName"></param>
        internal void InvokeReportRender(dynamic report, string format, string deviceInfo, string callBackMethodName)
        { 
            // Defines arguments to Render method
            Type localReportType = report.GetType();
            Delegate callback = Delegate.CreateDelegate(this.GetType("Microsoft.Reporting.WinForms.CreateStreamCallback"), _owner, callBackMethodName);
            Array warning = Array.CreateInstance(this.GetType("Microsoft.Reporting.WinForms.Warning"), 0);
            object[] args = new object[] { format, deviceInfo.ToString(), callback, warning };

            // Out parameters
            ParameterModifier pm = new ParameterModifier(4);
            pm[0] = false;      // format
            pm[1] = false;      // devinceInfo
            pm[2] = false;      // delegate to CallBackMethod
            pm[3] = true;       // Warning
            ParameterModifier[] pmods = { pm };

            // Executes the LocalReport.Render method
            localReportType.InvokeMember("Render", BindingFlags.InvokeMethod, null, report, args, pmods, null, null);           
                    
        }

        /// <summary>
        /// Execute the ServerReport.Render method
        /// </summary>
        /// <param name="report"></param>
        /// <param name="format"></param>
        /// <param name="deviceInfo"></param>
        /// <returns></returns>
        internal Stream InvokeReportServerRender(dynamic report, string format, string deviceInfo, bool isFirstPage)
        {
            // Defines arguments to Render method
            Type serverReportType = report.GetType();
            string mimeType = String.Empty;
            string fileExtension = String.Empty;

            // Generating Image renderer pages one at a time can be expensive.  In order
            // to generate page 2, the server would need to recalculate page 1 and throw it
            // away.  Using PersistStreams causes the server to generate all the pages in
            // the background but return as soon as page 1 is complete.
            var firstPageParameters = new NameValueCollection();
            firstPageParameters.Add("rs:PersistStreams", "True");

            // GetNextStream returns the next page in the sequence from the background process
            // started by PersistStreams.
            var nonFirstPageParameters = new NameValueCollection();
            nonFirstPageParameters.Add("rs:GetNextStream", "True");

            // Arguments
            object[] args = new object[] { format, deviceInfo, isFirstPage ? firstPageParameters : nonFirstPageParameters, mimeType, fileExtension};

            // Out parameters
            ParameterModifier pm = new ParameterModifier(5);
            pm[0] = false;      // format
            pm[1] = false;      // devinceInfo
            pm[2] = false;      // firstPageParameters / nonFirstPageParameters
            pm[3] = true;       // mimeType
            pm[4] = true;       // fileExtension
            ParameterModifier[] pmods = { pm };

            // Executes the ServerReport.Render method
            try
            {
                return serverReportType.InvokeMember("Render", BindingFlags.InvokeMethod, null, report, args, pmods, null, null);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                return null;
            }            
        }
    }
}
