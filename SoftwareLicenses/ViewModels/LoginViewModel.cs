using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SoftwareLicenses.Security;
using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SoftwareLicenses.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly AuthService _auth;

        [ObservableProperty] private string login = "";
        [ObservableProperty] private string password = "";
        [ObservableProperty] private string error = "";

        public event Action? LoginSucceeded;

        public LoginViewModel(AuthService auth)
        {
            _auth = auth;
        }

        [RelayCommand]
        private void LoginUser()
        {
            Error = "";

            if (_auth.Login(Login, Password))
                LoginSucceeded?.Invoke();
            else
                Error = "Неверный логин или пароль";
        }
    }
}
