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

   private ObservableCollection<Document> documente = [];
    public DocumentListPage()
    {
        InitializeComponent();


        _manager = new ManagerDocument(documente);



        //Inlocuit cu apel API
        Document doc1 = new Document("d1", "Test1", "All");
        Document doc2 = new Document("d2", "Test2", "All");
        Document doc3 = new Document("d3", "Test3", "All");

        documente.Add(doc1);
        documente.Add(doc2);
        documente.Add(doc3);

        _newButton = new NewButton(_manager);
        _deleteButton = new DeleteButton(_manager);

        LoadDocuments();
    }

    private void LoadDocuments()
    {
        // TODO: inlocuieste cu apelul real catre ApiService.GetSharedDocs()
        // GET /shared-docs

        


        

        DocumentsCollection.ItemsSource = _manager.Documents;
        SubtitleLabel.Text = $"{documente.Count} manager.Documents";
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedDocument = e.CurrentSelection.FirstOrDefault() as Document;

        // Activeaza/dezactiveaza butoanele in functie de selectie
        bool areSelected = _selectedDocument != null;
        OpenButton.IsEnabled = areSelected;

        OpenButton.BackgroundColor = areSelected
         ? Color.FromArgb("#4F6EF7")
         : Color.FromArgb("#1A1A2E");
        OpenButton.TextColor = areSelected
            ? Colors.White
            : Color.FromArgb("#3A3A5E");

        DeleteButton.IsEnabled = areSelected;
        DeleteButton.BackgroundColor = areSelected
            ? Color.FromArgb("#2A1A1A")
            : Color.FromArgb("#1A1A1A");
        DeleteButton.TextColor = areSelected
            ? Color.FromArgb("#FF6B6B")
            : Color.FromArgb("#4A2A2A");
    }

    private async void OnNewDocumentClicked(object sender, EventArgs e)
    {
        _newButton.Execute();
        LoadDocuments();
    }


    private async void OnOpenClicked(object sender, EventArgs e)
    {
        if (_selectedDocument == null) return;

        Guid shar = Guid.Empty;

        var editor = new DocumentEditorPage();
        editor.LoadDocument(
            shar,
            _selectedDocument.Titlu,
            _selectedDocument.Text,
            "All",
            1
        );

        await Navigation.PushAsync(editor);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_selectedDocument == null) return;

        bool confirmat = await DisplayAlert(
            "Delete Document",
            $"Esti sigur ca vrei sa stergi '{_selectedDocument.Titlu}'?",
            "Delete", "Cancel");

        if (!confirmat) return;

        // TODO: apel real catre ApiService.DeleteSharedDoc(_selectedDocument.ShareId)
        // DELETE /shared-docs/{shareId}

        if (confirmat)
        {
            _deleteButton.DocumentToDelete = _selectedDocument;
            _deleteButton.Execute();
        }

        LoadDocuments();
        _selectedDocument = null;
        OpenButton.IsEnabled = false;
        DeleteButton.IsEnabled = false;
    }
}