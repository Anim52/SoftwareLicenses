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

namespace SoftwareLicenses.ViewModels.Licenses
{
    public partial class LicenseEditViewModel : ObservableObject
    {
        private readonly bool _isEdit;
        private readonly int _idToEdit;

        // Список ПО для ComboBox
        [ObservableProperty] private ObservableCollection<Software> softwares = new();

        [ObservableProperty] private ObservableCollection<Supplier> suppliers = new();
        [ObservableProperty] private Supplier? selectedSupplier;


        // Выбранное ПО
        [ObservableProperty] private Software? selectedSoftware;

        // Поля лицензии
        [ObservableProperty] private LicenseType type = LicenseType.Volume;
        [ObservableProperty] private LicenseStatus status = LicenseStatus.Active;

        [ObservableProperty] private string? keyOrContract;
        [ObservableProperty] private int seats = 1;

        // Для WPF DatePicker используем DateTime?
        [ObservableProperty] private DateTime purchaseDate = DateTime.Today;
        [ObservableProperty] private DateTime? expireDate;

        [ObservableProperty] private decimal? cost;
        [ObservableProperty] private string? notes;

        public Array LicenseTypes => Enum.GetValues(typeof(LicenseType));
        public Array LicenseStatuses => Enum.GetValues(typeof(LicenseStatus));

        public string Title => _isEdit ? "Редактирование лицензии" : "Добавление лицензии";

        public LicenseEditViewModel(bool isEdit, License? source = null)
        {
            _isEdit = isEdit;

            LoadSoftwares();
            LoadSuppliers();


            if (_isEdit && source != null)
            {
                _idToEdit = source.Id;

                // ПО (может прийти из Include)
                SelectedSoftware = Softwares.FirstOrDefault(s => s.Id == source.SoftwareId);

                Type = source.Type;
                Status = source.Status;
                KeyOrContract = source.KeyOrContract;
                Seats = source.Seats;

                PurchaseDate = source.PurchaseDate.ToDateTime(TimeOnly.MinValue);
                ExpireDate = source.ExpireDate?.ToDateTime(TimeOnly.MinValue);

                Cost = source.Cost;
                Notes = source.Notes;
                if (source.SupplierId.HasValue)
                    SelectedSupplier = Suppliers.FirstOrDefault(s => s.Id == source.SupplierId.Value);

            }
        }

        private void LoadSoftwares()
        {
            using var db = new AppDbContext();
            var list = db.Softwares.AsNoTracking().OrderBy(s => s.Name).ToList();
            Softwares = new ObservableCollection<Software>(list);
        }
        private void LoadSuppliers()
        {
            using var db = new AppDbContext();
            var list = db.Suppliers.AsNoTracking().OrderBy(s => s.Name).ToList();
            Suppliers = new ObservableCollection<Supplier>(list);
        }

        private bool CanSave() => SelectedSoftware != null && Seats > 0;

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            using var db = new AppDbContext();

            if (_isEdit)
            {
                var entity = db.Licenses.FirstOrDefault(x => x.Id == _idToEdit);
                if (entity is null) return;

                Apply(entity);
            }
            else
            {
                var entity = new License();
                Apply(entity);
                db.Licenses.Add(entity);
            }

            db.SaveChanges();
            CloseDialog(true);
        }

        private void Apply(License entity)
        {
            entity.SoftwareId = SelectedSoftware!.Id;
            entity.Type = Type;
            entity.Status = Status;
            entity.SupplierId = SelectedSupplier?.Id;

            entity.KeyOrContract = string.IsNullOrWhiteSpace(KeyOrContract) ? null : KeyOrContract.Trim();
            entity.Seats = Seats;

            entity.PurchaseDate = DateOnly.FromDateTime(PurchaseDate);
            entity.ExpireDate = ExpireDate is null ? null : DateOnly.FromDateTime(ExpireDate.Value);

            entity.Cost = Cost;
            entity.Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim();
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

        partial void OnSelectedSoftwareChanged(Software? value) => SaveCommand.NotifyCanExecuteChanged();
        partial void OnSeatsChanged(int value) => SaveCommand.NotifyCanExecuteChanged();
    }
}
