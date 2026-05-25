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

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadDocumentsFromServer();
    }


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


    private void LoadDocuments()
    {
        DocumentsCollection.ItemsSource = _manager.Documents;
        SubtitleLabel.Text = $"{documente.Count} manager.Documents";
    }

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

    private async void OnShareClicked(object sender, EventArgs e)
    {
        _shareButton.Execute();
    }


    


    private async void OnNewDocumentClicked(object sender, EventArgs e)
    {
        string title = await DisplayPromptAsync("Document Nou", "Introdu titlul documentului:", "Crează", "Anulează");

        if (string.IsNullOrWhiteSpace(title)) return;

        
        _newButton.TitleToCreate = title;

        
        _newButton.Execute();

    }


    private async void OnOpenClicked(object sender, EventArgs e)
    {
        if (_selectedDocument == null) return;

       
        var docComplet = await _apiService.GetSharedDocByIdAsync(_selectedDocument.Id);

      

        if (docComplet == null)
        {
            await DisplayAlert("Eroare", "Nu s-a putut încărca conținutul documentului.", "OK");
            return;
        }

        // 2. Deschidem editorul cu datele REALE
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