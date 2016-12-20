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
using CRM_Client.Model;
using MahApps.Metro.Controls.Dialogs;
using CRM_Client.Service;

namespace CRM_Client.View
{
    public partial class ListStaffPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();

        public ListStaffPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dgStaff.ItemsSource = await _databasenEtities.Staff.ToArrayAsync();
            prLoadData.IsActive = false;
        }
        
        // Открытие карточки сотрудника

        private void dgStaff_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgStaff.SelectedItem != null)
            {
                Staff staff = (Staff)dgStaff.SelectedItem;
                Application.Current.Properties.Add("id", staff.ID);
                NavigationService.Content = new StaffPage();
            }
        }

        // Удаление сотрудника

        private async void dgStaff_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgStaff.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        _databasenEtities.Staff.Remove(dgStaff.SelectedItem as Staff);
                        await _databasenEtities.SaveChangesAsync();
                        dgStaff.ItemsSource = _databasenEtities.Staff.ToArray();
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
