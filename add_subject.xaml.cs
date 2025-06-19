using diplom.Models;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using Window = System.Windows.Window;

namespace diplom
{

    public partial class add_subject : Window, INotifyPropertyChanged, IDataErrorInfo
    {
        private string _subjectName;
        private string _description;
        private string _groupName;
        private diplom.Models.Type _selectedType;
        private List<diplom.Models.Type> _types;
        public string SubjectName
        {
            get { return _subjectName; }
            set
            {
                if (_subjectName != value)
                {
                    _subjectName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public diplom.Models.Type SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<diplom.Models.Type> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                OnPropertyChanged();
            }
        }

        public string GroupName
        {
            get { return _groupName; }
            set
            {
                if (_groupName != value)
                {
                    _groupName = value;
                    OnPropertyChanged();
                }
            }
        }
        public DiplomSchoolContext db = new DiplomSchoolContext();
        public add_subject()
        {
            InitializeComponent();
            LoadTypes();
            DataContext = this;

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }
        private void LoadTypes()
        {
            try
            {
                List<diplom.Models.Type> types = db.Types.Where(u => u.Id != 0).ToList();

                TypeComboBox.ItemsSource = types;
                TypeComboBox.DisplayMemberPath = "Type1";
                TypeComboBox.SelectedValuePath = "Id";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                        "Не получилось загрузить типы",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                Debug.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(this["SubjectName"]) && string.IsNullOrEmpty(this["Description"]) && SelectedType != null &&
                string.IsNullOrEmpty(this["GroupName"]))
            {
                try
                {

                    var lastSubject = db.Subjects.OrderByDescending(s => s.Idsubjects).FirstOrDefault();
                    int newSubjectId = lastSubject != null ? lastSubject.Idsubjects + 1 : 1;

                    if (TypeComboBox.SelectedItem is diplom.Models.Type selectedType)
                    {

                        Subject newSubject = new Subject
                        {

                            Idsubjects = newSubjectId,
                            Name = SubjectNameTextBox.Text,
                            Description = DescriptionTextBox.Text,
                        };

                        types_subjects typesSubject = new types_subjects
                        {
                            types_id = selectedType.Id,
                            subjects_idsubjects = newSubjectId

                        };

                        Group group = new Group
                        {
                            Name = GroupNameTextBox.Text
                        };

                        db.Subjects.Add(newSubject);
                        db.TypesSubjects.Add(typesSubject);
                        db.Groups.Add(group);
                        db.SaveChanges();

                        App.ShowToast("Предмет добавлен успешно");
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show(
                        "Тип не был выбран.",
                        "Уведомление",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );

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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Все несохраненные изменения будут утеряны. Закрыть окно?", "Предупреждение", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "SubjectName":
                        if (string.IsNullOrEmpty(SubjectName))
                        {
                            error = "Название занятия обязательно для заполнения.";
                        }
                        else if (SubjectName.Length > 45)
                        {
                            error = "Название занятия не должно превышать 45 символов.";
                        }
                        break;
                    case "Description":
                        if (string.IsNullOrEmpty(Description))
                        {
                            error = "Описание обязательно для заполнения.";
                        }
                        else if (Description.Length > 150)
                        {
                            error = "Описание не должно превышать 150 символов.";
                        }
                        break;
                    case "GroupName":
                        if (string.IsNullOrEmpty(GroupName))
                        {
                            error = "Название группы обязательно для заполнения.";
                        }
                        else if (GroupName.Length > 50)
                        {
                            error = "Название группы не должно превышать 50 символов.";
                        }
                        break;
                    case "SelectedType":
                        if (SelectedType == null)
                        {
                            error = "Тип занятия обязателен для выбора.";
                        }
                        break;
                }
                return error;
            }
        }

        public string Error
        {
            get { return null; }
        }

      
    }
}
