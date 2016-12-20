using CRM_Client.AnalyticsReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CRM_Client.View
{
    public partial class AnalyticsDealPage : Page
    {
        AnalyticsClient _client = new AnalyticsClient();

        public AnalyticsDealPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime dateFrom = DateTime.Now.AddDays(-30);
            DateTime dateTo = DateTime.Now;

            dpFromDate.SelectedDate = dateFrom;
            dpToDate.SelectedDate = dateTo;
        }

        // Загрузка аналитики

        private async void LoadDateAnalytics(DateTime? dateFrom, DateTime? dateTo)
        {
            if (dateFrom != null && dateTo != null)
            {
                ResultService[] resultService = await _client.StateTransactionsAsync((DateTime)dateFrom, (DateTime)dateTo);
                int summa = resultService.Sum(a => a.Value);

                if (summa != 0)
                    for (int i = 0; i < resultService.Length; i++)
                        resultService[i].Value = resultService[i].Value * 100 / summa;

                cbStateDeal.ItemsSource = resultService;
                prLoadData.Visibility = Visibility.Collapsed;
            }
        }

        // Собития изменения даты

        private void dpDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDateAnalytics(dpFromDate.SelectedDate, dpToDate.SelectedDate);
        }


    }
}
