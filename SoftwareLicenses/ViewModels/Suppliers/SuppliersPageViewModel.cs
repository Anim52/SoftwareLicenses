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

namespace SoftwareLicenses.ViewModels.Suppliers
{
    public partial class SuppliersPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Supplier> suppliers = new();
        [ObservableProperty] private Supplier? selected;

        public SuppliersPageViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();

            var list = db.Suppliers
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ToList();

            Suppliers = new ObservableCollection<Supplier>(list);
        }

        [RelayCommand]
        public void Add()
        {
            var vm = new SupplierEditViewModel(isEdit: false);

            var win = new SupplierEditWindow
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

            var vm = new SupplierEditViewModel(isEdit: true, source: Selected);

            var win = new SupplierEditWindow
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

            if (MessageBox.Show($"Удалить поставщика: {Selected.Name}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var entity = db.Suppliers.FirstOrDefault(x => x.Id == Selected.Id);
                if (entity is null) return;

                db.Suppliers.Remove(entity);
                db.SaveChanges();

                Load();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Нельзя удалить поставщика, т.к. он привязан к лицензиям.",
                    "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
