using CRM_Client.Model;
using CRM_Client.Service;
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
    public partial class ReminderPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();
        Reminder _newReminder = new Reminder();
        bool _isNewRecord;
        int _ID_Row = 0;
        public int ID_User { get; set; }

        public ReminderPage()
        {
            InitializeComponent();
            _isNewRecord = true;
            ID_User = (int)Application.Current.Properties["ID_User"];
        }

        public ReminderPage(int id)
        {
            InitializeComponent();
            _ID_Row = id;
            ID_User = (int)Application.Current.Properties["ID_User"];
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    Reminder reminder = _newReminder;
                    List<CardClient> companyList = new List<CardClient> { new CardClient() };
                    List<People> peopleList = new List<People> { new People() };
                    List<Goods> goodsList = new List<Goods> { new Goods() };

                    if (!_isNewRecord)
                    {
                        reminder = _databasenEtities.Reminder.Single(a => a.ID == _ID_Row);
                        ID_User = (int)reminder.ID_StaffSender;
                    }

                    PostPeople[] postPeopleArr = _databasenEtities.PostPeople.OrderBy(a => a.Name).ToArray();
                    Staff[] staffArr = _databasenEtities.Staff.ToArray();
                    companyList.AddRange(_databasenEtities.CardClient.Where(a => !String.IsNullOrEmpty(a.NameCompany)).ToArray());
                    peopleList.AddRange(_databasenEtities.People.ToArray());
                    goodsList.AddRange(_databasenEtities.Goods.ToArray());

                    Dispatcher.Invoke(() =>
                    {
                        DataContext = reminder;
                        cbPost.ItemsSource = postPeopleArr;
                        cbStaffSender.ItemsSource = staffArr;
                        cbStaffSender.DataContext = this;
                        cbPeople.ItemsSource = peopleList;
                        cbCompany.ItemsSource = companyList;
                        cbGoods.ItemsSource = goodsList;

                        if (_isNewRecord)
                        {
                            cbStaffSender.IsEnabled = false;
                            dpDataEvent.SelectedDate = DateTime.Now;
                        }

                        if (reminder.ID_Client == null)
                            tbCompanyID.Text = "0";

                        if (reminder.ID_People == null)
                            tbPeopleID.Text = "0";

                        if (reminder.ID_Deal == null)
                            tbDealID.Text = "0";

                        if (reminder.ID_Goods == null)
                            tbGoodsID.Text = "0";

                        gMain.IsEnabled = true;
                    });

                }
                catch (Exception ex)
                {
                    MessageService.MetroMessageDialogError(ex.Message);
                }
            });
        }

        // Событьие изменения должности

        private void cbPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbPost.SelectedItem != null)
            {
                PostPeople postPeople = cbPost.SelectedItem as PostPeople;
                List<Staff> staffList = new List<Staff> { new Staff() };

                staffList.AddRange(_databasenEtities.Staff.Where(a => a.ID_Post == postPeople.ID).ToArray());
                cbStaff.ItemsSource = staffList;
            }
        }

        // Событие изменения компании

        private void cbCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbCompany.SelectedItem != null)
            {
                CardClient cardClient = cbCompany.SelectedItem as CardClient;

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
                    List<People> peopleList = new List<People> { new People() };
                    peopleList.AddRange(_databasenEtities.People.ToArray());
                    cbPeople.ItemsSource = peopleList;
                    tbPostPeople.Visibility = cbPostPeople.Visibility = Visibility.Collapsed;
                }
            }
            else
                tbPostPeople.Visibility = cbPostPeople.Visibility = Visibility.Collapsed;
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
                    string info = String.Format("Название компании: {0}\nСтана: {1}\nОбласть: {2}\nГород: {3}\nКарточа создана: {4}", cardClient.NameCompany, cardClient.City.Area.Country.Name, cardClient.City.Area.Name, cardClient.City.Name, cardClient.CreateCard);
                    MessageService.MetroMessageDialog("Информация о компании", info);
                }

                NavigationService.StopLoading();
                return;
            }

            int id = Convert.ToInt32(hyperlink.Tag);

            if (id > 0)
                Application.Current.Properties.Add("id", id);
            else
                NavigationService.StopLoading();
        }

        // Сохранение напоминания

        private async void btnSaveReminder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Добавление нового напоминания

                if (_isNewRecord)
                {
                    _newReminder.ID_StaffSender = ID_User;
                    _databasenEtities.Reminder.Add(_newReminder);
                }

                int resultDB = await _databasenEtities.SaveChangesAsync();
                MessageService.MetroMessageDialogResult(resultDB);

                if (_isNewRecord && resultDB > -1)
                {
                    DataContext = _newReminder = new Reminder();
                    tbCompanyID.Text = tbPeopleID.Text = "0";
                    tbDealID.Text = tbGoodsID.Text = "0";
                    dpDataEvent.SelectedDate = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageService.MetroMessageDialogError(ex.Message);
            }
        }

    }
}
