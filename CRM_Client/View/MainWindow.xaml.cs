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
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading;
using CRM_Client.Model;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using CRM_Client.Service;
using System.Security.Cryptography;

namespace CRM_Client.View
{
    public partial class MainWindow : MetroWindow
    {
        Random _rand = new Random();
        bool _isOddNum;
        int _ID_User;
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();
        DispatcherTimer _dispatcherTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DesktopPage();

            // Вход в систему

            while (true)
            {
                LoginDialogData resultLogin = await MessageService.MetroLoginDialog();

                if (resultLogin != null)
                {
                    ProgressDialogController controller = await this.ShowProgressAsync("Пожалуйста ожидайте", "Идет проверка данных...");

                    Staff staff = await Task<Staff>.Factory.StartNew(() => _databasenEtities.Staff.SingleOrDefault(a => a.Login == resultLogin.Username));

                    using (MD5 md5Hash = MD5.Create())
                    {
                        await controller.CloseAsync();

                        if (staff != null && HashMD5.VerifyMd5Hash(md5Hash, resultLogin.Password, staff.Password))
                        {
                            Application.Current.Properties["ID_User"] = _ID_User = staff.ID;
                            break;
                        }
                        else
                            await MessageService.MetroMessageDialog("Результат входа", "Вы ввели неверный логин или пароль!");
                    }
                }
                else
                    Close();
            }

            _dispatcherTimer.Interval = new TimeSpan(0, 5, 0);
            _dispatcherTimer.Tick += dispatcherTimer_Tick;
            _dispatcherTimer.Start();
        }

        // Собитие таймера

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            DateTime timeFrom = DateTime.Now - new TimeSpan(0, 5, 0);

            Staff staff = _databasenEtities.Staff.Single(a => a.ID == _ID_User);
            Reminder[] reminder = _databasenEtities.Reminder.Where(a => a.ID_Staff == _ID_User &&
                                   a.DateEvent >= timeFrom && a.DateEvent <= DateTime.Now ||
                                   a.ID_Post == staff.ID && a.ID_Staff == null &&
                                   a.DateEvent >= timeFrom && a.DateEvent <= DateTime.Now).ToArray();

            if (reminder.Length != 0)
                MessageService.MetroMessageDialog("Напоминание", reminder[0].Description);
        }

        // Собитие возврата назад

        private void Button_Back_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.NavigationService.CanGoBack)
            {
                MainFrame.NavigationService.GoBack();
                MainFrame.NavigationService.RemoveBackEntry();
                Application.Current.Properties.Remove("id");
            }
        }

        // Собитие изменение содержимого главного Frame

        private void Frame_MainWindow_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!(e.Content is Page))
                return;

            Page page = e.Content as Page;
            tbTitlePage.Text = page.Title;

            scrollMain.ScrollToHome();

            // Выбор направления трасформации содержимого окна

            if (_isOddNum)
                transitioning.Transition = TransitionType.Left;
            else
                transitioning.Transition = TransitionType.Right;

            transitioning.ReloadTransition();
            _isOddNum = !_isOddNum;
        }
        
    }
}
