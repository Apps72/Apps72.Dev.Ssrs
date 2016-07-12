using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Apps72.Dev.Ssrs.ReportViewer.Local
{
    /// <summary>
    /// Display the Report Properties Window
    /// </summary>
    /// <example>
    /// <code>
    ///     LocalReportParametersWindow winReportParameters = new LocalReportParametersWindow("C:\\MyReport.rdlc");
    ///     winReportParameters.Owner = this;
    ///     if (winReportParameters.ShowDialog())
    ///     {
    ///        int n = winReportParameters.LocalReport.Parameters.Count();
    ///     }
    /// </code>
    /// </example>
    public partial class LocalReportParametersWindow : Window
    {

        #region CONSTRUCTORS

        /// <summary>
        /// /// Initializes a new ReportParametersWindow.
        /// </summary>
        /// <param name="fileName">File to read.</param>
        public LocalReportParametersWindow(string fileName)
            : this(fileName, string.Empty, string.Empty)
        {

        }

        /// <summary>
        /// /// Initializes a new ReportParametersWindow.
        /// </summary>
        /// <param name="fileName">File to read.</param>
        /// <param name="userName">SQL Server Username to execute the Report query</param>
        /// <param name="password">SQL Server Password to execute the Report query</param>
        public LocalReportParametersWindow(string fileName, string userName, string password)
        {
            InitializeComponent();

            this.LocalReport = new LocalReportInformation(fileName, userName, password);
            lstReportParameters.ItemsSource = LocalReport.Parameters;
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets all datasources and parameters contained in the report.
        /// </summary>
        public LocalReportInformation LocalReport { get; private set; }

        #endregion

        #region PRIVATES

        /// <summary>
        /// On button click, verifies the given parameter values by the user.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            string values = String.Empty;

            // Browse the reportparameters
            foreach (LocalReportParameter item in lstReportParameters.Items)
            {
                switch (item.DataType)
                {
                    case LocalReportParameter.ParameterTypes.String:
                        values = item.Values as string;
                        if (values != null)
                        {
                            if (!item.IsEmptyString && string.IsNullOrEmpty(values)) errors.AppendLine(string.Format("Parameter [{0}] is empty.", item.Prompt));
                        }
                        else if (!item.IsNullValue && string.IsNullOrEmpty(values))
                        {
                            errors.AppendLine(string.Format("Parameter [{0}] is required.", item.Prompt));
                        }
                        break;

                    case LocalReportParameter.ParameterTypes.Integer:
                        values = item.Values as string;
                        if (values != null)
                        {
                            int result;
                            if (!int.TryParse(values, out result))
                            {
                                errors.AppendLine(string.Format("Parameter [{0}] is not a valid integer.", item.Prompt));
                            }
                        }
                        else if (!item.IsNullValue && string.IsNullOrEmpty(values))
                        {
                            errors.AppendLine(string.Format("Parameter [{0}] is required.", item.Prompt));
                        }
                        break;

                    case LocalReportParameter.ParameterTypes.Boolean:
                        if (item.Values != null)
                        {
                            string parsedBool = Convert.ToString(item.Values);
                            bool result;
                            if (!bool.TryParse(parsedBool, out result))
                            {
                                errors.AppendLine(string.Format("Parameter [{0}] is not a valid boolean.", item.Prompt));
                            }
                        }
                        else if (!item.IsNullValue && item.Values == null)
                        {
                            errors.AppendLine(string.Format("Parameter [{0}] is required.", item.Prompt));
                        }
                        break;

                    case LocalReportParameter.ParameterTypes.Float:
                        values = item.Values as string;
                        if (values != null)
                        {
                            double result;
                            if (!double.TryParse(values, out result))
                            {
                                errors.AppendLine(string.Format("Parameter [{0}] is not a valid float.", item.Prompt));
                            }
                        }
                        else if (!item.IsNullValue && string.IsNullOrEmpty(values))
                        {
                            errors.AppendLine(string.Format("Parameter [{0}] is required.", item.Prompt));
                        }
                        break;

                    case LocalReportParameter.ParameterTypes.DateTime:
                        DateTime? dateTimeValue = item.Values as DateTime?;
                        if (!item.IsNullValue && dateTimeValue == null)
                        {
                            errors.AppendLine(string.Format("Parameter [{0}] is required.", item.Prompt));
                        }
                        break;

                    case LocalReportParameter.ParameterTypes.ComboBox:
                        if (item.Values == null)
                        {
                            errors.AppendLine(string.Format("Parameter [{0}] no value selected.", item.Prompt));
                        }
                        else
                        {
                            KeyValuePair<string, string> value = (KeyValuePair<string, string>)item.Values;
                            item.Values = value.Key;
                        }

                        break;
                }
            }

            if (errors.Length > 0)
            {
                this.DialogResult = null;
                MessageBox.Show(errors.ToString(), "Errors", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        #endregion
    }

}
