/* 
 * Author: Miron Victor
 * Description:
 * Logic for the document editor implementing Command and Memento patterns. 
 * It handles document loading, user permissions (ReadOnly mode), and history navigation. 
 * It also triggers auto-save on every text change and ensures the document 
 * is saved before navigating back to the list.
 */

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



    /// <summary>
    /// Populates the editor with document data and configures access rights (Read-Only vs Full Edit).
    /// </summary>
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



    /// <summary>
    /// Monitors text changes and automatically triggers a snapshot for the Undo history.
    /// </summary>
    private void ContentEditor_TextChanged(object sender, TextChangedEventArgs e)
    {
        _editorState.Text = ContentEditor.Text;

        _autoSaveCmd.Execute();
    }




    /// <summary>
    /// Manually saves the current document state to the cloud and updates metadata.
    /// </summary>
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        _editorState.Text = ContentEditor.Text;
        _editorState.DataAndTime = DateTime.Now.ToString();
        _editorState.NumeDocument = DocumentTitleLabel.Text;

        _saveCmd.Execute();
    }




    /// <summary>
    /// Executes the undo command and updates the editor text.
    /// </summary>
    private void OnUndoClicked(object? sender, EventArgs e)
    {
        _undoCmd.Execute();

        ContentEditor.Text = _editorState.Text;
    }


    /// <summary>
    /// Executes the redo command and updates the editor text.
    /// </summary>
    private void OnRedoClicked(object? sender, EventArgs e)
    {
        _redoCmd.Execute();

        ContentEditor.Text = _editorState.Text;
    }


    /// <summary>
    /// Performs a final save of the current work and navigates back to the document list.
    /// </summary>
    private async void OnCloseClicked(object? sender, EventArgs e)
    {
        var content = ContentEditor.Text ?? "";

        _saveCmd.Execute();

        await Shell.Current.GoToAsync("///DocumentListPage");
    }
}