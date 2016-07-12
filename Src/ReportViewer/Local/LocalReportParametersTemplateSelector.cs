using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace Apps72.Dev.Ssrs.ReportViewer.Local
{
    /// <summary>
    /// Class that initializes a template for the ReportParameter.
    /// </summary>
    internal class LocalReportParametersTemplateSelector : DataTemplateSelector
    {
        #region PROPERTIES

        /// <summary>
        /// Gets or Sets the DataTemlate for String Parameter
        /// </summary>
        public DataTemplate StringDataType { get; set; }

        /// <summary>
        /// Gets or Sets the DataTemlate for Integer Parameter
        /// </summary>
        public DataTemplate IntegerDataType { get; set; }

        /// <summary>
        /// Gets or Sets the DataTemplate for Boolean Parameter
        /// </summary>
        public DataTemplate BooleanDataType { get; set; }

        /// <summary>
        /// Gets or Sets the DataTemplate for Float Parameter
        /// </summary>
        public DataTemplate FloatDataType { get; set; }

        /// <summary>
        /// Gets or Sets the DataTemplate for DateTime Parameter
        /// </summary>
        public DataTemplate DateTimeDataType { get; set; }

        /// <summary>
        /// Gets or Sets the DataTemplate for Available Values Parameter (combobox)
        /// </summary>
        public DataTemplate ComboBoxDataType { get; set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Returns the appropriate DataTemplate based on the report parameter data type.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            LocalReportParameter reportParameter = item as LocalReportParameter;

            if (reportParameter != null)
            {
                switch (reportParameter.DataType)
                {
                    case LocalReportParameter.ParameterTypes.String:
                        return StringDataType;
                    case LocalReportParameter.ParameterTypes.Integer:
                        return IntegerDataType;
                    case LocalReportParameter.ParameterTypes.Boolean:
                        return BooleanDataType;
                    case LocalReportParameter.ParameterTypes.Float:
                        return FloatDataType;
                    case LocalReportParameter.ParameterTypes.DateTime:
                        return DateTimeDataType;
                    case LocalReportParameter.ParameterTypes.ComboBox:
                        return ComboBoxDataType;
                    default:
                        return null;
                }
            }

            throw new ArgumentException("Item is not a ReportParameter type.");
        }

        #endregion

    }
}
