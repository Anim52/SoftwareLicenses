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

namespace SoftwareLicenses.ViewModels.Devices
{
    public partial class DevicesPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Device> devices = new();
        [ObservableProperty] private Device? selected;

        public DevicesPageViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();

            var list = db.Devices
                .AsNoTracking()
                .Include(d => d.ResponsibleEmployee)
                .Include(d => d.Enterprise)
                .OrderBy(d => d.InventoryNumber)
                .ThenBy(d => d.Hostname)
                .ToList();

            Devices = new ObservableCollection<Device>(list);
        }

        [RelayCommand]
        public void Add()
        {
            var vm = new DeviceEditViewModel(isEdit: false);

            var win = new DeviceEditWindow
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

            var vm = new DeviceEditViewModel(isEdit: true, source: Selected);

            var win = new DeviceEditWindow
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

            if (MessageBox.Show($"Удалить устройство: {Selected.InventoryNumber} ({Selected.Hostname})?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var entity = db.Devices.FirstOrDefault(x => x.Id == Selected.Id);
                if (entity is null) return;

                db.Devices.Remove(entity);
                db.SaveChanges();

                Load();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Нельзя удалить устройство, т.к. есть связанные установки ПО. Удалите установки или перенесите их.",
                    "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
