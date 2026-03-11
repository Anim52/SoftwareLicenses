using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SoftwareLicenses.ViewModels.Dashboard
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty] private int expiringCount;
        [ObservableProperty] private int expiredCount;
        [ObservableProperty] private int noLicenseInstallationsCount;
        [ObservableProperty] private int overusedLicensesCount;

        [ObservableProperty] private ObservableCollection<ExpiringLicenseRow> expiringLicenses = new();

        public DashboardViewModel()
        {
            Load();
        }

        [RelayCommand]
        public void Load()
        {
            using var db = new AppDbContext();

            var today = DateOnly.FromDateTime(DateTime.Today);
            var until = today.AddDays(30);

            // 1) Истекают (Active + ExpireDate в пределах 30 дней)
            var expiringLicensesQuery = db.Licenses
                .AsNoTracking()
                .Include(l => l.Software)
                .Where(l =>
                    l.Status == LicenseStatus.Active &&
                    l.ExpireDate != null &&
                    l.ExpireDate >= today &&
                    l.ExpireDate <= until);

            // 2) Просроченные (ExpireDate < today ИЛИ статус Expired)
            expiredCount = db.Licenses.AsNoTracking()
                .Count(l => (l.ExpireDate != null && l.ExpireDate < today) || l.Status == LicenseStatus.Expired);

            // 3) Установки без лицензии (и ПО не бесплатное)
            noLicenseInstallationsCount = db.Installations.AsNoTracking()
                .Include(i => i.Software)
                .Count(i => i.LicenseId == null && i.Software.IsFree == false);

            // UsedSeats по каждой лицензии
            var usedSeatsByLicense = db.Installations.AsNoTracking()
                .Where(i => i.LicenseId != null)
                .GroupBy(i => i.LicenseId!.Value)
                .Select(g => new { LicenseId = g.Key, Used = g.Count() })
                .ToList()
                .ToDictionary(x => x.LicenseId, x => x.Used);

            // 4) Превышение мест (Used > Seats)
            overusedLicensesCount = db.Licenses.AsNoTracking()
                .ToList()
                .Count(l =>
                {
                    var used = usedSeatsByLicense.TryGetValue(l.Id, out var u) ? u : 0;
                    return l.Seats > 0 && used > l.Seats;
                });

            // Таблица истекающих
            var expiringList = expiringLicensesQuery
                .OrderBy(l => l.ExpireDate)
                .ToList();

            ExpiringCount = expiringList.Count;

            ExpiringLicenses = new ObservableCollection<ExpiringLicenseRow>(
                expiringList.Select(l =>
                {
                    var used = usedSeatsByLicense.TryGetValue(l.Id, out var u) ? u : 0;
                    return new ExpiringLicenseRow
                    {
                        LicenseId = l.Id,
                        SoftwareName = l.Software?.Name ?? "",
                        KeyOrContract = l.KeyOrContract,
                        Status = l.Status.ToString(),
                        ExpireDate = l.ExpireDate,
                        Seats = l.Seats,
                        UsedSeats = used
                    };
                })
            );
        }
    }
}
