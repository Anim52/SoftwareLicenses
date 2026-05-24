using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using SoftwareLicenses.View.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SoftwareLicenses.ViewModels.Reports
{
    public partial class ReportsPageViewModel : ObservableObject
    {
        [ObservableProperty] private int days = 30;

        [ObservableProperty] private ObservableCollection<ExpiringLicenseReportRow> expiringLicenses = new();
        [ObservableProperty] private ObservableCollection<NoLicenseInstallationReportRow> noLicenseInstallations = new();
        [ObservableProperty] private ObservableCollection<LicenseUsageReportRow> licenseUsage = new();

        public ReportsPageViewModel()
        {
            BuildAll();
        }

        [RelayCommand]
        public void BuildAll()
        {
            BuildExpiring();
            BuildNoLicense();
            BuildUsage();
        }

        [RelayCommand]
        public void BuildExpiring()
        {
            using var db = new AppDbContext();
            var today = DateOnly.FromDateTime(DateTime.Today);
            var until = today.AddDays(Days);

            var usedByLicense = db.Installations.AsNoTracking()
                .Where(i => i.LicenseId != null)
                .GroupBy(i => i.LicenseId!.Value)
                .Select(g => new { LicenseId = g.Key, Used = g.Count() })
                .ToList()
                .ToDictionary(x => x.LicenseId, x => x.Used);

            var list = db.Licenses.AsNoTracking()
                .Include(l => l.Software)
                .Include(l => l.Supplier)
                .Include(l => l.Enterprise)
                .Where(l => l.Status == LicenseStatus.Active
                            && l.ExpireDate != null
                            && l.ExpireDate >= today
                            && l.ExpireDate <= until)
                .OrderBy(l => l.ExpireDate)
                .ToList()
                .Select(l => new ExpiringLicenseReportRow
                {
                    Software = l.Software.Name,
                    Enterprise = l.Enterprise?.Name,
                    Supplier = l.Supplier?.Name,
                    Status = l.Status.ToString(),
                    KeyOrContract = l.KeyOrContract,
                    Seats = l.Seats,
                    Used = usedByLicense.TryGetValue(l.Id, out var u) ? u : 0,
                    ExpireDate = l.ExpireDate
                })
                .ToList();

            ExpiringLicenses = new ObservableCollection<ExpiringLicenseReportRow>(list);
        }

        [RelayCommand]
        public void BuildNoLicense()
        {
            using var db = new AppDbContext();

            var list = db.Installations.AsNoTracking()
                .Include(i => i.Device).ThenInclude(d => d.Enterprise)
                .Include(i => i.Software)
                .Include(i => i.InstalledByEmployee) // если ты добавил InstalledByEmployee
                .Where(i => i.LicenseId == null && i.Software.IsFree == false)
                .OrderBy(i => i.Device.InventoryNumber)
                .ThenBy(i => i.Software.Name)
                .ToList()
                .Select(i => new NoLicenseInstallationReportRow
                {
                    DeviceInv = i.Device.InventoryNumber,
                    DeviceHost = i.Device.Hostname,
                    Enterprise = i.Device.Enterprise?.Name,
                    Software = i.Software.Name,
                    InstallDate = i.InstallDate,
                    InstalledBy = i.InstalledByEmployee?.FullName
                })
                .ToList();

            NoLicenseInstallations = new ObservableCollection<NoLicenseInstallationReportRow>(list);
        }

        [RelayCommand]
        public void BuildUsage()
        {
            using var db = new AppDbContext();

            var usedByLicense = db.Installations.AsNoTracking()
                .Where(i => i.LicenseId != null)
                .GroupBy(i => i.LicenseId!.Value)
                .Select(g => new { LicenseId = g.Key, Used = g.Count() })
                .ToList()
                .ToDictionary(x => x.LicenseId, x => x.Used);

            var list = db.Licenses.AsNoTracking()
                .Include(l => l.Software)
                .Include(l => l.Enterprise)
                .OrderBy(l => l.Software.Name)
                .ToList()
                .Select(l => new LicenseUsageReportRow
                {
                    Software = l.Software.Name,
                    Enterprise = l.Enterprise?.Name,
                    KeyOrContract = l.KeyOrContract,
                    Seats = l.Seats,
                    Used = usedByLicense.TryGetValue(l.Id, out var u) ? u : 0,
                    ExpireDate = l.ExpireDate,
                    Status = l.Status.ToString()
                })
                .OrderByDescending(r => r.Over > 0)
                .ThenBy(r => r.Software)
                .ToList();

            LicenseUsage = new ObservableCollection<LicenseUsageReportRow>(list);
        }

        // ---------- CSV экспорт (без зависимостей) ----------
        [RelayCommand]
        public void ExportExpiringCsv()
            => ExportCsv("expiring_licenses.csv",
                "Enterprise,Software,Supplier,Status,KeyOrContract,Seats,Used,ExpireDate",
                ExpiringLicenses.Select(r => string.Join(",",
                    Csv(r.Enterprise), Csv(r.Software), Csv(r.Supplier), Csv(r.Status), Csv(r.KeyOrContract),
                    r.Seats, r.Used, Csv(r.ExpireDate?.ToString()))));

        [RelayCommand]
        public void ExportNoLicenseCsv()
            => ExportCsv("no_license_installations.csv",
                "Enterprise,DeviceInv,DeviceHost,Software,InstallDate,InstalledBy",
                NoLicenseInstallations.Select(r => string.Join(",",
                    Csv(r.Enterprise), Csv(r.DeviceInv), Csv(r.DeviceHost), Csv(r.Software),
                    Csv(r.InstallDate.ToString()), Csv(r.InstalledBy))));

        [RelayCommand]
        public void ExportUsageCsv()
            => ExportCsv("license_usage.csv",
                "Enterprise,Software,KeyOrContract,Status,Seats,Used,Over,ExpireDate",
                LicenseUsage.Select(r => string.Join(",",
                    Csv(r.Enterprise), Csv(r.Software), Csv(r.KeyOrContract), Csv(r.Status),
                    r.Seats, r.Used, r.Over, Csv(r.ExpireDate?.ToString()))));

        private void ExportCsv(string defaultName, string header, IEnumerable<string> lines)
        {
            var dlg = new SaveFileDialog
            {
                FileName = defaultName,
                Filter = "CSV files (*.csv)|*.csv",
                DefaultExt = ".csv"
            };
            if (dlg.ShowDialog() != true) return;

            var sb = new StringBuilder();
            sb.AppendLine(header);
            foreach (var line in lines) sb.AppendLine(line);

            File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show("Экспорт выполнен.", "Отчёт", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private static string Csv(string? s)
        {
            s ??= "";
            // экранирование CSV
            if (s.Contains('"') || s.Contains(',') || s.Contains('\n') || s.Contains('\r'))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        // ---------- Печать (можно сохранить в PDF через Microsoft Print to PDF) ----------
        [RelayCommand]
        public void PrintExpiring()
            => Print(BuildFlowDocumentForExpiring(), "Отчёт: истекающие лицензии");

        [RelayCommand]
        public void PrintNoLicense()
            => Print(BuildFlowDocumentForNoLicense(), "Отчёт: установки без лицензии");

        [RelayCommand]
        public void PrintUsage()
            => Print(BuildFlowDocumentForUsage(), "Отчёт: использование лицензий");

        private void Print(FlowDocument doc, string title)
        {
            var pd = new PrintDialog();
            if (pd.ShowDialog() != true) return;

            doc.PageWidth = pd.PrintableAreaWidth;
            doc.PageHeight = pd.PrintableAreaHeight;
            doc.PagePadding = new Thickness(40);
            doc.ColumnWidth = double.PositiveInfinity;

            var paginator = ((IDocumentPaginatorSource)doc).DocumentPaginator;
            pd.PrintDocument(paginator, title);
        }

        private FlowDocument BuildFlowDocumentForExpiring()
        {
            var doc = BaseDoc($"Истекающие лицензии (в течение {Days} дней)");
            var table = NewTable(new[] { "Предприятие", "ПО", "Поставщик", "Seats", "Used", "До", "Ключ" });

            foreach (var r in ExpiringLicenses)
                AddRow(table, r.Enterprise ?? "", r.Software, r.Supplier ?? "", r.Seats.ToString(), r.Used.ToString(),
                    r.ExpireDate?.ToString() ?? "", r.KeyOrContract ?? "");

            doc.Blocks.Add(table);
            return doc;
        }

        private FlowDocument BuildFlowDocumentForNoLicense()
        {
            var doc = BaseDoc("Установки без лицензии (для платного ПО)");
            var table = NewTable(new[] { "Предприятие", "Инв№", "Hostname", "ПО", "Дата", "Установил" });

            foreach (var r in NoLicenseInstallations)
                AddRow(table, r.Enterprise ?? "", r.DeviceInv, r.DeviceHost, r.Software, r.InstallDate.ToString(), r.InstalledBy ?? "");

            doc.Blocks.Add(table);
            return doc;
        }

        private FlowDocument BuildFlowDocumentForUsage()
        {
            var doc = BaseDoc("Использование лицензий (Seats/Used)");
            var table = NewTable(new[] { "Предприятие", "ПО", "Seats", "Used", "Over", "До", "Статус", "Ключ" });

            foreach (var r in LicenseUsage)
                AddRow(table, r.Enterprise ?? "", r.Software, r.Seats.ToString(), r.Used.ToString(), r.Over.ToString(),
                    r.ExpireDate?.ToString() ?? "", r.Status, r.KeyOrContract ?? "");

            doc.Blocks.Add(table);
            return doc;
        }

        private static FlowDocument BaseDoc(string title)
        {
            var doc = new FlowDocument();
            doc.Blocks.Add(new Paragraph(new Run(title))
            {
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 12)
            });
            doc.Blocks.Add(new Paragraph(new Run("Дата формирования: " + DateTime.Now.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture)))
            {
                Margin = new Thickness(0, 0, 0, 12)
            });
            return doc;
        }

        private static Table NewTable(string[] headers)
        {
            var table = new Table();
            for (int i = 0; i < headers.Length; i++) table.Columns.Add(new TableColumn());

            var rg = new TableRowGroup();
            table.RowGroups.Add(rg);

            var hr = new TableRow();
            rg.Rows.Add(hr);

            foreach (var h in headers)
                hr.Cells.Add(new TableCell(new Paragraph(new Run(h)) { FontWeight = FontWeights.Bold }));

            return table;
        }

        private static void AddRow(Table table, params string[] cells)
        {
            var row = new TableRow();
            table.RowGroups[0].Rows.Add(row);

            foreach (var c in cells)
                row.Cells.Add(new TableCell(new Paragraph(new Run(c))));
        }
    }
}
