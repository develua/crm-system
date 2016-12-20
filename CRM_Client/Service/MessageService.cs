using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CRM_Client.Service
{
    class MessageService
    {
        static MetroWindow mainWindow = App.Current.MainWindow as MetroWindow;
        static MetroDialogSettings dialogSettings = new MetroDialogSettings();

        static MessageService()
        {
            dialogSettings.AffirmativeButtonText = "Закрыть";
        }

        public static void MetroMessageDialogResult(int res)
        {
            string message = (res > -1) ? "Операция была успешно выполнена." : "Во время выполнения операции произошла ошибка.";
            mainWindow.ShowMessageAsync("Результат операции", message, MessageDialogStyle.Affirmative, dialogSettings);
        }

        public static void MetroMessageDialogError(string message)
        {
            mainWindow.ShowMessageAsync("Произошла ошибка", message, MessageDialogStyle.Affirmative, dialogSettings);
        }

        public static Task<MessageDialogResult> MetroMessageDialog(string title, string message)
        {
            return mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.Affirmative, dialogSettings);
        }

        public static Task<MessageDialogResult> MetroMessageDialogQuestion(string title, string message)
        {
            MetroDialogSettings settings = new MetroDialogSettings();
            settings.AffirmativeButtonText = "Да";
            settings.NegativeButtonText = "Нет";

            return mainWindow.ShowMessageAsync(title, message, MessageDialogStyle.AffirmativeAndNegative, settings);
        }

        public static Task<LoginDialogData> MetroLoginDialog()
        {
            LoginDialogSettings settings = new LoginDialogSettings();
            settings.AffirmativeButtonText = "Вход";
            settings.NegativeButtonText = "Отмена";
            settings.UsernameWatermark = "Логин";
            settings.PasswordWatermark = "Пароль";
            settings.NegativeButtonVisibility = Visibility.Visible;

            return mainWindow.ShowLoginAsync("Вход в систему", "Введите ваши данные для входа.", settings);
        }
    }
}
