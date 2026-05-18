using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Documente
{
    internal class ManagerDocument
    {
        public ObservableCollection<Document> Documents { get; set; }

        public ManagerDocument(ObservableCollection<Document> documents)
        {
            this.Documents = documents;
        }
    }
}
