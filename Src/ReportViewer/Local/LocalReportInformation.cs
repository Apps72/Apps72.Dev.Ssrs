using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Xml;

namespace Apps72.Dev.Ssrs.ReportViewer.Local
{
    /// <summary>
    /// Class to manage a list of Local Report Parameters and SQL Data Sources (included in RDLC).
    /// </summary>
    public class LocalReportInformation
    {
        #region DECLARATIONS

        private ObservableCollection<LocalReportParameter> _items = new ObservableCollection<LocalReportParameter>();
        private List<LocalReportDataSource> _sqlDataSources = new List<LocalReportDataSource>();
        private string _userName = string.Empty;
        private string _password = string.Empty;

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a new instance of LocalReportInformation.
        /// </summary>
        /// <param name="fileName">RDLC filename to manage.</param>
        /// <param name="userName">SQL Server UserName to execute the DataSouce CommandText</param>
        /// <param name="password">SQL Server Password to execute the DataSouce CommandText</param>
        public LocalReportInformation(string fileName, string userName, string password)
        {
            _userName = userName;
            _password = password;

            XmlDocument document = new XmlDocument();
            document.Load(fileName);
            ParseXmlDocument(document);
        }

        /// <summary>
        /// Initializes a new instance of LocalReportInformation.
        /// </summary>
        /// <param name="fileName">RDLC filename to manage.</param>
        public LocalReportInformation(string fileName)
            : this(fileName, string.Empty, string.Empty)
        {
        }

        #endregion

        #region PROPERTIES

        /// <summary>
        /// Gets all Parameters included in the Report (RDLC).
        /// </summary>
        public LocalReportParameter[] Parameters
        {
            get
            {
                return _items.ToArray();
            }
        }

        /// <summary>
        /// Gets all datasources included in the Report (RDLC).
        /// </summary>
        public LocalReportDataSource[] DataSources
        {
            get
            {
                return _sqlDataSources.ToArray();
            }
        }

        #endregion

        #region PRIVATES

        /// <summary>
        /// Parses the RDLC Document provided and extracts the Parameters and DataSources.
        /// </summary>
        /// <param name="document"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void ParseXmlDocument(XmlDocument document)
        {
            XmlElement root = document.DocumentElement;
            XmlNode nodelistDataSource = root["DataSources"];
            XmlNode nodelistDataSets = root["DataSets"];
            XmlNode nodelistReportParameters = root["ReportParameters"];
            Dictionary<string, string> dataSources = new Dictionary<string, string>();

            #region DataSource

            XmlNodeList childListDataSource = nodelistDataSource.ChildNodes;
            foreach (XmlNode childDataSourceItem in childListDataSource)
            {
                foreach (XmlNode childnode in childDataSourceItem)
                {
                    if (childnode.Name == "ConnectionProperties")
                    {
                        XmlNodeList properties = childnode.ChildNodes;
                        string name = string.Empty;
                        string connectionString = string.Empty;

                        foreach (XmlNode property in properties)
                        {
                            switch (property.Name)
                            {
                                case "ConnectString":
                                    name = childDataSourceItem.Attributes["Name"].Value;
                                    connectionString = property.InnerText;
                                    break;

                                case "IntegratedSecurity":
                                    if (!LocalReportDataSource.IncludeIntegratedSecurity(connectionString))
                                    {
                                        if (!connectionString.EndsWith(";"))
                                            connectionString += ";";

                                        connectionString += "Integrated Security=SSPI;";
                                    }
                                    break;
                            }
                        }

                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(connectionString))
                        {
                            dataSources.Add(name, connectionString);
                        }
                    }
                }
            }

            #endregion

            #region Datasets

            XmlNodeList childListDataSets = nodelistDataSets.ChildNodes;

            foreach (XmlNode childnode in childListDataSets)
            {
                // Check if DataSet element exists.
                if (childnode.Name == "DataSet")
                {
                    LocalReportDataSource dataSource = new LocalReportDataSource(_userName, _password);

                    XmlNodeList fldsList = childnode.ChildNodes;

                    foreach (XmlNode fldsNode in fldsList)
                    {
                        // Check if Fields
                        if (fldsNode.Name == "Query")
                        {
                            XmlNodeList qryList = fldsNode.ChildNodes;

                            foreach (XmlNode qryNode in qryList)
                            {
                                // Check for CommandText
                                if (qryNode.Name == "CommandText")
                                {
                                    dataSource.CommandText = qryNode.InnerText;
                                }
                                else if (qryNode.Name == "DataSourceName")
                                {
                                    dataSource.DataSourceName = qryNode.InnerText;
                                }

                            }

                        }
                    }

                    dataSource.DataSetName = childnode.Attributes["Name"].Value;

                    _sqlDataSources.Add(dataSource);
                }
            }
            #endregion

            SetDataSources(_sqlDataSources, dataSources);

            #region ReportParameters

            this.SetReportParameters(nodelistReportParameters);

            #endregion


        }

