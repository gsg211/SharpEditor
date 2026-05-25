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
        var email = UsernameEntry.Text?.Trim();
        var password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("Please fill in all fields.");
            return;
        }

        SetLoading(true);


        var rezultat = await _signIn.Verificare(email, password);


        SetLoading(false);

        if (rezultat.IsSuccess)
        {
            await Shell.Current.GoToAsync("///DocumentListPage");
        }
        else
        {
            ShowError(rezultat.Message);
        }


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

    private void OnHelpClicked(object sender, EventArgs e)
    {
        var helpPath = Path.Combine(AppContext.BaseDirectory, "SharpEditorHelp.chm");
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = helpPath,
            UseShellExecute = true
        });
    }
}