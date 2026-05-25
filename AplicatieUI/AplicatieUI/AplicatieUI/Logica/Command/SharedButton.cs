using AplicatieUI.Logica.API;
using AplicatieUI.Logica.Documente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    internal class ShareButton : ICommandButton
    {
        private readonly ApiService _apiService = new ApiService();
        private readonly Page _parentPage; 

        public Document SelectedDocument { get; set; }

        public ShareButton(Page parentPage)
        {
            _parentPage = parentPage;
        }

        public async void Execute()
        {
            if (SelectedDocument == null) return;

            string actiune = await _parentPage.DisplayActionSheet("Gestionare Acces", "Anulează", null,
                "Adaugă utilizator nou (ID)", "Elimină acces utilizator");

            if (actiune == "Adaugă utilizator nou (ID)")
            {
                await ExecuteAddFlow();
            }
            else if (actiune == "Elimină acces utilizator")
            {
                await ExecuteRemoveFlow();
            }
        }

        private async Task ExecuteAddFlow()
        {
            string idInput = await _parentPage.DisplayPromptAsync("Share", "Introdu ID-ul utilizatorului:");
            if (string.IsNullOrEmpty(idInput) || !int.TryParse(idInput, out int targetId)) return;

            string perm = await _parentPage.DisplayActionSheet("Permisiune", "Anulează", null, "ReadOnly", "ReadWrite");
            if (perm == "Anulează" || string.IsNullOrEmpty(perm)) return;

            var res = await _apiService.ShareDocumentAsync(SelectedDocument.Id, targetId, perm);
            await _parentPage.DisplayAlert("Info", res.Message, "OK");
        }

        private async Task ExecuteRemoveFlow()
        {
            var listaShares = await _apiService.GetDocumentSharesAsync(SelectedDocument.Id);

            if (listaShares == null || listaShares.Count == 0)
            {
                await _parentPage.DisplayAlert("Info", "Niciun alt utilizator nu are acces.", "OK");
                return;
            }

            var optiuni = listaShares.Select(s => $"User ID: {s.UserId} ({s.Permission})").ToArray();
            string selectat = await _parentPage.DisplayActionSheet("Elimină acces:", "Anulează", null, optiuni);

            if (selectat == "Anulează" || string.IsNullOrEmpty(selectat)) return;

            var shareSelectat = listaShares.FirstOrDefault(s => selectat.Contains($"ID: {s.UserId}"));

            if (shareSelectat != null)
            {
                bool confirm = await _parentPage.DisplayAlert("Confirmare", $"Elimini accesul utilizatorului {shareSelectat.UserId}?", "Elimină", "Anulează");
                if (confirm)
                {
                    bool succes = await _apiService.RevokeShareAsync(SelectedDocument.Id, shareSelectat.UserId);
                    if (succes) await _parentPage.DisplayAlert("Succes", "Acces revocat.", "OK");
                }
            }
        }
    }
}
