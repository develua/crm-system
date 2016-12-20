using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Linq;
using CRM_Client.Model;
using System.Threading.Tasks;
using CRM_Client.Service;

namespace CRM_Client.View
{
    public partial class DesktopPage : Page
    {
        List<Flyout> _listFlyout;

        public DesktopPage()
        {
            InitializeComponent();

            _listFlyout = new List<Flyout> { flyoutTask, flyoutClient, flyoutStaff, flyoutAnalytics, flyoutDeal, flyoutSettings, flyoutGoods, flyoutReminder };
        }

        // События клика по Tile

        private void Tile_Task_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenFlyout(flyoutTask.IsOpen);
            flyoutTask.IsOpen = !flyoutTask.IsOpen;
        }

        private void Tile_Client_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenFlyout(flyoutClient.IsOpen);
            flyoutClient.IsOpen = !flyoutClient.IsOpen;  
        }

        private void Tile_Staff_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenFlyout(flyoutStaff.IsOpen);
            flyoutStaff.IsOpen = !flyoutStaff.IsOpen;  
        }

        private void Tile_Analytics_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenFlyout(flyoutAnalytics.IsOpen);
            flyoutAnalytics.IsOpen = !flyoutAnalytics.IsOpen;
        }

        private void Tile_Deal_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenFlyout(flyoutDeal.IsOpen);
            flyoutDeal.IsOpen = !flyoutDeal.IsOpen;  
        }

        private void Tile_Setting_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenFlyout(flyoutSettings.IsOpen);
            flyoutSettings.IsOpen = !flyoutSettings.IsOpen;
        }

        private void Tile_Goods_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenFlyout(flyoutGoods.IsOpen);
            flyoutGoods.IsOpen = !flyoutGoods.IsOpen;
        }

        private void Tile_Reminder_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenFlyout(flyoutReminder.IsOpen);
            flyoutReminder.IsOpen = !flyoutReminder.IsOpen;
        }

        // Событие потери курсора с Flyout и закрытие его

        private void Flyout_Close_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.OriginalSource is Flyout)
                (sender as Flyout).IsOpen = false;
        }

        // Событие выбора пункта меню Flyout

        private void Flyout_ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!(e.OriginalSource is ListBox))
                    return;

                ListBox listBox = e.OriginalSource as ListBox;
                ListBoxItem item = listBox.SelectedItem as ListBoxItem;

                // Создание объекта с именем в ListBoxItem.Tag

                if (item != null && item.Tag != null && item.Tag.ToString() != "" && item.Tag is string)
                {
                    object objectPage = null;

                    // Определение справочника и создание объекта страницы
                    
                    switch (item.Tag.ToString())
                    {
                        case "ReceiveTaskPage": Application.Current.Properties["ReceiveTask"] = true;  objectPage = new TaskPage();
                            break;
                        case "Directory_StatucTask": objectPage = new DirectoryPage("Список состояний задачи", typeof(ConditionTask));
                            break;
                        case "Directory_PostPeople": objectPage = new DirectoryPage("Список должностей", typeof(PostPeople));
                            break;
                        case "Directory_StatusDeal": objectPage = new DirectoryPage("Список состояний сделки", typeof(ConditionDeal));
                            break;
                        case "Directory_TypeGoods": objectPage = new DirectoryPage("Список видов товара/услуг", typeof(TypeGoods));
                            break;
                        case "Directory_TypePhone": objectPage = new DirectoryPage("Список видов телефонов", typeof(TypePhone));
                            break;
                        case "Directory_TypeEmail": objectPage = new DirectoryPage("Список видов email", typeof(TypeEmail));
                            break;
                    }

                    // Определение и создание обьекта

                    if (objectPage == null)
                    {
                        Assembly assembly = Assembly.GetExecutingAssembly();
                        Type type = assembly.GetTypes().Single(a => a.Name == (string)item.Tag);
                        objectPage = Assembly.GetExecutingAssembly().CreateInstance(type.FullName);
                    }

                    NavigationService.Content = objectPage;
                }

                listBox.UnselectAll();
            }
            catch (Exception ex)
            {
                MessageService.MetroMessageDialogError(ex.Message);
            }
        }

        // Закрытие открытих Flyout

        private void CloseOpenFlyout(bool isRun)
        {
            if (!isRun)
                for (int i = 0; i < _listFlyout.Count; i++)
                    _listFlyout[i].IsOpen = false;
        }

    }
}
