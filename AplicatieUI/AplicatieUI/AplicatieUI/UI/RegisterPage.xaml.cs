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

        SetLoading(true);

        var rezultat = await _signUp.ExecutaInregistrare(username, email, password);

        SetLoading(false);

        if (rezultat.IsSuccess)
        {
            await DisplayAlert("Succes", rezultat.Message, "OK");
            await Shell.Current.GoToAsync("//LoginPage");
        }
        else
        {
            ShowError(rezultat.Message);
        }

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