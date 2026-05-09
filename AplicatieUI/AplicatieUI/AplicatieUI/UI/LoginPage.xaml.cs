namespace AplicatieUI.UI;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
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

        // TODO: inlocuieste cu apelul real catre ApiService.Login()
        await Task.Delay(1000);

        SetLoading(false);

        // TODO: dupa ce primesti JWT-ul, navigheaza catre DocumentListPage:
        // await Shell.Current.GoToAsync("//DocumentListPage");

        ShowSuccess("Login successful! (TODO: connect to API)");
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