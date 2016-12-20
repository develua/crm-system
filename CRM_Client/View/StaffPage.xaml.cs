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
using System.Security.Cryptography;
using MahApps.Metro.Controls.Dialogs;
using System.Text.RegularExpressions;

namespace CRM_Client.View
{
    public partial class StaffPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();
        Staff _newStaff;
        PhoneStaff _newPhone;
        EmailStaff _newEmail;
        List<PhoneStaff> _phoneList;
        List<EmailStaff> _emailList;
        List<PhoneStaff> _delPhoneList = new List<PhoneStaff>();
        List<EmailStaff> _delEmailList = new List<EmailStaff>();
        int _ID_Row = 0;
        bool _isNewRecord;

        public StaffPage()
        {
            InitializeComponent();

            if (Application.Current.Properties.Contains("id"))
            {
                _ID_Row = (int)Application.Current.Properties["id"];
                eHistoryDeal.Visibility = eHistoryTask.Visibility = Visibility.Visible;
                btnSaveData.Background = (LinearGradientBrush)FindResource("Gradient_Amber");
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
                    Staff staff = _newStaff = new Staff();
                    _newPhone = new PhoneStaff();
                    _newEmail = new EmailStaff();
                    _phoneList = new List<PhoneStaff>();
                    _emailList = new List<EmailStaff>();
                    Deal[] dealArr = null;
                    TaskBD[] taskBDArr = null;

                    if (!_isNewRecord)
                    {
                        staff = _databasenEtities.Staff.Single(a => a.ID == _ID_Row);
                        _phoneList.AddRange(_databasenEtities.PhoneStaff.Where(a => a.ID_Staff == _ID_Row).ToArray());
                        _emailList.AddRange(_databasenEtities.EmailStaff.Where(a => a.ID_Staff == _ID_Row).ToArray());

                        dealArr = _databasenEtities.Deal.Where(a => a.ID_Staff == staff.ID).ToArray();
                        taskBDArr = _databasenEtities.TaskBD.Where(a => a.ID_Staff == staff.ID).ToArray();
                    }

                    PostPeople[] postPeopleArr = _databasenEtities.PostPeople.OrderBy(a => a.Name).ToArray();
                    Country[] countryArr = _databasenEtities.Country.OrderBy(a => a.Name).ToArray();
                    TypePhone[] typePhoneArr = _databasenEtities.TypePhone.OrderBy(a => a.Name).ToArray();
                    TypeEmail[] typeEmailArr = _databasenEtities.TypeEmail.OrderBy(a => a.Name).ToArray();

                    Dispatcher.Invoke(() =>
                    {
                        DataContext = staff;
                        cbPostStaff.ItemsSource = postPeopleArr;
                        cbCountry.ItemsSource = countryArr;

                        cbTypePhone.ItemsSource = dgcbTypePhone.ItemsSource = typePhoneArr;
                        cbTypeEmail.ItemsSource = dgcbTypeEmail.ItemsSource = typeEmailArr;

                        gNewPhone.DataContext = _newPhone;
                        gNewEmail.DataContext = _newEmail;
                        dgPhone.ItemsSource = _phoneList.ToArray();
                        dgEmail.ItemsSource = _emailList.ToArray();

                        if (!_isNewRecord)
                        {
                            dgDeal.ItemsSource = dealArr;
                            dgTask.ItemsSource = taskBDArr;
                        }

                        mainGrid.IsEnabled = true;
                    });

                }
                catch(Exception ex)
                {
                    MessageService.MetroMessageDialogError(ex.Message);
                }
            });
        }

        // Событие изменение страны

        private void cbCountry_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbArea.ItemsSource = cbCity.ItemsSource = null;

            if (cbCountry.SelectedValue != null)
                cbArea.ItemsSource = _databasenEtities.Area.Where(a => a.ID_Country == (int)cbCountry.SelectedValue).OrderBy(a => a.Name).ToArray();
        }

        // Событие изменение области

        private void cbArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbArea.SelectedValue != null)
                cbCity.ItemsSource = _databasenEtities.City.Where(a => a.ID_Area == (int)cbArea.SelectedValue).OrderBy(a => a.Name).ToArray();
        }

        // Добавление нового телефона

        private void btnAddNewPhone_Click(object sender, RoutedEventArgs e)
        {
            if (Regex.IsMatch(_newPhone.Phone, @"^\+[0-9]{1,3}-[0-9]{2}-[0-9]{2}-[0-9]{2}-[0-9]{3}$", RegexOptions.IgnoreCase))
            {
                _phoneList.Add(_newPhone);
                gNewPhone.DataContext = _newPhone = new PhoneStaff();
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
                gNewEmail.DataContext = _newEmail = new EmailStaff();
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
                    PhoneStaff phoneStaff = dgPhone.SelectedItem as PhoneStaff;

                    if (phoneStaff.ID != 0)
                        _delPhoneList.Add(phoneStaff);

                    _phoneList.Remove(phoneStaff);
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
                    EmailStaff emailStaff = dgEmail.SelectedItem as EmailStaff;

                    if (emailStaff.ID != 0)
                        _delEmailList.Add(emailStaff);

                    _emailList.Remove(emailStaff);
                    dgEmail.ItemsSource = _emailList.ToArray();
                }
            }
        }

        // Сохранение карточки сотрудника

        private async void bntSaveCard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Шифрование пароля

                if (pbNewPassword.Password != "")
                    using (MD5 md5Hash = MD5.Create())
                    {
                        Staff staff = (_isNewRecord) ? _newStaff : DataContext as Staff;
                        staff.Password = HashMD5.GetMd5Hash(md5Hash, pbNewPassword.Password);
                    }

                // Добавление нового сотрудника

                if (_isNewRecord)
                    _ID_Row = _databasenEtities.Staff.Add(_newStaff).ID;

                // Добавление нового телефона

                for (int i = 0; i < _phoneList.Count; i++)
                    if (_phoneList[i].ID_Staff == 0)
                    {
                        _phoneList[i].ID_Staff = _ID_Row;
                        _databasenEtities.PhoneStaff.Add(_phoneList[i]);
                    }

                // Добавление нового Email

                for (int i = 0; i < _emailList.Count; i++)
                    if (_emailList[i].ID_Staff == 0)
                    {
                        _emailList[i].ID_Staff = _ID_Row;
                        _databasenEtities.EmailStaff.Add(_emailList[i]);
                    }

                // Удаление телефонов и email

                _databasenEtities.PhoneStaff.RemoveRange(_delPhoneList);
                _databasenEtities.EmailStaff.RemoveRange(_delEmailList);

                int resultDB = await _databasenEtities.SaveChangesAsync();
                MessageService.MetroMessageDialogResult(resultDB);

                if (_isNewRecord && resultDB > -1)
                {
                    cbCountry.SelectedIndex = -1;

                    _newStaff = new Staff();
                    DataContext = _newStaff;

                    dgPhone.ItemsSource = dgEmail.ItemsSource = null;
                    _ID_Row = 0;
                }

                pbNewPassword.Password = "";
            }
            catch(Exception ex)
            {
                MessageService.MetroMessageDialogError(ex.Message);
            }
        }

    }
}
