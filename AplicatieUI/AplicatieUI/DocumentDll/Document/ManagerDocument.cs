/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * A management class that acts as a container for the document collection. 
 * It uses an ObservableCollection to ensure that any data changes made by commands 
 * (like adding or removing files) are automatically synchronized with the UI.
 */


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Documente
{
    public class ManagerDocument
    {
        public ObservableCollection<Document> Documents { get; set; }


        /// <summary>
        /// Initializes the manager with an existing document collection.
        /// </summary>
        public ManagerDocument(ObservableCollection<Document> documents)
        {
            this.Documents = documents;
        }
    }
}
