using AplicatieUI.Logica.Documente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    internal class DeleteButton : ICommandButton
    {
        private ManagerDocument _manager;

        public Document DocumentToDelete { get; set; }

        public DeleteButton(ManagerDocument manager)
        {
            this._manager = manager;
        }
        public async void Execute()
        {
            string token = "dbasyigdfibcadsadaskiufhas";

            string tokenPrim = await SecureStorage.Default.GetAsync("tekenulMeu");
            if (DocumentToDelete != null && tokenPrim == token)
            {
                _manager.Documents.Remove(DocumentToDelete);

                DocumentToDelete = null;
            }
        }
    }
}
