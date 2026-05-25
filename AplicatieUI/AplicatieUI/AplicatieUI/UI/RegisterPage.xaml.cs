/* 
 * Author: Miron Victor
 * Description:
 * Manages the registration logic by collecting user input and calling the SignUp service. 
 * It handles UI state transitions during the process (loading indicators, button toggling), 
 * displays validation or server errors, and navigates back to the Login page upon success.
 */


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



    /// <summary>
    /// Collects form data and invokes the sign-up service to create a new user account on the server.
    /// </summary>
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



    /// <summary>
    /// Navigates the user back to the login screen.
    /// </summary>
    private async void OnSignInTapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync("//LoginPage");
    }



    /// <summary>
    /// Displays an error message in the UI with the appropriate color coding.
    /// </summary>
    private void ShowError(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.TextColor = Color.FromArgb("#FF6B6B");
        ErrorLabel.IsVisible = true;
    }



    /// <summary>
    /// Displays an success message in the UI with the appropriate color coding.
    /// </summary>
    private void ShowSuccess(string message)
    {
        ErrorLabel.Text = message;
        ErrorLabel.TextColor = Color.FromArgb("#4CAF50");
        ErrorLabel.IsVisible = true;
    }



    /// <summary>
    /// Updates the UI to show or hide the loading state.
    /// </summary>
    private void SetLoading(bool isLoading)
    {
        RegisterButton.IsEnabled = !isLoading;
        RegisterButton.Text = isLoading ? "Creating account..." : "Create Account";
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        ErrorLabel.IsVisible = false;
    }
}