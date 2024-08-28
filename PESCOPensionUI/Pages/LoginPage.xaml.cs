using PensionSystem.Entities.DTOs;

namespace PESCOPensionUI.Pages;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        DisplayAlert("radio Buttion", "hi", "Cancel");
    }

    private void LoginButton_Clicked(object sender, EventArgs e)
    {
        var login = new LoginDTO()
        {
            Email = EmailEntry.Text,
            Password = PasswordEntry.Text
        };
        if (string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
            DisplayAlert("Invalid Login", "Email & Password are required", "Ok");
        else
        {
            if (Application.Current != null)
            {
                var navPag = new HomePage();
                Application.Current.MainPage = navPag;
            }
        }
    }
}