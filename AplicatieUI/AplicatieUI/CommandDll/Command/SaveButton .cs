/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Implements the Command pattern for saving the document to the database. 
 * It pushes the current editor state to the Undo history and calls the ApiService 
 * to update the document content on the server, while also handling version 
 * increments upon a successful update.
 */


using AplicatieUI.Logica.API;
using AplicatieUI.Logica.Memento;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    public class SaveButton : ICommandButton
    {
        private readonly Memento.Editor _editor;
        private readonly ApiService _apiService = new ApiService();
        private readonly int _shareId;
        private int _version;

        public SaveButton(Memento.Editor editor, int shareId, int version)
        {
            _editor = editor;
            _shareId = shareId;
            _version = version;
        }

        public async void Execute()
        {
            _editor.UndoHistory.Push(_editor.MakeSnapshot());

            bool succes = await _apiService.UpdateDocumentAsync(_shareId, _editor.Text, _version);
            if (succes)
            {
                _version++; 
                Console.WriteLine("Salvat cu succes în cloud!");
            }
        }
    }
}
