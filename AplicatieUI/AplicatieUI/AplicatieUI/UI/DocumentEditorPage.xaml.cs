namespace AplicatieUI.UI;

public partial class DocumentEditorPage : ContentPage
{
    private Guid _shareId;
    private int _currentVersion = 1;
    private string _lastSavedContent = "";

    private Stack<string> _redoStack = new();

    public DocumentEditorPage()
    {
        InitializeComponent();
    }

    public void LoadDocument(Guid shareId, string title, string content, string permission, int version)
    {
        _shareId = shareId;
        _currentVersion = version;
        _lastSavedContent = content;

        DocumentTitleLabel.Text = title;
        PermissionLabel.Text = permission;
        ContentEditor.Text = content;

        if (permission == "ReadOnly")
        {
            ContentEditor.IsReadOnly = true;
            SaveButton.IsEnabled = false;
            SaveButton.BackgroundColor = Color.FromArgb("#252836");
            UndoButton.IsEnabled = false;
            RedoButton.IsEnabled = false;
        }

        _redoStack.Clear();
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var content = ContentEditor.Text ?? "";

        if (content == _lastSavedContent)
        {
            ShowStatus("No changes to save.", "#6B6B80");
            return;
        }

        // TODO: inlocuieste cu apelul real la API:
        // var rezultat = await ApiService.UpdateDocument(_shareId, content, _currentVersion);
        // PUT /shared-docs/{shareId}
        // Body: { content, version: _currentVersion }
        // Daca raspunsul e 409 Conflict → versiunea e in urma, afiseaza eroare
        // Daca raspunsul e 200 OK → actualizeaza _currentVersion cu versiunea noua din raspuns

        _lastSavedContent = content;
        _redoStack.Clear();

        ShowStatus("Saved!", "#4CAF50");
    }

    private void OnUndoClicked(object? sender, EventArgs e)
    {
        string text = ContentEditor.Text ?? "";

        if (string.IsNullOrWhiteSpace(text))
            return;

        _redoStack.Push(text);

        ContentEditor.Text = RemoveLastWord(text);
    }

    private void OnRedoClicked(object? sender, EventArgs e)
    {
        if (_redoStack.Count == 0)
            return;

        ContentEditor.Text = _redoStack.Pop();
    }

    private string RemoveLastWord(string text)
    {
        text = text.TrimEnd();

        if (text.Length == 0)
            return "";

        int lastSpace = text.LastIndexOf(' ');

        if (lastSpace == -1)
            return "";

        return text.Substring(0, lastSpace + 1);
    }

    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        var content = ContentEditor.Text ?? "";

        if (content != _lastSavedContent)
        {
            bool continua = await DisplayAlert(
                "Unsaved Changes",
                "You have unsaved changes. Close anyway?",
                "Close", "Cancel");

            if (!continua)
                return;
        }

        await Shell.Current.GoToAsync("///DocumentListPage");
    }

    private async void ShowStatus(string message, string culoare)
    {
        StatusLabel.Text = message;
        StatusLabel.TextColor = Color.FromArgb(culoare);
        StatusLabel.IsVisible = true;

        await Task.Delay(3000);

        StatusLabel.IsVisible = false;
    }
}