/* 
 * Author: Miron Victor
 * Description:
 * Logic for the login screen that handles user authentication. 
 * It validates credentials, manages UI feedback (loading states and error messages), 
 * and controls navigation to the Document List or Register pages. 
 * Also includes a method to launch the external help documentation.
 */

using AplicatieUI.Logica.SignIn_SignUp;

namespace AplicatieUI.UI;

public partial class LoginPage : ContentPage
{
    private SignIn _signIn;

    /// <summary>
    /// Initializes the UI components and instantiates the sign-in logic handler.
    /// </summary>
    public LoginPage()
    {
        InitializeComponent();
        _signIn = new SignIn();
    }


    /// <summary>
    /// Handles the login process: validates input fields, activates the loading state, 
    /// and verifies credentials through the API.
    /// </summary>
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



    /// <summary>
    /// Navigates the user to the registration page (RegisterPage).
    /// </summary>
    private async void OnSignUpTapped(object sender, TappedEventArgs e)
    {
         await Shell.Current.GoToAsync("///RegisterPage");
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
    /// Updates the UI visual state during asynchronous operations by toggling buttons and the loading indicator.
    /// </summary>  
    private void SetLoading(bool isLoading)
    {
        LoginButton.IsEnabled = !isLoading;
        LoginButton.Text = isLoading ? "Signing in..." : "Sign In";
        LoadingIndicator.IsRunning = isLoading;
        LoadingIndicator.IsVisible = isLoading;
        ErrorLabel.IsVisible = false;
    }


    /// <summary>
    /// Launches the external help documentation file (.chm) from the application directory.
    /// </summary>
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