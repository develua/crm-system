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
    public partial class ListTaskPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();

        public ListTaskPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dgTask.ItemsSource = await _databasenEtities.TaskBD.ToArrayAsync();
            prLoadData.IsActive = false;
        }

        // Открытие карточки задачи

        private void dgTask_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgTask.SelectedItem != null)
            {
                TaskBD taskBD = (TaskBD)dgTask.SelectedItem;
                NavigationService.Content = new TaskPage(taskBD.ID);
            }
        }

        // Удаление задачи

        private async void dgTask_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgTask.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        _databasenEtities.TaskBD.Remove(dgTask.SelectedItem as TaskBD);
                        await _databasenEtities.SaveChangesAsync();
                        dgTask.ItemsSource = _databasenEtities.TaskBD.ToArray();
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
