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
    public partial class ListReminderPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();

        public ListReminderPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dgReminder.ItemsSource = await _databasenEtities.Reminder.ToArrayAsync();
            prLoadData.IsActive = false;
        }

        // Открытие карточки напоминания

        private void dgReminder_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgReminder.SelectedItem != null)
            {
                Reminder reminder = (Reminder)dgReminder.SelectedItem;
                NavigationService.Content = new ReminderPage(reminder.ID);
            }
        }

        // Удаление напоминания

        private async void dgReminder_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgReminder.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        _databasenEtities.Reminder.Remove(dgReminder.SelectedItem as Reminder);
                        await _databasenEtities.SaveChangesAsync();
                        dgReminder.ItemsSource = _databasenEtities.Reminder.ToArray();
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
