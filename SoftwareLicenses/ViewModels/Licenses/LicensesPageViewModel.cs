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

namespace SoftwareLicenses.ViewModels.Licenses
{
    public partial class LicensesPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<License> licenses = new();
        [ObservableProperty] private License? selected;

        public LicensesPageViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();

            var list = db.Licenses
     .AsNoTracking()
     .Include(l => l.Software)
     .Include(l => l.Supplier)
     .OrderBy(l => l.Software.Name)
     .ThenByDescending(l => l.ExpireDate)
     .ToList();


            Licenses = new ObservableCollection<License>(list);
        }

        [RelayCommand]
        public void Add()
        {
            var vm = new LicenseEditViewModel(isEdit: false);

            var win = new LicenseEditWindow
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

            var vm = new LicenseEditViewModel(isEdit: true, source: Selected);

            var win = new LicenseEditWindow
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

            if (MessageBox.Show($"Удалить лицензию для: {Selected.Software?.Name}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            var entity = db.Licenses.FirstOrDefault(x => x.Id == Selected.Id);
            if (entity is null) return;

            db.Licenses.Remove(entity);
            db.SaveChanges();

            Load();
        }
    }
}
