﻿using diplom.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace diplom
{

    public partial class subject : Window, IDataErrorInfo
    {

        public string Error => null;

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "Name":
                        if (string.IsNullOrEmpty(name.Text))
                        {
                            error = "Имя занятия обязательно для заполнения.";
                        }
                        else if (name.Text.Length > 255)
                        {
                            error = "Имя занятия не должно превышать 255 символов.";
                        }
                        break;
                    case "Description":
                        if (string.IsNullOrEmpty(description.Text))
                        {
                            error = "Описание обязательно для заполнения.";
                        }
                        break;
                }
                return error;
            }
        }
        public DiplomSchoolContext db = new DiplomSchoolContext();
        private subjectsshow constS = new subjectsshow();
        public int constID;
        public subject(int ID)
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            constID = ID;
            NewData();
        }

        public void NewData()
        {

            try
            {
                constS = db.SubjectShowItems
                   .FromSqlRaw($"SELECT * FROM subjectsshow WHERE subject_id = {constID}")
                   .FirstOrDefault();

                if (constS != null)
                {
                    Id.Text = constS.subject_id.ToString();
                    name.Text = constS.subject_name;
                    description.Text = constS.description;
                    IdType.Text = constS.type_id.ToString();
                    type.Text = constS.type_name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "Не получилось загрузить занятие.",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error
                   );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить занятие? Это может привести к необратимым потерям данных. ", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.No)
                {
                    var subjectToDelete = db.Subjects.Find(constS.subject_id);
                    if (subjectToDelete != null)
                    {
                        db.Subjects.Remove(subjectToDelete);
                        db.SaveChanges();

                        App.ShowToast("Занятие успешно удалено!");
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                       "Не получилось найти данное занятие.",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error
                   );

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        "Возникла неизвестная проблема. Пожалуйста, попробуйте позднее.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            name.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            description.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();


            if (string.IsNullOrEmpty(this["Name"]) == false || string.IsNullOrEmpty(this["Description"]) == false)
            {
                MessageBox.Show(
                       "Все обязательные поля должны быть заполнены!",
                       "Уведомление",
                       MessageBoxButton.OK,
                       MessageBoxImage.Warning
                   );
                return;
            }

            try
            {
                var subjectToUpdate = db.Subjects.Find(constS.subject_id);
                if (subjectToUpdate != null)
                {
                    subjectToUpdate.Name = name.Text;
                    subjectToUpdate.Description = description.Text;

                    db.SaveChanges();

                    App.ShowToast("Изменения успешно сохранены.");
                    this.DialogResult = true;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Занятие не найдено.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                       "Возникла неизвестная проблема. Пожалуйста, попробуйте позднее.",
                       "Ошибка",
                       MessageBoxButton.OK,
                       MessageBoxImage.Error
                   );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
