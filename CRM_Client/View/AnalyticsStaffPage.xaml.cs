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
    public partial class AnalyticsStaffPage : Page
    {
        AnalyticsClient _client = new AnalyticsClient();

        public AnalyticsStaffPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime dateFrom = DateTime.Now.AddDays(-30);
            DateTime dateTo = DateTime.Now;

            dpFromDate1.SelectedDate = dpFromDate2.SelectedDate = dateFrom;
            dpToDate1.SelectedDate = dpToDate2.SelectedDate = dateTo;
        }

        // Загрузка аналитики

        private async void LoadDateAnalytics(int numChart, DateTime? dateFrom, DateTime? dateTo)
        {
            if (dateFrom != null && dateTo != null)
            {
                if (numChart == 1 || numChart == 0)
                    csTop5StaffDeal.ItemsSource = await _client.Top5StaffSummaDealAsync((DateTime)dateFrom, (DateTime)dateTo);

                if (numChart == 2 || numChart == 0)
                    csTop5StateDeal.ItemsSource = await _client.Top5StaffRelevanceAsync((DateTime)dateFrom, (DateTime)dateTo);

                prLoadData1.Visibility = prLoadData2.Visibility = Visibility.Collapsed;
            }
        }

        // Собития изменения даты

        private void dpDate1_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDateAnalytics(1, dpFromDate1.SelectedDate, dpToDate1.SelectedDate);
        }

        private void dpDate2_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDateAnalytics(2, dpFromDate2.SelectedDate, dpToDate2.SelectedDate);
        }

    }
}