        /// <summary>
        /// Set the report parameters.
        /// </summary>
        /// <param name="node"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        private void SetReportParameters(XmlNode node)
        {
            XmlNodeList rParams = node.ChildNodes;

            foreach (XmlNode rParam in rParams)
            {
                string name = string.Empty;
                string prompt = string.Empty;
                LocalReportParameter.ParameterTypes dataType;
                bool nullable = false;
                bool allowBlank = false;

                name = rParam.Attributes["Name"].InnerText;
                if (rParam["Prompt"].InnerText != null) prompt = rParam["Prompt"].InnerText;

                if (rParam["Nullable"] != null) nullable = Convert.ToBoolean(rParam["Nullable"].InnerText);
                if (rParam["AllowBlank"] != null) allowBlank = Convert.ToBoolean(rParam["AllowBlank"] != null);

                // Get the available values
                Dictionary<string, string> parameterValues = new Dictionary<string, string>();
                bool hasParameters = false;

                XmlNodeList rItems = rParam.ChildNodes;
                foreach (XmlNode rItem in rItems)
                {
                    if (rItem.Name == "ValidValues")
                    {
                        if (rItem["ParameterValues"] != null)
                        {
                            foreach (XmlNode pValue in rItem["ParameterValues"])
                            {
                                hasParameters = true;
                                if (pValue["Value"] != null)
                                {
                                    parameterValues.Add(pValue["Value"].InnerText, pValue["Value"].InnerText);
                                }
                            }
                        }
                        if (rItem["DataSetReference"] != null)
                        {
                            XmlNodeList dsRefItems = rItem["DataSetReference"].ChildNodes;
                            LocalReportDataSource requestDataSource = null;
                            string valueField = string.Empty;
                            string labelField = string.Empty;
                            // Looking for filling value
                            #region Looking
                            foreach (XmlNode pNode in dsRefItems)
                            {
                                hasParameters = true;
                                switch (pNode.Name)
                                {
                                    case "DataSetName":
                                        var dataSource = _sqlDataSources.FirstOrDefault(ds => ds.DataSetName == pNode.InnerText);
                                        if (dataSource != null)
                                        {
                                            requestDataSource = dataSource;
                                        }
                                        else
                                        {
                                            throw new EntryPointNotFoundException(string.Format("The datasources {0} was not found.", pNode.Value));
                                        }
                                        break;
                                    case "ValueField":
                                        valueField = pNode.InnerText;
                                        break;
                                    case "LabelField":
                                        labelField = pNode.InnerText;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            #endregion

                            // Execution Query to get value
                            if (!string.IsNullOrEmpty(valueField) && requestDataSource != null)
                            {
                                // If the Label isn't define use the valueField as label
                                if (string.IsNullOrEmpty(labelField))
                                {
                                    labelField = valueField;
                                }

                                // Retrieve all data from the Database
                                using (var cmd = new SqlDatabaseCommand(requestDataSource.ConnectionString))
                                {
                                    cmd.CommandText = requestDataSource.CommandText;

                                    System.Data.DataTable results = cmd.ExecuteTable();

                                    foreach (System.Data.DataRow result in results.Rows)
                                    {
                                        parameterValues.Add(Convert.ToString(result[valueField]), Convert.ToString(result[labelField]));
                                    }
                                }

                            }

                        }
                    }
                }

                dataType = GetParameterType(rParam["DataType"].InnerText, hasParameters);

                LocalReportParameter parameter = new LocalReportParameter()
                {
                    Name = name,
                    Prompt = prompt,
                    DataType = dataType,
                    IsNullValue = nullable,
                    IsEmptyString = allowBlank,
                    AvailableValues = parameterValues
                };

                // Set all boolean values to false
                if (dataType == LocalReportParameter.ParameterTypes.Boolean)
                {
                    parameter.Values = false;
                }

                // Add item to the collection
                this._items.Add(parameter);

            }
        }

        /// <summary>
        /// Link's a DataSource with it's connection string.
        /// </summary>
        /// <param name="sqlDataSources">The list of Report DataSources</param>
        /// <param name="dataSources">The dictionary of connection strings</param>
        private void SetDataSources(List<LocalReportDataSource> sqlDataSources, Dictionary<string, string> dataSources)
        {
            foreach (KeyValuePair<string, string> source in dataSources)
            {
                foreach (LocalReportDataSource dataSource in sqlDataSources.Where(x => x.DataSourceName == source.Key))
                {
                    dataSource.ConnectionString = source.Value;
                }

            }
        }

        /// <summary>
        /// Gets the report parameter enum.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="hasParameterValues"></param>
        /// <returns></returns>
        private LocalReportParameter.ParameterTypes GetParameterType(string type, bool hasParameterValues)
        {
            if (hasParameterValues)
            {
                return LocalReportParameter.ParameterTypes.ComboBox;
            }
            else
            {
                switch (type)
                {
                    case "String":
                        return LocalReportParameter.ParameterTypes.String;
                    case "Boolean":
                        return LocalReportParameter.ParameterTypes.Boolean;
                    case "DateTime":
                        return LocalReportParameter.ParameterTypes.DateTime;
                    case "Integer":
                        return LocalReportParameter.ParameterTypes.Integer;
                    case "Float":
                        return LocalReportParameter.ParameterTypes.Float;
                    default:
                        return LocalReportParameter.ParameterTypes.String;
                }
            }
        }

        #endregion
    }
}
