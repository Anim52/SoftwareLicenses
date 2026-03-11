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

namespace SoftwareLicenses.ViewModels.Softwares
{
    public partial class SoftwaresPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Software> softwares = new();
        [ObservableProperty] private Software? selected;

        public SoftwaresPageViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();
            var list = db.Softwares.AsNoTracking().OrderBy(x => x.Name).ToList();
            Softwares = new ObservableCollection<Software>(list);
        }

        [RelayCommand]
        public void Add()
        {
            // создаём VM окна для добавления
            var vm = new SoftwareEditViewModel(isEdit: false);

            // создаём окно, задаём DataContext и показываем модально
            var win = new SoftwareEditWindow
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

            // передаём выбранный объект в VM окна (копируем значения)
            var vm = new SoftwareEditViewModel(isEdit: true, source: Selected);

            var win = new SoftwareEditWindow
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

            if (MessageBox.Show($"Удалить ПО: {Selected.Name}?", "Подтверждение",
                MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            var entity = db.Softwares.FirstOrDefault(x => x.Id == Selected.Id);
            if (entity is null) return;

            db.Softwares.Remove(entity);
            db.SaveChanges();

            Load();
        }
    }
}
