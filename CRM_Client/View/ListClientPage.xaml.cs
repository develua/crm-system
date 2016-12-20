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
    public partial class ListClientPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();

        public ListClientPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dgClient.ItemsSource = await _databasenEtities.People.ToArrayAsync();
            prLoadData.IsActive = false;
        }

        // Открытие карточки клиента

        private void dgClient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgClient.SelectedItem != null)
            {
                People people = (People)dgClient.SelectedItem;
                Application.Current.Properties.Add("id", people.ID);
                NavigationService.Content = new ClientPage();
            }
        }

        // Удаление клиента

        private async void dgClient_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgClient.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        People people = dgClient.SelectedItem as People;
                        CardClient cardClient = people.CardClient;

                        _databasenEtities.People.Remove(people);

                        if (cardClient.People.Count == 0)
                            _databasenEtities.CardClient.Remove(cardClient);

                        await _databasenEtities.SaveChangesAsync();
                        dgClient.ItemsSource = _databasenEtities.People.ToArray();
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
