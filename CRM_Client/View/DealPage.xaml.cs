using CRM_Client.Model;
using CRM_Client.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    public partial class DealPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();
        Deal _newDeal;
        GoodsInDeal _newGoodsInDeal;
        List<GoodsInDeal> _goodsInDealList;
        bool _isNewRecord;
        int _ID_Row = 0;
        public int ID_User { get; set; }

        public DealPage()
        {
            InitializeComponent();

            if (Application.Current.Properties.Contains("id"))
                _ID_Row = (int)Application.Current.Properties["id"];
            else
                _isNewRecord = true;

            ID_User = (int)Application.Current.Properties["ID_User"];
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Deal deal = _newDeal = new Deal();
                    _newGoodsInDeal = new GoodsInDeal();
                    _goodsInDealList = new List<GoodsInDeal>();
                    List<CardClient> companyList = new List<CardClient> { new CardClient() };

                    if (!_isNewRecord)
                    {
                        deal = _databasenEtities.Deal.Single(a => a.ID == _ID_Row);
                        _goodsInDealList.AddRange(_databasenEtities.GoodsInDeal.Where(a => a.Deal.ID == deal.ID));
                        ID_User = deal.ID_Staff;
                    }

                    Staff[] staffArr = _databasenEtities.Staff.ToArray();
                    companyList.AddRange(_databasenEtities.CardClient.Where(a => !String.IsNullOrEmpty(a.NameCompany)).ToArray());
                    ConditionDeal[] conditionDealArr = _databasenEtities.ConditionDeal.OrderBy(a => a.Name).ToArray();
                    TypeGoods[] typeGoodsArr = _databasenEtities.TypeGoods.OrderBy(a => a.Name).ToArray();
                    People[] peopleArr;

                    if (deal.People != null && deal.People.CardClient != null && deal.People.CardClient.NameCompany != "")
                        peopleArr = _databasenEtities.People.Where(a => a.ID_CardClient == deal.People.ID_CardClient).ToArray();
                    else
                        peopleArr = _databasenEtities.People.ToArray();

                    Dispatcher.Invoke(() =>
                    {
                        gbDeal.DataContext = deal;
                        cbCompany.ItemsSource = companyList;
                        cbStaff.DataContext = this;
                        cbStaff.ItemsSource = staffArr;
                        cbPeople.ItemsSource = peopleArr;
                        cbStatus.ItemsSource = conditionDealArr;
                        cbTypeGoods.ItemsSource = typeGoodsArr;
                        dgGoods.ItemsSource = _goodsInDealList;
                        gNewGoods.DataContext = _newGoodsInDeal;

                        if (_isNewRecord)
                        {
                            cbStaff.IsEnabled = false;
                            dpDateDeal.IsEnabled = false;
                            tbCompanyID.Text = "0";
                            dpDateDeal.SelectedDate = DateTime.Now;
                        }
                        else
                        {
                            cbCompany.SelectedValue = deal.People.ID_CardClient;

                            if (deal.People.CardClient.NameCompany == "")
                                tbCompanyID.Text = "0";
                        }

                        gMain.IsEnabled = true;
                    });
                }
                catch (Exception ex)
                {
                    MessageService.MetroMessageDialogError(ex.Message);
                }
            });
        }

        // Событие изменения компании

        private void cbCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCompany.SelectedItem != null)
            {
                CardClient cardClient = cbCompany.SelectedItem as CardClient;
                tbCompanyID.Text = cardClient.ID.ToString();

                if (cardClient.ID != 0)
                {
                    tbPostPeople.Visibility = cbPostPeople.Visibility = Visibility.Visible;
                    cbPostPeople.ItemsSource = cardClient.People.Select(a => a.PostPeople);
                    cbPeople.ItemsSource = _databasenEtities.People.Where(a => a.ID_CardClient == cardClient.ID).ToArray();
                    cbPostPeople.SelectedIndex = 0;
                    cbPeople.SelectedIndex = 0;
                }
                else
                {
                    cbPeople.ItemsSource = _databasenEtities.People.ToArray();
                    tbPostPeople.Visibility = cbPostPeople.Visibility = Visibility.Collapsed;
                }
            }
            else
                tbPostPeople.Visibility = cbPostPeople.Visibility = Visibility.Collapsed;
        }

        private void tbCompanyID_LostFocus(object sender, RoutedEventArgs e)
        {
            int id;

            if (int.TryParse(tbCompanyID.Text, out id))
                cbCompany.SelectedValue = id;
        }

        // Событие изменения должности человека

        private void cbPostPeople_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPostPeople.SelectedItem != null)
            {
                PostPeople postPeople = cbPostPeople.SelectedItem as PostPeople;
                cbPeople.ItemsSource = _databasenEtities.People.Where(a => a.ID_Post == postPeople.ID).ToArray();
                cbPeople.SelectedIndex = 0;
            }
        }

        // Событие нажатия на ссылку

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Hyperlink hyperlink = (e.OriginalSource as Hyperlink);

            if (hyperlink.Name == "Company")
            {
                if (cbCompany.SelectedItem != null)
                {
                    CardClient cardClient = cbCompany.SelectedItem as CardClient;

                    if (cardClient.ID != 0)
                    {
                        string info = String.Format("Название компании: {0}\nСтана: {1}\nОбласть: {2}\nГород: {3}\nКарточа создана: {4}", cardClient.NameCompany, cardClient.City.Area.Country.Name, cardClient.City.Area.Name, cardClient.City.Name, cardClient.CreateCard);
                        MessageService.MetroMessageDialog("Информация о компании", info);
                    }
                }

                return;
            }

            int id = Convert.ToInt32(hyperlink.Tag);

            if (id > 0)
                Application.Current.Properties.Add("id", id);
            else
                NavigationService.StopLoading();
        }

        // Открытие карточки товара

        private void dgGoods_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgGoods.SelectedItem != null)
            {
                GoodsInDeal goodsInDeal = (GoodsInDeal)dgGoods.SelectedItem;
                Application.Current.Properties["id"] = goodsInDeal.ID_Goods;
                NavigationService.Content = new GoodsPage();
            }
        }

        // Собитие изменнения типа товара

        private void cbTypeGoods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTypeGoods.SelectedItem != null)
            {
                TypeGoods typeGoods = cbTypeGoods.SelectedItem as TypeGoods;
                cbGoods.ItemsSource = _databasenEtities.Goods.Where(a => a.ID_TypeGoods == typeGoods.ID).OrderBy(a => a.Name).ToArray();
            }
        }

        // Событие добавления товара

        private void btnAddNewGoods_Click(object sender, RoutedEventArgs e)
        {
            _newGoodsInDeal.Goods = _databasenEtities.Goods.Single(a => a.ID == _newGoodsInDeal.ID_Goods);

            if (_newGoodsInDeal.Goods.CountUnits >= _newGoodsInDeal.CountUnits)
            {
                _goodsInDealList.Add(_newGoodsInDeal);

                // Подсчет общей суммы

                decimal price = Decimal.Parse(tbPrice.Text, CultureInfo.InvariantCulture);
                price += _newGoodsInDeal.CountUnits * _newGoodsInDeal.Goods.Price;
                tbPrice.Text = price.ToString();

                _newGoodsInDeal.Goods.CountUnits -= _newGoodsInDeal.CountUnits;

                gNewGoods.DataContext = _newGoodsInDeal = new GoodsInDeal();
                dgGoods.Items.Refresh();
            }
            else
                MessageService.MetroMessageDialog("Количесто товара не достаточно", "Вы ввели количество товара больше чем есть в наличие.");
        }

        // Сохранение сделки

        private async void btnSaveDeal_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Deal deal = gbDeal.DataContext as Deal;
                deal.ID_Staff = ID_User;
                deal.DateDeal = DateTime.Now;

                // Добавление новой сделки

                if (_isNewRecord)
                    _ID_Row = _databasenEtities.Deal.Add(_newDeal).ID;

                // Добавление в сделку

                for (int i = 0; i < _goodsInDealList.Count; i++)
                    if (_goodsInDealList[i].ID_Deal == 0)
                    {
                        _goodsInDealList[i].ID_Deal = _ID_Row;
                        _databasenEtities.GoodsInDeal.Add(_goodsInDealList[i]);
                    }

                int resultDB = await _databasenEtities.SaveChangesAsync();
                MessageService.MetroMessageDialogResult(resultDB);

                if (_isNewRecord && resultDB > -1)
                {
                    gbDeal.DataContext = _newDeal = new Deal();
                    cbCompany.SelectedIndex = 0;
                    _goodsInDealList.Clear();
                    dgGoods.Items.Refresh();
                    dpDateDeal.SelectedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageService.MetroMessageDialogError(ex.Message);
            }
        }

        
    }
}
