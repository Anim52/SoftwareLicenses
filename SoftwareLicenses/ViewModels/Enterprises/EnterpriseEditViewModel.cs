using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using System.Windows;

namespace SoftwareLicenses.ViewModels.Enterprises
{
    public partial class EnterpriseEditViewModel : ObservableObject
    {
        private readonly bool _isEdit;
        private readonly int _idToEdit;

        [ObservableProperty] private string name = "";
        [ObservableProperty] private string? inn;
        [ObservableProperty] private string? address;
        [ObservableProperty] private string? contactPerson;
        [ObservableProperty] private string? phone;
        [ObservableProperty] private string? email;
        [ObservableProperty] private string? notes;

        public string Title => _isEdit ? "Редактирование предприятия" : "Добавление предприятия";

        public EnterpriseEditViewModel(bool isEdit, Enterprise? source = null)
        {
            _isEdit = isEdit;

            if (_isEdit && source != null)
            {
                _idToEdit = source.Id;
                Name = source.Name;
                Inn = source.Inn;
                Address = source.Address;
                ContactPerson = source.ContactPerson;
                Phone = source.Phone;
                Email = source.Email;
                Notes = source.Notes;
            }
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            using var db = new AppDbContext();

            var nameTrimmed = Name.Trim();
            var exists = db.Enterprises.Any(e => e.Name == nameTrimmed && (!_isEdit || e.Id != _idToEdit));
            if (exists)
            {
                MessageBox.Show("Предприятие с таким названием уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEdit)
            {
                var entity = db.Enterprises.FirstOrDefault(x => x.Id == _idToEdit);
                if (entity is null) return;
                Apply(entity);
            }
            else
            {
                var entity = new Enterprise();
                Apply(entity);
                db.Enterprises.Add(entity);
            }

            db.SaveChanges();
            CloseDialog(true);
        }

        private void Apply(Enterprise e)
        {
            e.Name = Name.Trim();
            e.Inn = string.IsNullOrWhiteSpace(Inn) ? null : Inn.Trim();
            e.Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim();
            e.ContactPerson = string.IsNullOrWhiteSpace(ContactPerson) ? null : ContactPerson.Trim();
            e.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();
            e.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
            e.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
        }

        [RelayCommand]
        public void Cancel() => CloseDialog(false);

        private void CloseDialog(bool result)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (ReferenceEquals(w.DataContext, this))
                {
                    w.DialogResult = result;
                    w.Close();
                    return;
                }
            }
        }

        partial void OnNameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    }
}
