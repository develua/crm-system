using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRM_Service
{
    class Analytics : IAnalytics
    {
        // топ 5 клиентов по сумме сделок

        public ResultService[] Top5CustomersSummaDeal(DateTime fromDate, DateTime toDate)
        {
            return ExecuteProcedure("Top5CustomersSummaDeal", fromDate, toDate);
        }

        // топ 5 клиентов по релевантности сделок

        public ResultService[] Top5CustomersRelevance(DateTime fromDate, DateTime toDate)
        {
            return ExecuteProcedure("Top5CustomersRelevance", fromDate, toDate);
        }

        // топ 5 сотрудников по сумме сделок

        public ResultService[] Top5StaffSummaDeal(DateTime fromDate, DateTime toDate)
        {
            return ExecuteProcedure("Top5StaffSummaDeal", fromDate, toDate);
        }

        // топ 5 сотрудников по релевантности сделок

        public ResultService[] Top5StaffRelevance(DateTime fromDate, DateTime toDate)
        {
            return ExecuteProcedure("Top5StaffRelevance", fromDate, toDate);
        }

        // состояние сделок

        public ResultService[] StateTransactions(DateTime fromDate, DateTime toDate)
        {
            return ExecuteProcedure("StateTransactions", fromDate, toDate);
        }

        // вызов процедуры

        private ResultService[] ExecuteProcedure(string nameProc, DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<ResultService> _collectionData = new List<ResultService>();
                string _conectionString = ConfigurationManager.ConnectionStrings["localdb"].ConnectionString;

                using (SqlConnection connection = new SqlConnection(_conectionString))
                {
                    SqlCommand command = new SqlCommand();
                    connection.Open();

                    command.Connection = connection;
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = nameProc;

                    command.Parameters.Add(new SqlParameter("DateFrom", fromDate));
                    command.Parameters.Add(new SqlParameter("DateTo", toDate));

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                        _collectionData.Add(new ResultService { ID = Convert.ToInt32(reader["ID"]), Name = reader["Name"].ToString(), Value = Convert.ToInt32(reader["Value"]) });

                    return _collectionData.ToArray();
                }
            }
            catch { }

            return null;
        }

    }
}
