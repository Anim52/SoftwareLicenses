using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using SoftwareLicenses.View.Dialog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace SoftwareLicenses.ViewModels.Users
{
    public partial class AccountsPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Account> accounts = new();
        [ObservableProperty] private Account? selected;

        public AccountsPageViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();

            var list = db.Accounts
                .AsNoTracking()
                .Include(a => a.Employee)
                .OrderBy(a => a.Login)
                .ToList();

            Accounts = new ObservableCollection<Account>(list);
        }

        [RelayCommand]
        public void Add()
        {
            var vm = new AccountEditViewModel(isEdit: false);

            var win = new AccountEditWindow
            {
                DataContext = vm
            };

            var owner = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive);

            if (owner != null && owner != win)
                win.Owner = owner;

            if (win.ShowDialog() == true)
                Load();
        }

        [RelayCommand]
        public void Edit()
        {
            if (Selected is null) return;

            var vm = new AccountEditViewModel(isEdit: true, source: Selected);

            var win = new AccountEditWindow
            {
                DataContext = vm
            };

            var owner = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive);

            if (owner != null && owner != win)
                win.Owner = owner;

            if (win.ShowDialog() == true)
                Load();
        }

        [RelayCommand]
        public void Delete()
        {
            if (Selected is null) return;

            if (MessageBox.Show($"Удалить пользователя: {Selected.Login}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            var entity = db.Accounts.FirstOrDefault(a => a.Id == Selected.Id);
            if (entity is null) return;

            db.Accounts.Remove(entity);
            db.SaveChanges();

            Load();
        }
    }
}
