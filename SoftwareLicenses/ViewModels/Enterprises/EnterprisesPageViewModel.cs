using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using SoftwareLicenses.View.Dialog;
using System.Collections.ObjectModel;
using System.Windows;

namespace SoftwareLicenses.ViewModels.Enterprises
{
    public partial class EnterprisesPageViewModel : ObservableObject
    {
        [ObservableProperty] private ObservableCollection<Enterprise> enterprises = new();
        [ObservableProperty] private Enterprise? selected;

        public EnterprisesPageViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();

            var list = db.Enterprises
                .AsNoTracking()
                .Include(e => e.Devices)
                .Include(e => e.Licenses)
                .OrderBy(e => e.Name)
                .ToList();

            Enterprises = new ObservableCollection<Enterprise>(list);
        }

        [RelayCommand]
        public void Add()
        {
            var vm = new EnterpriseEditViewModel(isEdit: false);
            var win = new EnterpriseEditWindow { DataContext = vm };
            var owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (owner != null && owner != win) win.Owner = owner;
            if (win.ShowDialog() == true) Load();
        }

        [RelayCommand]
        public void Edit()
        {
            if (Selected is null) return;
            var vm = new EnterpriseEditViewModel(isEdit: true, source: Selected);
            var win = new EnterpriseEditWindow { DataContext = vm };
            var owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (owner != null && owner != win) win.Owner = owner;
            if (win.ShowDialog() == true) Load();
        }

        [RelayCommand]
        public void Delete()
        {
            if (Selected is null) return;

            if (MessageBox.Show($"Удалить предприятие: {Selected.Name}? Устройства и лицензии останутся, но без привязки к предприятию.",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            using var db = new AppDbContext();
            var entity = db.Enterprises.FirstOrDefault(x => x.Id == Selected.Id);
            if (entity is null) return;

            db.Enterprises.Remove(entity);
            db.SaveChanges();
            Load();
        }
    }
}
