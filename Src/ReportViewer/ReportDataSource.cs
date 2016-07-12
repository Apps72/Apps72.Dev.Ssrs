using System;

namespace Apps72.Dev.Ssrs.ReportViewer
{
    /// <summary>
    /// Represents a data source for a report
    /// </summary>
    public class ReportDataSource
    {
        /// <summary>
        /// Gets or sets the name of the report
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value data set of the report
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets a Microsoft Reporting ReportDataSource with same Name and Value properties
        /// </summary>
        internal dynamic GetMicrosoftReportDataSource(ReportControl owner)
        {
            dynamic rds = owner.ReportInstance.NewReportDataSource;
            rds.Name = this.Name;
            rds.Value = this.Value;
            return rds;
        }
    }
}
