using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Memento
{
    internal class Snapshot
    {
        public string Text { get; } 

        public string NumeDocument { get; }

        public string DataAndTime { get; }
        

        public Snapshot(string text,string numeDocument = "",string dataAndTime = "")
        {
            this.Text = text;
            this.NumeDocument = numeDocument;
            this.DataAndTime = dataAndTime;

        }
    }
}
