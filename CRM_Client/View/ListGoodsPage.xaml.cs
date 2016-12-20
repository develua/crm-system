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
using CRM_Client.Model;
using System.Data.Entity;
using MahApps.Metro.Controls.Dialogs;
using CRM_Client.Service;

namespace CRM_Client.View
{
    public partial class ListGoodsPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();

        public ListGoodsPage()
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            dgGoods.ItemsSource = await _databasenEtities.Goods.ToArrayAsync();
            prLoadData.IsActive = false;
        }

        // Открутие карточки товара

        private void dgGoods_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgGoods.SelectedItem != null)
            {
                Goods goods = (Goods)dgGoods.SelectedItem;
                Application.Current.Properties.Add("id", goods.ID);
                NavigationService.Content = new GoodsPage();
            }
        }

        // Удаление напоминания

        private async void dgGoods_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgGoods.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        _databasenEtities.Goods.Remove(dgGoods.SelectedItem as Goods);
                        await _databasenEtities.SaveChangesAsync();
                        dgGoods.ItemsSource = _databasenEtities.Goods.ToArray();
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
