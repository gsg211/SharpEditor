using AplicatieUI.Logica.Command;
using AplicatieUI.Logica.Memento;
using System;
using System.Data;

namespace AplicatieUI.UI;

public partial class DocumentEditorPage : ContentPage
{
    private AplicatieUI.Logica.Memento.Editor _editorState = new();


    private int _currentShareId;
    private int _currentVersion;



    private AutoSaveButton _autoSaveCmd;
    private SaveButton _saveCmd;
    private UndoButton _undoCmd;
    private RedoButton _redoCmd;

    public DocumentEditorPage()
    {
        InitializeComponent();

        _autoSaveCmd = new AutoSaveButton(_editorState);
        _saveCmd = new SaveButton(_editorState,_currentShareId,_currentVersion);
        _undoCmd = new UndoButton(_editorState);
        _redoCmd = new RedoButton(_editorState);
    }

    public void LoadDocument(int shareId, string title, string content, string permission, int version)
    {


        _currentShareId = shareId;
        _currentVersion = version;

        DocumentTitleLabel.Text = title;
        ContentEditor.Text = content;

        _saveCmd = new SaveButton(_editorState, _currentShareId, _currentVersion);

        if (permission == "ReadOnly")
        {
            ContentEditor.IsReadOnly = true;
            SaveButton.IsEnabled = false;
            SaveButton.BackgroundColor = Color.FromArgb("#252836");
            UndoButton.IsEnabled = false;
            RedoButton.IsEnabled = false;
        }

    }

    private void ContentEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        _editorState.Text = ContentEditor.Text;

        _autoSaveCmd.Execute();
    }
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        _editorState.Text = ContentEditor.Text;
        _editorState.DataAndTime = DateTime.Now.ToString();
        _editorState.NumeDocument = DocumentTitleLabel.Text;

        _saveCmd.Execute();
    }

    private void OnUndoClicked(object? sender, EventArgs e)
    {
        _undoCmd.Execute();

        ContentEditor.Text = _editorState.Text;
    }

    private void OnRedoClicked(object? sender, EventArgs e)
    {
        _redoCmd.Execute();

        ContentEditor.Text = _editorState.Text;
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

        _saveCmd.Execute();

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