/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Logic for the document list explorer that syncs with the API. 
 * It handles fetching documents from the server, managing selection states, 
 * and updating UI controls based on user permissions (e.g., Owner vs Read-Only). 
 * Uses the Command pattern for file operations like creating, deleting, or sharing, 
 * and manages navigation to the DocumentEditorPage.
 */

using AplicatieUI.Logica.API;
using AplicatieUI.Logica.Command;
using AplicatieUI.Logica.Documente;
using System.Collections.ObjectModel;

namespace AplicatieUI.UI;

public partial class DocumentListPage : ContentPage
{
    private ManagerDocument _manager;
    private Document? _selectedDocument;
    private NewButton _newButton;
    private DeleteButton _deleteButton;
    private ShareButton _shareButton;


    private readonly ApiService _apiService = new ApiService();


    
    private ObservableCollection<Document> documente = [];

    public DocumentListPage()
    {
        InitializeComponent();

        _manager = new ManagerDocument(documente);

        _newButton = new NewButton(_manager);
        _deleteButton = new DeleteButton(_manager);
        _shareButton = new ShareButton(this);

        DocumentsCollection.ItemsSource = documente;
    }



    /// <summary>
    /// Triggered when the page is displayed; initiates a refresh of the document list from the server.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDocumentsFromServer();
    }


    /// <summary>
    /// Fetches the latest list of shared documents from the API and updates the local collection.
    /// </summary>
    private async Task LoadDocumentsFromServer()
    {
        var docsApi = await _apiService.GetSharedDocsAsync();
            
        documente.Clear();
        foreach (var d in docsApi)
        {
            var localDoc = new Document(d.ShareId, d.Title, "", 1, d.Permission);
            documente.Add(localDoc);
        }

        SubtitleLabel.Text = $"{documente.Count} documents found";
    }



    /// <summary>
    /// Manages document selection and toggles action buttons (Open, Share, Delete) based on user permissions.
    /// </summary>
    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedDocument = e.CurrentSelection.FirstOrDefault() as Document;

        // Activeaza/dezactiveaza butoanele in functie de selectie
        bool areSelected = _selectedDocument != null;

        bool isOwner = areSelected && _selectedDocument.Permission == "Owner";

        DeleteButton.IsEnabled = isOwner;
        OpenButton.IsEnabled = areSelected;



        
        OpenButton.BackgroundColor = areSelected
         ? Color.FromArgb("#4F6EF7")
         : Color.FromArgb("#1A1A2E");
        OpenButton.TextColor = areSelected
            ? Colors.White
            : Color.FromArgb("#3A3A5E");

        DeleteButton.BackgroundColor = isOwner
        ? Color.FromArgb("#FF6B6B")
        : Color.FromArgb("#1A1A1A");

        ShareButton.IsEnabled = isOwner;
        ShareButton.BackgroundColor = isOwner ? Color.FromArgb("#4CAF50") : Color.FromArgb("#1A1A2E");

        _shareButton.SelectedDocument = _selectedDocument;

    }



    /// <summary>
    /// Opens the management interface to add or revoke user access for the selected document.
    /// </summary>
    private async void OnShareClicked(object sender, EventArgs e)
    {
        _shareButton.Execute();
    }




    /// <summary>
    /// Prompts the user for a title and executes the command to create a new document on the server.
    /// </summary>
    private async void OnNewDocumentClicked(object sender, EventArgs e)
    {
        string title = await DisplayPromptAsync("Document Nou", "Introdu titlul documentului:", "Crează", "Anulează");

        if (string.IsNullOrWhiteSpace(title)) return;

        
        _newButton.TitleToCreate = title;

        
        _newButton.Execute();

    }


    /// <summary>
    /// Retrieves the full content of the selected document and opens the editor page.
    /// </summary>
    private async void OnOpenClicked(object sender, EventArgs e)
    {
        if (_selectedDocument == null) return;

       
        var docComplet = await _apiService.GetSharedDocByIdAsync(_selectedDocument.Id);

      

        if (docComplet == null)
        {
            await DisplayAlert("Eroare", "Nu s-a putut încărca conținutul documentului.", "OK");
            return;
        }

        var editor = new DocumentEditorPage();
        editor.LoadDocument(
            docComplet.ShareId,
            docComplet.Document.Title,
            docComplet.Document.Content,
            docComplet.Permission,
            docComplet.Document.Version
        );

        await Navigation.PushAsync(editor);
    }



    /// <summary>
    /// Confirms the deletion with the user and removes the document from both the server and the local list.
    /// </summary>
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_selectedDocument == null) return;

        bool confirmat = await DisplayAlert(
            "Delete", $"Sigur ștergi '{_selectedDocument.Titlu}'?", "Șterge", "Anulează");

        if (!confirmat) return;

        _deleteButton.DocumentToDelete = _selectedDocument;

        _deleteButton.Execute();

        await Task.Delay(500);
        await LoadDocumentsFromServer(); 

        _selectedDocument = null;
    }
}