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

namespace SoftwareLicenses.ViewModels.Employees
{
    public partial class EmployeesPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee? selected;

        public EmployeesPageViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();

            var list = db.Employees
                .AsNoTracking()
                .OrderBy(e => e.FullName)
                .ToList();

            Employees = new ObservableCollection<Employee>(list);
        }

        [RelayCommand]
        public void Add()
        {
            var vm = new EmployeeEditViewModel(isEdit: false);

            var win = new EmployeeEditWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (win.ShowDialog() == true)
                Load();
        }

        [RelayCommand]
        public void Edit()
        {
            if (Selected is null) return;

            var vm = new EmployeeEditViewModel(isEdit: true, source: Selected);

            var win = new EmployeeEditWindow
            {
                Owner = Application.Current.MainWindow,
                DataContext = vm
            };

            if (win.ShowDialog() == true)
                Load();
        }

        [RelayCommand]
        public void Delete()
        {
            if (Selected is null) return;

            if (MessageBox.Show($"Удалить сотрудника: {Selected.FullName}?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                using var db = new AppDbContext();
                var entity = db.Employees.FirstOrDefault(x => x.Id == Selected.Id);
                if (entity is null) return;

                db.Employees.Remove(entity);
                db.SaveChanges();

                Load();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show("Нельзя удалить сотрудника, т.к. он назначен ответственным у устройств.",
                    "Ошибка удаления", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
