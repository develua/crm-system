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
using CRM_Client.Service;
using System.Threading;
using System.Data.Entity;
using MahApps.Metro.Controls.Dialogs;
using System.Text.RegularExpressions;

namespace CRM_Client.View
{
    public partial class ClientPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();
        People _newPeople;
        CardClient _newCardClient;
        PhonePeople _newPhone;
        EmailPeople _newEmail;
        List<PhonePeople> _phoneList;
        List<EmailPeople> _emailList;
        List<PhonePeople> _delPhoneList = new List<PhonePeople>();
        List<EmailPeople> _delEmailList = new List<EmailPeople>();
        bool _isNewRecord;
        int _ID_Row = 0;

        public ClientPage()
        {
            InitializeComponent();

            if (Application.Current.Properties.Contains("id"))
            {
                _ID_Row = (int)Application.Current.Properties["id"];
                eHistoryDeal.Visibility = eHistoryTask.Visibility = Visibility.Visible;
                btnSaveData.Background = (LinearGradientBrush)FindResource("Gradient_Violet");
            }
            else
                _isNewRecord = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    People people = _newPeople = new People();
                    _newCardClient = new CardClient();
                    _newPhone = new PhonePeople();
                    _newEmail = new EmailPeople();
                    _phoneList = new List<PhonePeople>();
                    _emailList = new List<EmailPeople>();
                    CardClient cardClient = _newCardClient;
                    Deal[] dealArr = null;
                    TaskBD[] taskBDArr = null;
                    People[] peopleCompanyArr = null;

                    if (!_isNewRecord)
                    {
                        people = _databasenEtities.People.Single(a => a.ID == _ID_Row);
                        cardClient = _databasenEtities.CardClient.Single(a => a.ID == people.ID_CardClient);
                        _phoneList.AddRange(_databasenEtities.PhonePeople.Where((a) => a.ID_People == _ID_Row).ToArray());
                        _emailList.AddRange(_databasenEtities.EmailPeople.Where((a) => a.ID_People == _ID_Row).ToArray());
                        dealArr = _databasenEtities.Deal.Where(a => a.People.ID_CardClient == cardClient.ID).ToArray();
                        taskBDArr = _databasenEtities.TaskBD.Where(a => a.ID_Client == cardClient.ID || a.ID_People == _ID_Row).ToArray();
                    }

                    PostPeople[] postPeopleArr = _databasenEtities.PostPeople.ToArray();
                    CardClient[] companyArr = _databasenEtities.CardClient.Where(a => !String.IsNullOrEmpty(a.NameCompany) || a.ID == people.ID_CardClient).OrderBy(a => a.NameCompany).ToArray();
                    Country[] countryArr = _databasenEtities.Country.OrderBy(a => a.Name).ToArray();
                    TypePhone[] typePhoneArr = _databasenEtities.TypePhone.OrderBy(a => a.Name).ToArray();
                    TypeEmail[] typeEmailArr = _databasenEtities.TypeEmail.OrderBy(a => a.Name).ToArray();

                    if (!_isNewRecord && !String.IsNullOrEmpty(people.CardClient.NameCompany))
                        peopleCompanyArr = _databasenEtities.People.Where(a => a.ID_CardClient == people.ID_CardClient && a.ID != people.ID).ToArray();

                    Dispatcher.Invoke(() =>
                    {
                        gGeneralInfo.DataContext = people;
                        gContactInfo.DataContext = dpCreateCard.DataContext = cardClient;
                        cbPost.ItemsSource = postPeopleArr;

                        List<CardClient> tempCardClient = new List<CardClient>();

                        if (_isNewRecord || !String.IsNullOrEmpty(cardClient.NameCompany))
                            tempCardClient.Add(_newCardClient);

                        tempCardClient.AddRange(companyArr);

                        cbCompany.ItemsSource = tempCardClient;
                        cbCountry.ItemsSource = countryArr;

                        dgcbTypePhone.ItemsSource = cbTypePhone.ItemsSource = typePhoneArr;
                        dgcbTypeEmail.ItemsSource = cbTypeEmail.ItemsSource = typeEmailArr;
                        gNewPhone.DataContext = _newPhone;
                        gNewEmail.DataContext = _newEmail;
                        dgPhone.ItemsSource = _phoneList.ToArray();
                        dgEmail.ItemsSource = _emailList.ToArray();
                        dgPeople.ItemsSource = peopleCompanyArr;

                        cbEntity.IsChecked = String.IsNullOrEmpty(cardClient.NameCompany) ? false : true;

                        if (!_isNewRecord)
                        {
                            dgDeal.ItemsSource = dealArr;
                            dgTask.ItemsSource = taskBDArr;

                            if (!String.IsNullOrEmpty(people.CardClient.NameCompany))
                                ePeopleCompany.Visibility = Visibility.Visible;
                        }

                        gMain.IsEnabled = true;

                        if (_isNewRecord)
                        {
                            dpCreateCard.SelectedDate = DateTime.Now;
                            dpCreateCard.IsEnabled = false;
                        }
                    });
                }
                catch (Exception ex)
                {
                    MessageService.MetroMessageDialogError(ex.Message);
                }
            });
        }

        // Собитие выбора опции юр. лицо

        private void cbEntity_Checked(object sender, RoutedEventArgs e)
        {
            tbPost.Visibility = cbPost.Visibility = tbNewNameCompany.Visibility = tbNewCompany.Visibility = tbNameCompany.Visibility = cbCompany.Visibility = Visibility.Visible;
        }

        // Собитие отмены опции юр. лицо

        private void cbEntity_Unchecked(object sender, RoutedEventArgs e)
        {
            tbPost.Visibility = cbPost.Visibility = tbNewNameCompany.Visibility = tbNewCompany.Visibility = tbNameCompany.Visibility = cbCompany.Visibility = Visibility.Collapsed;
        }

        // Собитие изменение компании

        private void cbCompany_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gContactInfo.DataContext = dpCreateCard.DataContext = cbCompany.SelectedItem;
        }

        // Событие изменение страны

        private void cbCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbArea.ItemsSource = cbCity.ItemsSource = null;

            if (cbCountry.SelectedItem != null)
            {
                int id = (cbCountry.SelectedItem as Country).ID;
                cbArea.ItemsSource = _databasenEtities.Area.Where(a => a.ID_Country == id).OrderBy(a => a.Name).ToArray();
            }
        }

        // Событие изменение области

        private void cbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbArea.SelectedItem != null)
            {
                int id = (cbArea.SelectedItem as Area).ID;
                cbCity.ItemsSource = _databasenEtities.City.Where(a => a.ID_Area == id).OrderBy(a => a.Name).ToArray();
            }
        }

        // Добавление нового телефона

        private void btnAddNewPhone_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(_newPhone.Phone, @"^\+[0-9]{1,3}-[0-9]{2}-[0-9]{2}-[0-9]{2}-[0-9]{3}$", RegexOptions.IgnoreCase))
            {
                _phoneList.Add(_newPhone);
                gNewPhone.DataContext = _newPhone = new PhonePeople();
                dgPhone.ItemsSource = _phoneList.ToArray();
            }
            else
                MessageService.MetroMessageDialogError("Вы ввели неверный номер телефона!\nРекомендуемый формат: +123-45-67-89-011");
        }

        // Добавление нового Email

        private void btnAddNewEmail_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(_newEmail.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
            {
                _emailList.Add(_newEmail);
                gNewEmail.DataContext = _newEmail = new EmailPeople();
                dgEmail.ItemsSource = _emailList.ToArray();
            }
            else
                MessageService.MetroMessageDialogError("Вы ввели неверный Email адрес!");
        }

        // Открытие карточки сделки

        private void dgDeal_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgDeal.SelectedItem != null)
            {
                Deal deal = (Deal)dgDeal.SelectedItem;
                Application.Current.Properties["id"] = deal.ID;
                NavigationService.Content = new DealPage();
            }
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

        // Открытие карточки клиента

        private void dgClient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgPeople.SelectedItem != null)
            {
                People people = (People)dgPeople.SelectedItem;
                Application.Current.Properties["id"] = people.ID;
                NavigationService.Content = new ClientPage();
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

        // Удаление телефона

        private async void dgPhone_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgPhone.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    PhonePeople phonePeople = dgPhone.SelectedItem as PhonePeople;

                    if (phonePeople.ID != 0)
                        _delPhoneList.Add(phonePeople);

                    _phoneList.Remove(phonePeople);
                    dgPhone.ItemsSource = _phoneList.ToArray();
                }
            }
        }

        // Удаление email

        private async void dgEmail_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgEmail.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    EmailPeople emailPeople = dgEmail.SelectedItem as EmailPeople;

                    if (emailPeople.ID != 0)
                        _delEmailList.Add(emailPeople);

                    _emailList.Remove(emailPeople);
                    dgEmail.ItemsSource = _emailList.ToArray();
                }
            }
        }

        // Удаление клиента

        private async void dgClient_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgPeople.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");

                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        People people = dgPeople.SelectedItem as People;
                        CardClient cardClient = people.CardClient;

                        _databasenEtities.People.Remove(people);

                        if (cardClient.People.Count == 0)
                            _databasenEtities.CardClient.Remove(cardClient);

                        await _databasenEtities.SaveChangesAsync();
                        People thisPeople = gGeneralInfo.DataContext as People;
                        dgPeople.ItemsSource = _databasenEtities.People.Where(a => a.ID_CardClient == thisPeople.ID_CardClient && a.ID != thisPeople.ID).ToArray();
                    }
                    catch (Exception ex)
                    {
                        MessageService.MetroMessageDialogError(ex.Message);
                    }
                }
            }
        }

        // Сохранение карточки

        private async void bntSaveClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Если нужно то, оставляем информацию только ЧС

                if(cbEntity.IsChecked == false)
                {
                    (cbCompany.SelectedItem as CardClient).NameCompany = null;
                    (gGeneralInfo.DataContext as People).ID_Post = null;
                }

                // Добавление города в карточку

                if (cbCity.SelectedItem != null)
                    (cbCompany.SelectedItem as CardClient).ID_City = (cbCity.SelectedItem as City).ID;

                // Добавление или закрепление человека за карточкой клиента

                if (cbCompany.SelectedIndex < 1)
                    (gGeneralInfo.DataContext as People).ID_CardClient = _databasenEtities.CardClient.Add(_newCardClient).ID;
                else if (cbCompany.SelectedItem != null)
                    (gGeneralInfo.DataContext as People).ID_CardClient = (cbCompany.SelectedItem as CardClient).ID;

                // Добавление новой карточки человека

                if (_isNewRecord)
                    _ID_Row = _databasenEtities.People.Add(_newPeople).ID;

                // Добавление нового телефона

                for (int i = 0; i < _phoneList.Count; i++)
                    if (_phoneList[i].ID_People == 0)
                    {
                        _phoneList[i].ID_People = _ID_Row;
                        _databasenEtities.PhonePeople.Add(_phoneList[i]);
                    }

                // Добавление нового Email

                for (int i = 0; i < _emailList.Count; i++)
                    if (_emailList[i].ID_People == 0)
                    {
                        _emailList[i].ID_People = _ID_Row;
                        _databasenEtities.EmailPeople.Add(_emailList[i]);
                    }

                // Удаление телефонов и email

                _databasenEtities.PhonePeople.RemoveRange(_delPhoneList);
                _databasenEtities.EmailPeople.RemoveRange(_delEmailList);

                int resultDB = await _databasenEtities.SaveChangesAsync();
                MessageService.MetroMessageDialogResult(resultDB);

                if (_isNewRecord && resultDB > -1)
                {
                    cbCountry.SelectedIndex = -1;

                    gContactInfo.DataContext = _newCardClient = new CardClient();
                    gGeneralInfo.DataContext = _newPeople = new People();

                    dpCreateCard.SelectedDate = DateTime.Now;
                    dgPhone.ItemsSource = dgEmail.ItemsSource = null;
                    _ID_Row = 0;
                }
            }
            catch (Exception ex)
            {
                MessageService.MetroMessageDialogError(ex.Message);
            }
        }

    }
}
