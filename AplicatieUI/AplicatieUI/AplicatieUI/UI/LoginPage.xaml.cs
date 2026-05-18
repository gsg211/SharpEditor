using AplicatieUI.Logica.SignIn_SignUp;

namespace AplicatieUI.UI;

public partial class LoginPage : ContentPage
{
    private SignIn _signIn;
    public LoginPage()
    {
        InitializeComponent();
        _signIn = new SignIn();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ShowError("Please fill in all fields.");
            return;
        }

        SetLoading(true);


        bool esteValid = await _signIn.Verificare(UsernameEntry.Text, PasswordEntry.Text);
        // TODO: inlocuieste cu apelul real catre ApiService.Login()
        if (esteValid)
        {
            SetLoading(false);
            await Shell.Current.GoToAsync("///DocumentListPage");
        }
        else
        {
            SetLoading(false);
            ShowError("User or passord is incorect!");
            return;
        }
        

        // TODO: dupa ce primesti JWT-ul, navigheaza catre DocumentListPage:
        
        
        

        
    }

    private async void OnSignUpTapped(object sender, TappedEventArgs e)
    {
        
         await Shell.Current.GoToAsync("///RegisterPage");

       
    }

    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.TextColor = Color.FromArgb("#FF6B6B");
        ErrorLabel.IsVisible = true;
    }

    private void ShowSuccess(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.TextColor = Color.FromArgb("#4CAF50");
        ErrorLabel.IsVisible = true;
    }

    private void SetLoading(bool isLoading)
    {
        LoginButton.IsEnabled = !isLoading;
        LoginButton.Text = isLoading ? "Signing in..." : "Sign In";
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        ErrorLabel.IsVisible = false;
    }
}