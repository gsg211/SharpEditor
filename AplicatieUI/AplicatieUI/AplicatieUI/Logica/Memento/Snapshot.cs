/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Represents the Memento object in the Memento pattern. 
 * It captures and stores a point-in-time state of the editor, including text, 
 * document title, and timestamp. It is designed to be immutable to ensure 
 * the integrity of the undo/redo history.
 */


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



        /// <summary>
        /// Initializes a new snapshot with the provided state data.
        /// </summary>
        public Snapshot(string text,string numeDocument = "",string dataAndTime = "")
        {
            this.Text = text;
            this.NumeDocument = numeDocument;
            this.DataAndTime = dataAndTime;

        }
    }
}
