/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Implements the Command pattern for deleting a document. 
 * It communicates with the API to remove the file from the server and, upon success, 
 * updates the local collection by removing the document from the ManagerDocument list.
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
    public class DeleteButton : ICommandButton
    {
        private ManagerDocument _manager;
        private readonly ApiService _apiService = new ApiService();
        public Document DocumentToDelete { get; set; }

        public DeleteButton(ManagerDocument manager) { _manager = manager; }

        public async void Execute()
        {
            if (DocumentToDelete == null) return;

            var result = await _apiService.DeleteDocumentAsync(DocumentToDelete.Id);

            if (result.IsSuccess)
            {
                _manager.Documents.Remove(DocumentToDelete);
            }
        }
    }
}
