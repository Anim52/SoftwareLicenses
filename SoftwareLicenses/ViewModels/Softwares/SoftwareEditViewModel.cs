using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SoftwareLicenses.ViewModels.Softwares
{
    public partial class SoftwareEditViewModel : ObservableObject
    {
        private readonly bool _isEdit;
        private readonly int _idToEdit;

        [ObservableProperty] private string name = "";
        [ObservableProperty] private string vendor = "";
        [ObservableProperty] private string? version;
        [ObservableProperty] private bool isFree;

        public string Title => _isEdit ? "Редактирование ПО" : "Добавление ПО";

        public SoftwareEditViewModel(bool isEdit, Software? source = null)
        {
            _isEdit = isEdit;

            if (_isEdit && source != null)
            {
                _idToEdit = source.Id;
                Name = source.Name;
                Vendor = source.Vendor;
                Version = source.Version;
                IsFree = source.IsFree;
            }
        }

        private bool CanSave() => !string.IsNullOrWhiteSpace(Name);

        [RelayCommand(CanExecute = nameof(CanSave))]
        public void Save()
        {
            using var db = new AppDbContext();

            if (_isEdit)
            {
                var entity = db.Softwares.FirstOrDefault(x => x.Id == _idToEdit);
                if (entity is null) return;

                entity.Name = Name.Trim();
                entity.Vendor = Vendor.Trim();
                entity.Version = string.IsNullOrWhiteSpace(Version) ? null : Version.Trim();
                entity.IsFree = IsFree;
            }
            else
            {
                db.Softwares.Add(new Software
                {
                    Name = Name.Trim(),
                    Vendor = Vendor.Trim(),
                    Version = string.IsNullOrWhiteSpace(Version) ? null : Version.Trim(),
                    IsFree = IsFree
                });
            }

            db.SaveChanges();

            // Закрываем окно с DialogResult = true
            CloseDialog(true);
        }

        [RelayCommand]
        public void Cancel() => CloseDialog(false);

        private void CloseDialog(bool result)
        {
            // Ищем окно, которому принадлежит DataContext = this
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
