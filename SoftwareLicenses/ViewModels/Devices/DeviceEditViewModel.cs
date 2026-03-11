using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using static Azure.Core.HttpHeader;

namespace SoftwareLicenses.ViewModels.Devices
{
    public partial class DeviceEditViewModel : ObservableObject
    {
        private readonly bool _isEdit;
        private readonly int _idToEdit;

        // список сотрудников (ответственный)
        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee? selectedEmployee;

        // поля устройства
        [ObservableProperty] private string inventoryNumber = "";
        [ObservableProperty] private string hostname = "";
        [ObservableProperty] private string? serialNumber;
        [ObservableProperty] private string? operatingSystem;
        [ObservableProperty] private string? location;
        [ObservableProperty] private string? notes;

        public string Title => _isEdit ? "Редактирование устройства" : "Добавление устройства";

        public DeviceEditViewModel(bool isEdit, Device? source = null)
        {
            _isEdit = isEdit;

            LoadEmployees();

            if (_isEdit && source != null)
            {
                _idToEdit = source.Id;

                InventoryNumber = source.InventoryNumber;
                Hostname = source.Hostname;
                SerialNumber = source.SerialNumber;
                OperatingSystem = source.OperatingSystem;
                Location = source.Location;
                Notes = source.Notes;

                if (source.ResponsibleEmployeeId.HasValue)
                    SelectedEmployee = Employees.FirstOrDefault(e => e.Id == source.ResponsibleEmployeeId.Value);
            }
        }

        private void LoadEmployees()
        {
            using var db = new AppDbContext();
            var list = db.Employees.AsNoTracking().OrderBy(e => e.FullName).ToList();
            Employees = new ObservableCollection<Employee>(list);
        }

        private bool CanSave()
            => !string.IsNullOrWhiteSpace(InventoryNumber) && !string.IsNullOrWhiteSpace(Hostname);

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            using var db = new AppDbContext();

            // Проверка уникальности InventoryNumber (чтоб не падало некрасиво)
            var inv = InventoryNumber.Trim();
            var exists = db.Devices.Any(d => d.InventoryNumber == inv && (!_isEdit || d.Id != _idToEdit));
            if (exists)
            {
                MessageBox.Show("Устройство с таким инвентарным номером уже существует.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEdit)
            {
                var entity = db.Devices.FirstOrDefault(x => x.Id == _idToEdit);
                if (entity is null) return;

                Apply(entity);
            }
            else
            {
                var entity = new Device();
                Apply(entity);
                db.Devices.Add(entity);
            }

            db.SaveChanges();
            CloseDialog(true);
        }

        private void Apply(Device d)
        {
            d.InventoryNumber = InventoryNumber.Trim();
            d.Hostname = Hostname.Trim();

            d.SerialNumber = string.IsNullOrWhiteSpace(SerialNumber) ? null : SerialNumber.Trim();
            d.OperatingSystem = string.IsNullOrWhiteSpace(OperatingSystem) ? null : OperatingSystem.Trim();
            d.Location = string.IsNullOrWhiteSpace(Location) ? null : Location.Trim();
            d.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();

            d.ResponsibleEmployeeId = SelectedEmployee?.Id;
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

        partial void OnInventoryNumberChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
        partial void OnHostnameChanged(string value) => SaveCommand.NotifyCanExecuteChanged();
    }
}
