using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Reporting.WebForms;
using System.Data.SqlClient;
using System.Data;

namespace ReportToPdfConverter.Utility
{
    public class RenderPdfUtility
    {
        /// <summary>
        /// Load the dataset from the Stored Procedure
        /// </summary>
        /// <param name="spName">Stored Procedure Name</param>
        /// <param name="reportParameters">List of Parameters that is need to execute the Stored Procedure</param>
        /// <returns>System.Data.DataSet object</returns>
        internal static System.Data.DataSet GetDataSet(string spName, IEnumerable<ReportCriteriaValue> reportParameters)
        {
            // Get connection string from web.config
            var thisConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["MovieDBContext"].ConnectionString);

            // Create Command String
            var cmd = new SqlCommand
            {
                Connection = thisConnection,
                CommandText = spName,
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 0
            };

            // Set parameters to the command String
            foreach (var parameter in reportParameters)
            {
                cmd.Parameters.Add(new SqlParameter(parameter.ParameterName, parameter.ParameterValue));
            }

            // Execute query
            var da = new SqlDataAdapter(cmd);

            // Fill the dataset
            var thisDataSet = new System.Data.DataSet();
            da.Fill(thisDataSet);

            return thisDataSet;
        }

        /// <summary>
        /// Loads the report file
        /// </summary>
        /// <param name="dataSet">Data set</param>
        /// <param name="path">Path of the report file</param>
        /// <param name="reportCriteriaValues">List of report parameters defined in the report</param>
        /// <returns>Microsoft.Reporting.WebForms.LocalReport</returns>
        internal static LocalReport LoadReport(System.Data.DataSet dataSet, string path, IEnumerable<ReportCriteriaValue> reportCriteriaValues)
        {
            // Get the DataSource
            var rd = new ReportDataSource("ReportDS", dataSet.Tables[0]);

            // Create new LocalReport from the path given
            var lr = new LocalReport { ReportPath = path };

            // Assign the data source to the report
            lr.DataSources.Add(rd);

            // Set report parameters
            foreach (var reportCriteriaValue in reportCriteriaValues)
            {
                lr.SetParameters(new ReportParameter(reportCriteriaValue.ParameterName, reportCriteriaValue.ParameterValue));
            }

            return lr;
        }

        /// <summary>
        /// Render the report rdlc as a PDF file
        /// </summary>
        /// <param name="localReport">Local Report to render</param>
        /// <param name="renderType">Render Type</param>
        /// <returns>byte[] rendered bytes, string mimetype</returns>
        internal static Tuple<byte[], string> RenderReport(Report localReport, string renderType)
        {
            var reportType = renderType;
            string mimeType;
            string encoding;
            string fileNameExtension;

            // Customize page settings
            var deviceInfo =
            "<DeviceInfo>" +
            "  <OutputFormat>" + renderType + "</OutputFormat>" +
                //"  <PageWidth>7.5in</PageWidth>" +
                //"  <PageHeight>11in</PageHeight>" +
                //"  <MarginTop>0.5in</MarginTop>" +
                //"  <MarginLeft>1in</MarginLeft>" +
                //"  <MarginRight>1in</MarginRight>" +
                //"  <MarginBottom>0.5in</MarginBottom>" +
            "</DeviceInfo>";

            Warning[] warnings;
            string[] streams;

            // Render report
            var renderedBytes = localReport.Render(reportType, deviceInfo, out mimeType, out encoding,
                out fileNameExtension,
                out streams,
                out warnings);

            return new Tuple<byte[], string>(renderedBytes, mimeType);
        }
    }
}