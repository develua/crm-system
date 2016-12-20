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
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using CRM_Client.Model;
using System.Data.Entity;
using CRM_Client.Service;

namespace CRM_Client.View
{
    public partial class GoodsPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();
        Goods _newGoods = new Goods { Price = 0.00m, CountUnits = 0 };
        int _ID_Row = 0;

        public GoodsPage()
        {
            InitializeComponent();

            if (Application.Current.Properties.Contains("id"))
                _ID_Row = (int)Application.Current.Properties["id"];
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (_ID_Row == 0)
                DataContext = _newGoods;
            else
                DataContext = await _databasenEtities.Goods.SingleAsync((a) => a.ID == _ID_Row);

            cbTypeGoods.ItemsSource = await _databasenEtities.TypeGoods.ToArrayAsync();

            mainGrid.IsEnabled = true;
        }

        // Добавление/Изменение товара/услуги

        private async void btnSeveGoods_Click(object sender, RoutedEventArgs e)
        {
            if (_ID_Row == 0)
                _databasenEtities.Goods.Add(_newGoods);

            int resultDB = await _databasenEtities.SaveChangesAsync();
            MessageService.MetroMessageDialogResult(resultDB);

            if (_ID_Row == 0)
            {
                _newGoods = new Goods { Price = 0.00m, CountUnits = 0 };
                DataContext = _newGoods;
            }
        }

    }
}
