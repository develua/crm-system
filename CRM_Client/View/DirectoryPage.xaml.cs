using CRM_Client.Model;
using CRM_Client.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

namespace CRM_Client.View
{
    public partial class DirectoryPage : Page
    {
        DatabaseCRMEntities _databasenEtities = new DatabaseCRMEntities();
        Type _type;

        public DirectoryPage(string title, Type typeObj)
        {
            InitializeComponent();

            Title = title;
            _type = typeObj;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadContent();
        }

        // Слхранение изменений

        private async void btnSeveType_Click(object sender, RoutedEventArgs e)
        {
            int resultDB = await _databasenEtities.SaveChangesAsync();
            MessageService.MetroMessageDialogResult(resultDB);
        }

        // Добавление нового элемента

        private async void btnAddNewType_Click(object sender, RoutedEventArgs e)
        {
            if (_type == typeof(ConditionTask))
                _databasenEtities.ConditionTask.Add(new ConditionTask { Name = tbNewElement.Text });
            else if (_type == typeof(PostPeople))
                _databasenEtities.PostPeople.Add(new PostPeople { Name = tbNewElement.Text });
            else if (_type == typeof(ConditionDeal))
                _databasenEtities.ConditionDeal.Add(new ConditionDeal { Name = tbNewElement.Text });
            else if (_type == typeof(TypeGoods))
                _databasenEtities.TypeGoods.Add(new TypeGoods { Name = tbNewElement.Text });
            else if (_type == typeof(TypePhone))
                _databasenEtities.TypePhone.Add(new TypePhone { Name = tbNewElement.Text });
            else if (_type == typeof(TypeEmail))
                _databasenEtities.TypeEmail.Add(new TypeEmail { Name = tbNewElement.Text });

            int resultDB = await _databasenEtities.SaveChangesAsync();
            MessageService.MetroMessageDialogResult(resultDB);

            tbNewElement.Text = "";
            LoadContent();
        }

        // Загрузка выбраного справочника

        private async void LoadContent()
        {
            if (_type == typeof(ConditionTask))
                dgMain.ItemsSource = await _databasenEtities.ConditionTask.OrderBy((a) => a.Name).ToArrayAsync();
            else if(_type == typeof(PostPeople))
                dgMain.ItemsSource = await _databasenEtities.PostPeople.OrderBy((a) => a.Name).ToArrayAsync();
            else if(_type == typeof(ConditionDeal))
                dgMain.ItemsSource = await _databasenEtities.ConditionDeal.OrderBy((a) => a.Name).ToArrayAsync();
            else if (_type == typeof(TypeGoods))
                dgMain.ItemsSource = await _databasenEtities.TypeGoods.OrderBy((a) => a.Name).ToArrayAsync();
            else if (_type == typeof(TypePhone))
                dgMain.ItemsSource = await _databasenEtities.TypePhone.OrderBy((a) => a.Name).ToArrayAsync();
            else if (_type == typeof(TypeEmail))
                dgMain.ItemsSource = await _databasenEtities.TypeEmail.OrderBy((a) => a.Name).ToArrayAsync();

            prLoadData.IsActive = false;
        }

        // Удаление элемента

        private async void dgMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && dgMain.SelectedItem != null)
            {
                MessageDialogResult dialogResult = await MessageService.MetroMessageDialogQuestion("Подтверждение удаления", "Вы действительно хотите удалить выбранный элемент?");
                
                if(dialogResult == MessageDialogResult.Affirmative)
                {
                    try
                    {
                        if (_type == typeof(ConditionTask))
                            _databasenEtities.ConditionTask.Remove(dgMain.SelectedItem as ConditionTask);
                        else if (_type == typeof(PostPeople))
                            _databasenEtities.PostPeople.Remove(dgMain.SelectedItem as PostPeople);
                        else if (_type == typeof(ConditionDeal))
                            _databasenEtities.ConditionDeal.Remove(dgMain.SelectedItem as ConditionDeal);
                        else if (_type == typeof(TypeGoods))
                            _databasenEtities.TypeGoods.Remove(dgMain.SelectedItem as TypeGoods);
                        else if (_type == typeof(TypePhone))
                            _databasenEtities.TypePhone.Remove(dgMain.SelectedItem as TypePhone);
                        else if (_type == typeof(TypeEmail))
                            _databasenEtities.TypeEmail.Remove(dgMain.SelectedItem as TypeEmail);

                        await _databasenEtities.SaveChangesAsync();
                        LoadContent();
                    }
                    catch(Exception ex)
                    {
                        MessageService.MetroMessageDialogError(ex.Message);
                    }
                }
            }
        }
    }
}
