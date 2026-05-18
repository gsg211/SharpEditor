using AplicatieUI.Logica.SignIn_SignUp;

namespace AplicatieUI.UI;

public partial class RegisterPage : ContentPage
{
    private SignUp _signUp;
    public RegisterPage()
    {
        InitializeComponent();
        _signUp = new SignUp();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        var username = UsernameEntry.Text?.Trim();
        var email = EmailEntry.Text?.Trim();
        var password = PasswordEntry.Text;
        var confirmPassword = ConfirmPasswordEntry.Text;

        // Validare locala
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            ShowError("Please fill in all fields.");
            return;
        }

        if (password != confirmPassword)
        {
            ShowError("Passwords do not match.");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters.");
            return;
        }

        SetLoading(true);

        // TODO: inlocuieste cu apelul real catre ApiService.Register()
  

        SetLoading(false);
        _signUp.Verificare();

        // TODO: dupa inregistrare, naviga catre LoginPage:

        await Shell.Current.GoToAsync("///DocumentListPage");

    }

    private async void OnSignInTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
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
        RegisterButton.IsEnabled = !isLoading;
        RegisterButton.Text = isLoading ? "Creating account..." : "Create Account";
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        ErrorLabel.IsVisible = false;
    }
}