/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * A core model class representing a document. 
 * It stores essential metadata such as the ID, title, text content, version number, 
 * and user permissions, serving as the primary data structure for 
 * passing information between the API and the UI.
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Documente
{
    internal class Document
    {
        public int Id { get; set; } 
        public string Titlu { get; set; }
        public string Text { get; set; }
        public string DataAndTime { get; set; }
        public int Version { get; set; } 
        public string Permission { get; set; }


        /// <summary>
        /// Initializes a new instance of the Document class.
        /// </summary>
        public Document(int id, string titlu, string text, int version, string permission)
        {
            Id = id;
            Titlu = titlu;
            Text = text;
            Version = version;
            Permission = permission;
        }
    }
}
