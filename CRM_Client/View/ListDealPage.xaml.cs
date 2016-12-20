using CRM_Client.Model;
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
using System.Data.Entity;
using MahApps.Metro.Controls.Dialogs;
using CRM_Client.Service;

namespace CRM_Client.View
{
    public partial class ListDealPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();

        public ListDealPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dgDeal.ItemsSource = await _databasenEtities.Deal.ToArrayAsync();
            prLoadData.IsActive = false;
        }

        // Открытие карточки сделки

        private void dgDeal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgDeal.SelectedItem != null)
            {
                Deal deal = (Deal)dgDeal.SelectedItem;
                Application.Current.Properties.Add("id", deal.ID);
                NavigationService.Content = new DealPage();
            }
        }

        // Удаление сделки

        private async void dgDeal_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgDeal.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        _databasenEtities.Deal.Remove(dgDeal.SelectedItem as Deal);
                        await _databasenEtities.SaveChangesAsync();
                        dgDeal.ItemsSource = _databasenEtities.Deal.ToArray();
                    }
                    catch (Exception ex)
                    {
                        MessageService.MetroMessageDialogError(ex.Message);
                    }
                }
            }
        }

    }
}
