/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Implements the Command pattern for creating a new document. 
 * It sends a creation request to the API and, upon receiving a successful response, 
 * maps the data into a local Document instance and adds it to the manager's collection 
 * to refresh the UI list.
 */


using AplicatieUI.Logica.API;
using AplicatieUI.Logica.Documente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    public class NewButton : ICommandButton
    {
        private ManagerDocument _manager;
        private readonly ApiService _apiService = new ApiService();

        public string TitleToCreate { get; set; } = "New Document";

        public NewButton(ManagerDocument manager)
        {
            this._manager = manager;
        }

        public async void Execute()
        {
            var result = await _apiService.CreateDocumentAsync(TitleToCreate, "Start typing here...");

            if (result != null)
            {
                Document doc = new Document(
                    result.ShareId,
                    result.Title ?? TitleToCreate,
                    result.Document?.Content ?? "",
                    1,
                    result.Permission ?? "Owner"
                );

                _manager.Documents.Add(doc);
            }
            else
            {
                Console.WriteLine("Eroare la crearea documentului pe server.");
            }
        }
    }
}
