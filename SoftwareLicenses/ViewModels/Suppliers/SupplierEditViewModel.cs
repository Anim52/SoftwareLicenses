using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SoftwareLicenses.ViewModels.Suppliers
{
    public partial class SupplierEditViewModel : ObservableObject
    {
        private readonly bool _isEdit;
        private readonly int _idToEdit;

        [ObservableProperty] private string name = "";
        [ObservableProperty] private string? inn;
        [ObservableProperty] private string? contact;
        [ObservableProperty] private string? phone;
        [ObservableProperty] private string? email;

        public string Title => _isEdit ? "Редактирование поставщика" : "Добавление поставщика";

        public SupplierEditViewModel(bool isEdit, Supplier? source = null)
        {
            _isEdit = isEdit;

            if (_isEdit && source != null)
            {
                _idToEdit = source.Id;
                Name = source.Name;
                Inn = source.Inn;
                Contact = source.Contact;
                Phone = source.Phone;
                Email = source.Email;
            }
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            using var db = new AppDbContext();

            if (_isEdit)
            {
                var entity = db.Suppliers.FirstOrDefault(x => x.Id == _idToEdit);
                if (entity is null) return;

                Apply(entity);
            }
            else
            {
                var entity = new Supplier();
                Apply(entity);
                db.Suppliers.Add(entity);
            }

            db.SaveChanges();
            CloseDialog(true);
        }

        private void Apply(Supplier s)
        {
            s.Name = Name.Trim();
            s.Inn = string.IsNullOrWhiteSpace(Inn) ? null : Inn.Trim();
            s.Contact = string.IsNullOrWhiteSpace(Contact) ? null : Contact.Trim();
            s.Phone = string.IsNullOrWhiteSpace(Phone) ? null : Phone.Trim();
            s.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
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
