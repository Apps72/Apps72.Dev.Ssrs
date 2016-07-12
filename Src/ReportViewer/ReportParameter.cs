using System;

namespace Apps72.Dev.Ssrs.ReportViewer
{
    /// <summary>
    /// Represents a parameter used by the report
    /// </summary>
    public class ReportParameter
    {
        /// <summary>
        /// Gets or sets the name of this paramter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a list of values for the parameter
        /// </summary>
        public System.Collections.Specialized.StringCollection Values { get; set; }

        /// <summary>
        /// Determines when the parameter is visible in the interface
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets a Microsoft Reporting ReportParameter with same Name, Value and Visible properties
        /// </summary>
        internal dynamic GetMicrosoftReportParameter(ReportControl owner)
        {
            dynamic rp = owner.ReportInstance.NewReportParameter;
            rp.Name = this.Name;
            rp.Values.Clear();
            foreach (string value in this.Values)
            {
                rp.Values.Add(value);
            }
            rp.Visible = this.IsVisible;
            return rp;
        }

    }
}
