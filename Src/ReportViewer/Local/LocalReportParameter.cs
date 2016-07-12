using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Apps72.Dev.Ssrs.ReportViewer.Local
{
    /// <summary>
    /// Class to manage a ReportParameter
    /// </summary>
    public class LocalReportParameter : INotifyPropertyChanged
    {
        #region ENUMS

        /// <summary>
        /// List of parameter types.
        /// </summary>
        public enum ParameterTypes
        {
            /// <summary>
            /// String
            /// </summary>
            String, 
            /// <summary>
            /// Boolean
            /// </summary>
            Boolean, 
            /// <summary>
            /// DateTime
            /// </summary>
            DateTime, 
            /// <summary>
            /// Integer
            /// </summary>
            Integer, 
            /// <summary>
            /// Float
            /// </summary>
            Float, 
            /// <summary>
            /// Combobox
            /// </summary>
            ComboBox
        }

        #endregion

        #region EVENTS

        /// <summary>
        /// Event handler for the PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region DECLARATIONS

        private object _values = null;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of LocalReportParameter
        /// </summary>
        internal LocalReportParameter() { }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets the identifier of the parameter.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the Prompt showed to the user.
        /// </summary>
        public string Prompt { get; internal set; }

        /// <summary>
        /// Gets the Data-Type of parameter.
        /// </summary>
        public ParameterTypes DataType { get; internal set; }

        /// <summary>
        /// Gets True if Empty string is allowed.
        /// </summary>
        public bool IsEmptyString { get; internal set; }

        /// <summary>
        /// Gets True if Null value is allowed.
        /// </summary>
        public bool IsNullValue { get; internal set; }

        /// <summary>
        /// Gets True if Multiple values are selectable.
        /// </summary>
        public bool IsMultipleValues { get; internal set; }

        /// <summary>
        /// Gets all available values for this parameter, only a value from this list can be used.
        /// </summary>
        public Dictionary<string, string> AvailableValues { get; internal set; }

        /// <summary>
        /// Gets the default set of values for this parameter, multiple if multi enabled parameter.
        /// </summary>
        public string[] DefaultValues { get; internal set; }

        /// <summary>
        /// Gets or sets the values selected for this property.
        /// </summary>
        public object Values
        {
            get
            {
                return _values;
            }
            set
            {
                _values = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Values"));
            }
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Handler method for the PropertyChanged.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        #endregion

    }
}
