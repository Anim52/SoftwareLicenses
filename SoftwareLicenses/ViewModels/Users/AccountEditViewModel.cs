using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using SoftwareLicenses.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using System.Windows;

namespace SoftwareLicenses.ViewModels.Users
{
    public partial class AccountEditViewModel : ObservableObject
    {
        private readonly bool _isEdit;
        private readonly int _idToEdit;

        // Список сотрудников (опциональная привязка)
        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee? selectedEmployee;

        // Поля аккаунта
        [ObservableProperty] private string login = "";
        [ObservableProperty] private UserRole role = UserRole.Technician;
        [ObservableProperty] private bool isActive = true;

        // Пароль: при редактировании можно не менять
        [ObservableProperty] private string password = "";
        [ObservableProperty] private string passwordRepeat = "";

        public Array Roles => Enum.GetValues(typeof(UserRole));

        public string Title => _isEdit ? "Редактирование пользователя" : "Добавление пользователя";

        public AccountEditViewModel(bool isEdit, Account? source = null)
        {
            _isEdit = isEdit;

            LoadEmployees();

            if (_isEdit && source != null)
            {
                _idToEdit = source.Id;

                Login = source.Login;
                Role = source.Role;
                IsActive = source.IsActive;

                if (source.EmployeeId.HasValue)
                    SelectedEmployee = Employees.FirstOrDefault(e => e.Id == source.EmployeeId.Value);

                // пароль специально не заполняем
            }
        }

        private void LoadEmployees()
        {
            using var db = new AppDbContext();
            var list = db.Employees.AsNoTracking().OrderBy(e => e.FullName).ToList();
            Employees = new ObservableCollection<Employee>(list);
        }

        private bool CanSave()
        {
            if (string.IsNullOrWhiteSpace(Login)) return false;

            // если добавление — пароль обязателен
            if (!_isEdit)
                return !string.IsNullOrWhiteSpace(Password) && Password == PasswordRepeat;

            // если редактирование — пароль можно не трогать, но если ввели, то должен совпасть
            if (!string.IsNullOrWhiteSpace(Password) || !string.IsNullOrWhiteSpace(PasswordRepeat))
                return Password == PasswordRepeat && !string.IsNullOrWhiteSpace(Password);

            return true;
        }

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            using var db = new AppDbContext();

            var normalizedLogin = Login.Trim();

            // уникальность логина
            var exists = db.Accounts.Any(a => a.Login == normalizedLogin && (!_isEdit || a.Id != _idToEdit));
            if (exists)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEdit)
            {
                var entity = db.Accounts.FirstOrDefault(a => a.Id == _idToEdit);
                if (entity is null) return;

                Apply(entity, isNew: false);
            }
            else
            {
                var entity = new Account();
                Apply(entity, isNew: true);
                db.Accounts.Add(entity);
            }

            db.SaveChanges();
            CloseDialog(true);
        }

        private void Apply(Account a, bool isNew)
        {
            a.Login = Login.Trim();
            a.Role = Role;
            a.IsActive = IsActive;
            a.EmployeeId = SelectedEmployee?.Id;

            // пароль:
            // - при добавлении обязателен
            // - при редактировании меняем только если введён
            if (isNew || (!string.IsNullOrWhiteSpace(Password)))
                a.PasswordHash = PasswordHasher.Hash(Password);
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

        partial void OnLoginChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
        partial void OnPasswordChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
        partial void OnPasswordRepeatChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
        partial void OnRoleChanged(UserRole value) => SaveCommand.NotifyCanExecuteChanged();
        partial void OnIsActiveChanged(bool value) => SaveCommand.NotifyCanExecuteChanged();
    }
}
