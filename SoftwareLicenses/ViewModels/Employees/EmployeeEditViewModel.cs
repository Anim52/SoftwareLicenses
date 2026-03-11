using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SoftwareLicenses.ViewModels.Employees
{
    public partial class EmployeeEditViewModel : ObservableObject
    {
        private readonly bool _isEdit;
        private readonly int _idToEdit;

        [ObservableProperty] private string fullName = "";
        [ObservableProperty] private string? position;
        [ObservableProperty] private string? department;
        [ObservableProperty] private string? email;

        public string Title => _isEdit ? "Редактирование сотрудника" : "Добавление сотрудника";

        public EmployeeEditViewModel(bool isEdit, Employee? source = null)
        {
            _isEdit = isEdit;

            if (_isEdit && source != null)
            {
                _idToEdit = source.Id;
                FullName = source.FullName;
                Position = source.Position;
                Department = source.Department;
                Email = source.Email;
            }
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(FullName);

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            using var db = new AppDbContext();

            if (_isEdit)
            {
                var entity = db.Employees.FirstOrDefault(x => x.Id == _idToEdit);
                if (entity is null) return;

                Apply(entity);
            }
            else
            {
                var entity = new Employee();
                Apply(entity);
                db.Employees.Add(entity);
            }

            db.SaveChanges();
            CloseDialog(true);
        }

        private void Apply(Employee e)
        {
            e.FullName = FullName.Trim();
            e.Position = string.IsNullOrWhiteSpace(Position) ? null : Position.Trim();
            e.Department = string.IsNullOrWhiteSpace(Department) ? null : Department.Trim();
            e.Email = string.IsNullOrWhiteSpace(Email) ? null : Email.Trim();
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

        partial void OnFullNameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    }
}
