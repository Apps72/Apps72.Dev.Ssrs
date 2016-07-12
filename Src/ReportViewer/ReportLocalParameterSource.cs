using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apps72.Dev.Ssrs.ReportViewer
{
    /// <summary>
    /// Type of local parameters
    /// </summary>
    public enum ReportLocalParameterSource
    {
        /// <summary>
        /// To use only DataSource and Parameters specified by code.
        /// </summary>
        None,
        /// <summary>
        /// To use parameters from local file, but specified datasources.
        /// </summary>
        ParametersOnlyInLocalFile,
        /// <summary>
        /// To use datasources from local file, but specified parameters.
        /// </summary>
        DataSourceOnlyInLocalFile,
        /// <summary>
        /// To use datasources and parameters from local file.
        /// </summary>
        BothInLocalFile,
    }
}
