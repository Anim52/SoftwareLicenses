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

namespace SoftwareLicenses.ViewModels.Installations
{
    public partial class InstallationsPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Installation> installations = new();
        [ObservableProperty] private Installation? selected;

        public InstallationsPageViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();

            var list = db.Installations
                .AsNoTracking()
                .Include(i => i.Device)
                .Include(i => i.Software)
                .Include(i => i.License)
                    .ThenInclude(l => l.Software)
                    .Include(i => i.InstalledByEmployee)
                .OrderBy(i => i.Device.InventoryNumber)
                .ThenBy(i => i.Software.Name)
                .ToList();

            Installations = new ObservableCollection<Installation>(list);
        }

        [RelayCommand]
        public void Add()
        {
            var vm = new InstallationEditViewModel(isEdit: false);

            var win = new InstallationEditWindow
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

            var vm = new InstallationEditViewModel(isEdit: true, source: Selected);

            var win = new InstallationEditWindow
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

            if (MessageBox.Show(
                $"Удалить установку: {Selected.Software?.Name} на {Selected.Device?.InventoryNumber}?",
                "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            var entity = db.Installations.FirstOrDefault(x => x.Id == Selected.Id);
            if (entity is null) return;

            db.Installations.Remove(entity);
            db.SaveChanges();

            Load();
        }
    }
}
