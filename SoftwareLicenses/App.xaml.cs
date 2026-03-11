using Microsoft.EntityFrameworkCore;
using SoftwareLicenses.Data;
using SoftwareLicenses.Models;
using SoftwareLicenses.Security;
using System.Configuration;
using System.Data;
using System.Windows;

namespace SoftwareLicenses
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            using (var db = new AppDbContext())
                db.Database.Migrate();
            using (var db = new AppDbContext())
            {
                db.Database.Migrate();

                if (!db.Accounts.Any())
                {
                    db.Accounts.AddRange(
                        new Account
                        {
                            Login = "admin",
                            PasswordHash = PasswordHasher.Hash("Admin_2026!"),
                            Role = UserRole.Admin,
                            IsActive = true
                        },
                        new Account
                        {
                            Login = "manager",
                            PasswordHash = PasswordHasher.Hash("Manager_2026!"),
                            Role = UserRole.Manager,
                            IsActive = true
                        },
                        new Account
                        {
                            Login = "tech",
                            PasswordHash = PasswordHasher.Hash("Tech_2026!"),
                            Role = UserRole.Technician,
                            IsActive = true
                        }
                    );
                    db.SaveChanges();
                }
            }
            var auth = new AuthService();

            var loginVm = new SoftwareLicenses.ViewModels.LoginViewModel(auth);
            var loginWin = new SoftwareLicenses.View.LoginWindow { DataContext = loginVm };

            loginVm.LoginSucceeded += () =>
            {
                var mainVm = new SoftwareLicenses.ViewModels.Main.MainViewModel(auth);
                var mainWin = new SoftwareLicenses.MainWindow { DataContext = mainVm };
                mainWin.Show();
                loginWin.Close();
            };

            loginWin.Show();
        }
    }

}
