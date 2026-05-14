namespace AplicatieUI.UI;

public partial class DocumentListPage : ContentPage
{
    public class DocumentItem
    {
        public Guid ShareId { get; set; }
        public string Title { get; set; } = "";
        public string UpdatedAt { get; set; } = "";
        public string Permission { get; set; } = "";
    }

    private DocumentItem? _selectedDocument;

    public DocumentListPage()
    {
        InitializeComponent();
        LoadDocuments();
    }

    private void LoadDocuments()
    {
        // TODO: inlocuieste cu apelul real catre ApiService.GetSharedDocs()
        // GET /shared-docs

        var documente = new List<DocumentItem>
        {
            new DocumentItem
            {
                ShareId = Guid.NewGuid(),
                Title = "Proiect IP",
                UpdatedAt = "Updated 10 min ago",
                Permission = "Owner"
            },
            new DocumentItem
            {
                ShareId = Guid.NewGuid(),
                Title = "Notite curs",
                UpdatedAt = "Updated 2 hours ago",
                Permission = "ReadWrite"
            },
            new DocumentItem
            {
                ShareId = Guid.NewGuid(),
                Title = "Document partajat",
                UpdatedAt = "Updated yesterday",
                Permission = "ReadOnly"
            }
        };

        DocumentsCollection.ItemsSource = documente;
        SubtitleLabel.Text = $"{documente.Count} documents";
    }

    private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        _selectedDocument = e.CurrentSelection.FirstOrDefault() as DocumentItem;

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
        var editor = new DocumentEditorPage();
        editor.LoadDocument(
            Guid.Empty,
            "New Document",
            "",
            "Owner",
            1
        );

        await Navigation.PushAsync(editor);
    }


    private async void OnOpenClicked(object sender, EventArgs e)
    {
        if (_selectedDocument == null) return;

        var editor = new DocumentEditorPage();
        editor.LoadDocument(
            _selectedDocument.ShareId,
            _selectedDocument.Title,
            "Continut de test...",
            _selectedDocument.Permission,
            1
        );

        await Navigation.PushAsync(editor);
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_selectedDocument == null) return;

        bool confirmat = await DisplayAlert(
            "Delete Document",
            $"Esti sigur ca vrei sa stergi '{_selectedDocument.Title}'?",
            "Delete", "Cancel");

        if (!confirmat) return;

        // TODO: apel real catre ApiService.DeleteSharedDoc(_selectedDocument.ShareId)
        // DELETE /shared-docs/{shareId}

        LoadDocuments();
        _selectedDocument = null;
        OpenButton.IsEnabled = false;
        DeleteButton.IsEnabled = false;
    }
}