using AplicatieUI.Logica.Memento;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    internal class SaveButton : ICommandButton
    {
        private readonly Memento.Editor _editor;

        public SaveButton(Memento.Editor editor)
        {
            _editor = editor;
        }

        public void Execute()
        {
            _editor._undoHistory.Push(_editor.MakeSnapshot());
            Snapshot uploadSnapshot = _editor._undoHistory.Peek();
            string text = uploadSnapshot.Text;
            string dataAndTime = uploadSnapshot.DataAndTime;
            string numeDocument = uploadSnapshot.NumeDocument;



            //Apelare metoda incarcare in baza de date

        }
    }
}
