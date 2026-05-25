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


        
        //public Document(string titlu, string text, string permission)
        //{
        //    Id = Guid.NewGuid();
        //    Titlu = titlu;
        //    Text = text;
        //    DataAndTime = DateTime.Now.ToString();
        //    Version = 1;
        //    Permission = permission;
        //}

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
