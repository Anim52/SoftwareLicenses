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
            {
                db.Database.Migrate();
                DbSeeder.Seed(db);
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
