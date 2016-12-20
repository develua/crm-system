using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace CRM_Service
{
    [ServiceContract]
    interface IAnalytics
    {
        [OperationContract]
        ResultService[] Top5CustomersSummaDeal(DateTime fromDate, DateTime toDate);
        [OperationContract]
        ResultService[] Top5CustomersRelevance(DateTime fromDate, DateTime toDate);
        [OperationContract]
        ResultService[] Top5StaffSummaDeal(DateTime fromDate, DateTime toDate);
        [OperationContract]
        ResultService[] Top5StaffRelevance(DateTime fromDate, DateTime toDate);
        [OperationContract]
        ResultService[] StateTransactions(DateTime fromDate, DateTime toDate);
    }
}
