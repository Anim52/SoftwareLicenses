using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoftwareLicenses.Models;
using SoftwareLicenses.Security;
using SoftwareLicenses.ViewModels.Dashboard;
using SoftwareLicenses.ViewModels.Devices;
using SoftwareLicenses.ViewModels.Employees;
using SoftwareLicenses.ViewModels.Enterprises;
using SoftwareLicenses.ViewModels.Installations;
using SoftwareLicenses.ViewModels.Licenses;
using SoftwareLicenses.ViewModels.Reports;
using SoftwareLicenses.ViewModels.Softwares;
using SoftwareLicenses.ViewModels.Suppliers;
using SoftwareLicenses.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.ViewModels.Main
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly AuthService? _auth;

        [ObservableProperty] private object? currentVm;

        // ✅ чтобы работало, если где-то создаёшь без auth
        public MainViewModel()
        {
            CurrentVm = new DashboardViewModel();
        }

        // ✅ вот конструктор, который ты хочешь
        public MainViewModel(AuthService auth)
        {
            _auth = auth;
            CurrentVm = new DashboardViewModel();
        }

        // -------- отображение пользователя ----------
        public string CurrentUserLogin => _auth?.CurrentUser?.Login ?? "";
        public string CurrentUserRole => _auth?.CurrentUser?.Role.ToString() ?? "";

        // -------- доступы по ролям ----------
        public bool IsAdmin => _auth?.CurrentUser?.Role == UserRole.Admin;
        public bool IsManager => _auth?.CurrentUser?.Role == UserRole.Manager;
        public bool IsTechnician => _auth?.CurrentUser?.Role == UserRole.Technician;

        // Пример правил:
        public bool CanManageDirectories => IsAdmin || IsManager; // справочники
        public bool CanManageUsers => IsAdmin;                    // пользователи (аккаунты)
        public bool CanSeeSuppliers => IsAdmin || IsManager;
        public bool CanSeeReports => IsAdmin || IsManager;


        // -------- навигация ----------
        [RelayCommand] private void OpenDashboard() => CurrentVm = new DashboardViewModel();
        [RelayCommand] private void OpenSoftwares() => CurrentVm = new SoftwaresPageViewModel();
        [RelayCommand] private void OpenLicenses() => CurrentVm = new LicensesPageViewModel();
        [RelayCommand] private void OpenDevices() => CurrentVm = new DevicesPageViewModel();
        [RelayCommand] private void OpenInstallations() => CurrentVm = new InstallationsPageViewModel();
        [RelayCommand] private void OpenEmployees() => CurrentVm = new EmployeesPageViewModel();
        [RelayCommand] private void OpenEnterprises() => CurrentVm = new EnterprisesPageViewModel();
        [RelayCommand] private void OpenSuppliers() => CurrentVm = new SuppliersPageViewModel();
        [RelayCommand] private void OpenUsers() => CurrentVm = new AccountsPageViewModel();
        [RelayCommand] private void OpenReports() => CurrentVm = new ReportsPageViewModel();



        // если хочешь логаут:
        [RelayCommand]
        private void Logout()
        {
            _auth?.Logout();
            // дальше можно закрыть MainWindow и снова показать Login (скажу как, если надо)
        }
    }
}
