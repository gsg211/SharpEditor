using AplicatieUI.Logica.Documente;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    internal class NewButton : ICommandButton
    {
        private ManagerDocument _manager;
        public NewButton(ManagerDocument manager)
        {
            this._manager = manager;
        }
        public void Execute()
        {
            Document doc = new Document("Now","aaa" ,"All");

            _manager.Documents.Add(doc);
        }
    }
}
