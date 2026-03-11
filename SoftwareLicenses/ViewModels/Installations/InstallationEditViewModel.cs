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
using static Azure.Core.HttpHeader;

namespace SoftwareLicenses.ViewModels.Installations
{
    public partial class InstallationEditViewModel : ObservableObject
    {
        private readonly bool _isEdit;
        private readonly int _idToEdit;

        // списки для ComboBox
        [ObservableProperty] private ObservableCollection<Device> devices = new();
        [ObservableProperty] private ObservableCollection<Software> softwares = new();
        [ObservableProperty] private ObservableCollection<License> licenses = new();
        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee? selectedInstalledBy;

        // выбранные значения
        [ObservableProperty] private Device? selectedDevice;
        [ObservableProperty] private Software? selectedSoftware;
        [ObservableProperty] private License? selectedLicense;

        // поля установки
        [ObservableProperty] private string? installedVersion;
        [ObservableProperty] private DateTime installDate = DateTime.Today; // для DatePicker
        [ObservableProperty] private string? installedBy;
        [ObservableProperty] private string? notes;

        public string Title => _isEdit ? "Редактирование установки" : "Добавление установки";

        public InstallationEditViewModel(bool isEdit, Installation? source = null)
        {
            _isEdit = isEdit;

            LoadDevices();
            LoadSoftwares();
            LoadEmployees();

            // Лицензии будут грузиться под выбранное ПО
            Licenses = new ObservableCollection<License>();

            if (_isEdit && source != null)
            {
                _idToEdit = source.Id;

                SelectedDevice = Devices.FirstOrDefault(d => d.Id == source.DeviceId);
                SelectedSoftware = Softwares.FirstOrDefault(s => s.Id == source.SoftwareId);

                InstalledVersion = source.InstalledVersion;
                InstallDate = source.InstallDate.ToDateTime(TimeOnly.MinValue);
                InstalledBy = source.InstalledBy;
                Notes = source.Notes;

                // когда ПО уже выбрано — подгрузим лицензии и выберем нужную
                if (SelectedSoftware != null)
                {
                    LoadLicensesForSoftware(SelectedSoftware.Id);
                    if (source.LicenseId.HasValue)
                        SelectedLicense = Licenses.FirstOrDefault(l => l.Id == source.LicenseId.Value);
                    if (source.InstalledByEmployeeId.HasValue)
                        SelectedInstalledBy = Employees.FirstOrDefault(e => e.Id == source.InstalledByEmployeeId.Value);

                }
            }
        }

        private void LoadDevices()
        {
            using var db = new AppDbContext();
            var list = db.Devices.AsNoTracking()
                .OrderBy(d => d.InventoryNumber)
                .ToList();
            Devices = new ObservableCollection<Device>(list);
        }

        private void LoadSoftwares()
        {
            using var db = new AppDbContext();
            var list = db.Softwares.AsNoTracking()
                .OrderBy(s => s.Name)
                .ToList();
            Softwares = new ObservableCollection<Software>(list);
        }
        private void LoadEmployees()
        {
            using var db = new AppDbContext();
            var list = db.Employees.AsNoTracking().OrderBy(e => e.FullName).ToList();
            Employees = new ObservableCollection<Employee>(list);
        }

        private void LoadLicensesForSoftware(int softwareId)
        {
            using var db = new AppDbContext();

            var list = db.Licenses.AsNoTracking()
                .Where(l => l.SoftwareId == softwareId)
                .OrderByDescending(l => l.Status == LicenseStatus.Active)
                .ThenBy(l => l.ExpireDate)
                .ToList();

            Licenses = new ObservableCollection<License>(list);

            // если текущая выбранная лицензия не относится к этому ПО — сбросим
            if (SelectedLicense != null && SelectedLicense.SoftwareId != softwareId)
                SelectedLicense = null;
        }

        private bool CanSave() => SelectedDevice != null && SelectedSoftware != null;

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            using var db = new AppDbContext();

            // Проверка уникальности (DeviceId + SoftwareId)
            var exists = db.Installations.Any(i =>
                i.DeviceId == SelectedDevice!.Id &&
                i.SoftwareId == SelectedSoftware!.Id &&
                (!_isEdit || i.Id != _idToEdit));

            if (exists)
            {
                MessageBox.Show("Такая установка уже существует (на этом устройстве это ПО уже добавлено).",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEdit)
            {
                var entity = db.Installations.FirstOrDefault(x => x.Id == _idToEdit);
                if (entity is null) return;

                Apply(entity);
            }
            else
            {
                var entity = new Installation();
                Apply(entity);
                db.Installations.Add(entity);
            }

            db.SaveChanges();
            CloseDialog(true);
        }

        private void Apply(Installation i)
        {
            i.DeviceId = SelectedDevice!.Id;
            i.SoftwareId = SelectedSoftware!.Id;

            i.LicenseId = SelectedLicense?.Id;

            i.InstalledVersion = string.IsNullOrWhiteSpace(InstalledVersion) ? null : InstalledVersion.Trim();
            i.InstallDate = DateOnly.FromDateTime(InstallDate);
            i.InstalledByEmployeeId = SelectedInstalledBy?.Id;
            i.InstalledBy = string.IsNullOrWhiteSpace(InstalledBy) ? null : InstalledBy.Trim();
            i.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
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

        partial void OnSelectedSoftwareChanged(Software? value)
        {
            SaveCommand.NotifyCanExecuteChanged();

            if (value is null)
            {
                Licenses = new ObservableCollection<License>();
                SelectedLicense = null;
                return;
            }

            LoadLicensesForSoftware(value.Id);
        }

        partial void OnSelectedDeviceChanged(Device? value) => SaveCommand.NotifyCanExecuteChanged();
    }
}
