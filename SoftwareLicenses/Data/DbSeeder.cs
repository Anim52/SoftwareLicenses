using SoftwareLicenses.Models;
using SoftwareLicenses.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace SoftwareLicenses.Data
{
    public static class DbSeeder
    {
        public static void Seed(AppDbContext db)
        {
            if (db.Softwares.Any()) return;

            var employees = new List<Employee>
{
    new() { FullName = "Морозов А.С.", Position = "Мастер по установке ПО", Department = "Выездная установка", Email = "a.morozov@it-ip.ru" },
    new() { FullName = "Григорьева О.В.", Position = "Менеджер по работе с клиентами", Department = "Администрирование", Email = "o.grigorieva@it-ip.ru" },
    new() { FullName = "Кузнецов Д.А.", Position = "Технический специалист", Department = "Удалённая поддержка", Email = "d.kuznetsov@it-ip.ru" },
    new() { FullName = "Соколова И.П.", Position = "Инженер по настройке", Department = "Конфигурация ПК", Email = "i.sokolova@it-ip.ru" },
    new() { FullName = "Лебедев Р.А.", Position = "Мастер по лицензиям", Department = "Сопровождение ПО", Email = "r.lebedev@it-ip.ru" },
    new() { FullName = "Виноградов Т.К.", Position = "Стажёр-установщик", Department = "Выездная установка", Email = "t.vinogradov@it-ip.ru" },
    new() { FullName = "Белов Е.Н.", Position = "Мастер по установке ПО", Department = "Выездная установка", Email = "e.belov@it-ip.ru" },
    new() { FullName = "Некрасова А.Ю.", Position = "Специалист техподдержки", Department = "Удалённая поддержка", Email = "a.nekrasova@it-ip.ru" },
    new() { FullName = "Тимофеев В.Р.", Position = "Инженер по лицензированию", Department = "Сопровождение ПО", Email = "v.timofeev@it-ip.ru" },
    new() { FullName = "Ермакова Л.С.", Position = "Диспетчер по выездам", Department = "Администрирование", Email = "l.ermakova@it-ip.ru" },
    new() { FullName = "Степанов Г.М.", Position = "Настройщик серверов", Department = "Конфигурация ПК", Email = "g.stepanov@it-ip.ru" }
};

            var suppliers = new List<Supplier>
{
    new() { Name = "ООО «ТехноСервис»", Inn = "7701234567", Contact = "Иванов П.А.", Phone = "+7 (903) 111-22-33", Email = "info@tehservice.ru" },
    new() { Name = "ИП Соколова М.И.", Inn = "7712345678", Contact = "Соколова М.И.", Phone = "+7 (916) 222-33-44", Email = "msokolova@mail.ru" },
    new() { Name = "ООО «Ромашка ЛТД»", Inn = "7723456789", Contact = "Петров С.В.", Phone = "+7 (909) 333-44-55", Email = "romashka@rashka.ru" },
    new() { Name = "АО «Автопартс»", Inn = "7734567890", Contact = "Кузнецов А.А.", Phone = "+7 (925) 444-55-66", Email = "kuznetsov@auto.ru" },
    new() { Name = "ООО «БизнесСофт»", Inn = "7745678901", Contact = "Морозова Т.Р.", Phone = "+7 (910) 555-66-77", Email = "soft@bsoft.ru" },
    new() { Name = "ИП Васильев Д.А.", Inn = "7756789012", Contact = "Васильев Д.А.", Phone = "+7 (919) 666-77-88", Email = "vasilyev@yandex.ru" },
    new() { Name = "ООО «Экомаркет»", Inn = "7767890123", Contact = "Новиков П.И.", Phone = "+7 (920) 777-88-99", Email = "info@ecomarket.ru" },
    new() { Name = "ЗАО «СтройИнвест»", Inn = "7778901234", Contact = "Григорьев Е.С.", Phone = "+7 (921) 888-99-00", Email = "grig@stroi.ru" },
    new() { Name = "ООО «МедиаГрупп»", Inn = "7789012345", Contact = "Белова А.В.", Phone = "+7 (922) 999-00-00", Email = "belova@medgroup.ru" },
    new() { Name = "ИП Тимофеев К.Н.", Inn = "7790123456", Contact = "Тимофеев К.Н.", Phone = "+7 (923) 000-11-22", Email = "timofeev@bk.ru" }
};

            var softwares = new List<Software>
{
    new() { Name = "Adobe CreativeCloud", Vendor = "Adobe", Version = "2025", IsFree = false },
    new() { Name = "JetBrains All Products Pack", Vendor = "JetBrains", Version = "2025.1", IsFree = false },
    new() { Name = "Microsoft 365 Business", Vendor = "Microsoft", Version = "Business", IsFree = false },
    new() { Name = "Figma Organization", Vendor = "Figma", Version = "Enterprise", IsFree = false },
    new() { Name = "Slack Pro", Vendor = "Slack Technologies", Version = "Pro", IsFree = false },
    new() { Name = "Zoom Business", Vendor = "Zoom", Version = "Business", IsFree = false },
    new() { Name = "Autodesk AutoCAD", Vendor = "Autodesk", Version = "2025", IsFree = false },
    new() { Name = "PandaDoc", Vendor = "PandaDoc", Version = "2025", IsFree = false },
    new() { Name = "Kaspersky Endpoint Security", Vendor = "Kaspersky", Version = "12.0", IsFree = false },
    new() { Name = "LibreOffice", Vendor = "The Document Foundation", Version = "7.6", IsFree = true },
    new() { Name = "GIMP", Vendor = "GIMP Team", Version = "2.10", IsFree = true },
    new() { Name = "Ubuntu Server", Vendor = "Canonical", Version = "22.04 LTS", IsFree = true },
    new() { Name = "PostgreSQL", Vendor = "PostgreSQL Global Development Group", Version = "16.2", IsFree = true },
    new() { Name = "Firefox", Vendor = "Mozilla", Version = "115.0", IsFree = true }
};

            db.Employees.AddRange(employees);
            db.Suppliers.AddRange(suppliers);
            db.Softwares.AddRange(softwares);
            db.SaveChanges();

            var licenses = new List<License>
{
    new() { SoftwareId = softwares[0].Id, SupplierId = suppliers[0].Id, Type = LicenseType.Subscription, Status = LicenseStatus.Active, KeyOrContract = "ADB-3M4N-5O6P", Seats = 12, PurchaseDate = new DateOnly(2025, 1, 10), ExpireDate = new DateOnly(2026, 1, 10), Cost = 75000 },
    new() { SoftwareId = softwares[1].Id, SupplierId = suppliers[1].Id, Type = LicenseType.Subscription, Status = LicenseStatus.Active, KeyOrContract = "JB-2025-8A9B", Seats = 8, PurchaseDate = new DateOnly(2025, 2, 14), ExpireDate = new DateOnly(2026, 2, 14), Cost = 68000 },
    new() { SoftwareId = softwares[2].Id, SupplierId = suppliers[4].Id, Type = LicenseType.PerUser, Status = LicenseStatus.Active, KeyOrContract = "OFF21-5E6F-7G8H", Seats = 25, PurchaseDate = new DateOnly(2025, 3, 1), ExpireDate = new DateOnly(2026, 3, 1), Cost = 82000 },
    new() { SoftwareId = softwares[3].Id, SupplierId = suppliers[8].Id, Type = LicenseType.PerDevice, Status = LicenseStatus.Expired, KeyOrContract = "FIG-TRIAL-2025", Seats = 15, PurchaseDate = new DateOnly(2025, 4, 20), ExpireDate = new DateOnly(2025, 4, 20), Cost = 0 },
    new() { SoftwareId = softwares[4].Id, SupplierId = suppliers[2].Id, Type = LicenseType.Subscription, Status = LicenseStatus.Active, KeyOrContract = "SLK-PRO-2025", Seats = 30, PurchaseDate = new DateOnly(2025, 5, 5), ExpireDate = new DateOnly(2026, 5, 5), Cost = 45000 },
    new() { SoftwareId = softwares[5].Id, SupplierId = suppliers[3].Id, Type = LicenseType.Subscription, Status = LicenseStatus.Expired, KeyOrContract = "ZOOM-BUS-2024", Seats = 10, PurchaseDate = new DateOnly(2024, 6, 11), ExpireDate = new DateOnly(2025, 6, 11), Cost = 26000 },
    new() { SoftwareId = softwares[6].Id, SupplierId = suppliers[7].Id, Type = LicenseType.PerDevice, Status = LicenseStatus.Active, KeyOrContract = "AUTO-9C0D", Seats = 6, PurchaseDate = new DateOnly(2025, 1, 15), ExpireDate = new DateOnly(2026, 1, 15), Cost = 110000 },
    new() { SoftwareId = softwares[7].Id, SupplierId = suppliers[6].Id, Type = LicenseType.Subscription, Status = LicenseStatus.Active, KeyOrContract = "PDOC-2025-77", Seats = 14, PurchaseDate = new DateOnly(2025, 2, 22), ExpireDate = new DateOnly(2026, 2, 22), Cost = 34000 },
    new() { SoftwareId = softwares[8].Id, SupplierId = suppliers[0].Id, Type = LicenseType.Volume, Status = LicenseStatus.Active, KeyOrContract = "KAS-901Ж", Seats = 40, PurchaseDate = new DateOnly(2025, 4, 10), ExpireDate = new DateOnly(2026, 4, 10), Cost = 52000 }
};

            db.Licenses.AddRange(licenses);
            db.SaveChanges();

            var devices = new List<Device>
{
    new() { InventoryNumber = "IT-101", Hostname = "design-ws-01", SerialNumber = "SN-101", OperatingSystem = "Windows 11 Pro", Location = "Отдел дизайна, каб. 12", ResponsibleEmployeeId = employees[0].Id },
    new() { InventoryNumber = "IT-102", Hostname = "dev-mb-03", SerialNumber = "SN-102", OperatingSystem = "macOS Ventura", Location = "Отдел разработки, каб. 8", ResponsibleEmployeeId = employees[1].Id },
    new() { InventoryNumber = "IT-103", Hostname = "office-pc-12", SerialNumber = "SN-103", OperatingSystem = "Ubuntu 22.04", Location = "Бухгалтерия, каб. 5", ResponsibleEmployeeId = employees[2].Id },
    new() { InventoryNumber = "IT-104", Hostname = "render-node-07", SerialNumber = "SN-104", OperatingSystem = "Windows 10 Pro", Location = "Рендер-ферма, серверная", ResponsibleEmployeeId = employees[3].Id },
    new() { InventoryNumber = "IT-105", Hostname = "manager-lap-02", SerialNumber = "SN-105", OperatingSystem = "Windows 11 Pro", Location = "Отдел продаж, каб. 15", ResponsibleEmployeeId = employees[4].Id },
    new() { InventoryNumber = "IT-106", Hostname = "test-lin-04", SerialNumber = "SN-106", OperatingSystem = "Debian 12", Location = "Лаборатория тестирования", ResponsibleEmployeeId = employees[5].Id },
    new() { InventoryNumber = "IT-107", Hostname = "sales-lap-03", SerialNumber = "SN-107", OperatingSystem = "Windows 11 Pro", Location = "Отдел продаж, каб. 16", ResponsibleEmployeeId = employees[6].Id },
    new() { InventoryNumber = "IT-108", Hostname = "backup-srv-01", SerialNumber = "SN-108", OperatingSystem = "Ubuntu Server 22.04", Location = "Серверная, стойка 4", ResponsibleEmployeeId = employees[7].Id },
    new() { InventoryNumber = "IT-109", Hostname = "qa-mac-02", SerialNumber = "SN-109", OperatingSystem = "macOS Sonoma", Location = "Лаборатория тестирования", ResponsibleEmployeeId = employees[8].Id }
};

            db.Devices.AddRange(devices);
            db.SaveChanges();

            db.Installations.AddRange(
    new Installation
    {
        DeviceId = devices[0].Id,
        SoftwareId = softwares[0].Id,
        LicenseId = licenses[0].Id,
        InstalledByEmployeeId = employees[0].Id,
        InstalledBy = employees[0].FullName,
        InstalledVersion = "2025",
        InstallDate = new DateOnly(2025, 1, 10),
        Notes = "Установка для отдела дизайна"
    },
    new Installation
    {
        DeviceId = devices[1].Id,
        SoftwareId = softwares[1].Id,
        LicenseId = licenses[1].Id,
        InstalledByEmployeeId = employees[1].Id,
        InstalledBy = employees[1].FullName,
        InstalledVersion = "2025.1",
        InstallDate = new DateOnly(2025, 2, 15),
        Notes = "ПО для разработки"
    },
    new Installation
    {
        DeviceId = devices[2].Id,
        SoftwareId = softwares[9].Id, // LibreOffice
        LicenseId = null,
        InstalledByEmployeeId = employees[2].Id,
        InstalledBy = employees[2].FullName,
        InstalledVersion = "7.6",
        InstallDate = new DateOnly(2025, 3, 20),
        Notes = "Свободное ПО"
    },
    new Installation
    {
        DeviceId = devices[3].Id,
        SoftwareId = softwares[8].Id, // Kaspersky
        LicenseId = licenses[8].Id,
        InstalledByEmployeeId = employees[3].Id,
        InstalledBy = employees[3].FullName,
        InstalledVersion = "12.0",
        InstallDate = new DateOnly(2025, 4, 5),
        Notes = "Антивирусная защита"
    },
    new Installation
    {
        DeviceId = devices[4].Id,
        SoftwareId = softwares[2].Id, // Microsoft 365
        LicenseId = licenses[2].Id,
        InstalledByEmployeeId = employees[4].Id,
        InstalledBy = employees[4].FullName,
        InstalledVersion = "Business",
        InstallDate = new DateOnly(2025, 5, 12),
        Notes = "Офисный пакет"
    },
    new Installation
    {
        DeviceId = devices[5].Id,
        SoftwareId = softwares[12].Id, // PostgreSQL
        LicenseId = null,
        InstalledByEmployeeId = employees[5].Id,
        InstalledBy = employees[5].FullName,
        InstalledVersion = "16.2",
        InstallDate = new DateOnly(2025, 6, 18),
        Notes = "СУБД для тестов"
    },
    new Installation
    {
        DeviceId = devices[6].Id,
        SoftwareId = softwares[4].Id, // Slack
        LicenseId = licenses[4].Id,
        InstalledByEmployeeId = employees[6].Id,
        InstalledBy = employees[6].FullName,
        InstalledVersion = "Pro",
        InstallDate = new DateOnly(2025, 6, 25),
        Notes = "Коммуникационный сервис"
    },
    new Installation
    {
        DeviceId = devices[7].Id,
        SoftwareId = softwares[11].Id, // Ubuntu Server
        LicenseId = null,
        InstalledByEmployeeId = employees[7].Id,
        InstalledBy = employees[7].FullName,
        InstalledVersion = "22.04",
        InstallDate = new DateOnly(2025, 7, 5),
        Notes = "Сервер резервного копирования"
    },
    new Installation
    {
        DeviceId = devices[8].Id,
        SoftwareId = softwares[13].Id, // Firefox
        LicenseId = null,
        InstalledByEmployeeId = employees[8].Id,
        InstalledBy = employees[8].FullName,
        InstalledVersion = "115.0",
        InstallDate = new DateOnly(2025, 7, 12),
        Notes = "Браузер"
    }
);

           

            db.SaveChanges();

            db.Accounts.AddRange(
     new Account { Login = "admin", PasswordHash = PasswordHasher.Hash("Admin_2026!"), Role = UserRole.Admin, IsActive = true, EmployeeId = employees[0].Id },
     new Account { Login = "a.morozov", PasswordHash = PasswordHasher.Hash("123456"), Role = UserRole.Manager, IsActive = true, EmployeeId = employees[0].Id },
     new Account { Login = "o.grigorieva", PasswordHash = PasswordHasher.Hash("123456"), Role = UserRole.Manager, IsActive = true, EmployeeId = employees[1].Id },
     new Account { Login = "d.kuznetsov", PasswordHash = PasswordHasher.Hash("123456"), Role = UserRole.Manager, IsActive = true, EmployeeId = employees[2].Id },
     new Account { Login = "i.sokolova", PasswordHash = PasswordHasher.Hash("123456"), Role = UserRole.Technician, IsActive = true, EmployeeId = employees[3].Id },
     new Account { Login = "r.lebedev", PasswordHash = PasswordHasher.Hash("123456"), Role = UserRole.Technician, IsActive = true, EmployeeId = employees[4].Id }
 );

            db.SaveChanges();

            db.SaveChanges();
        }
    }
}
