using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    internal class AutoSaveButton : ICommandButton
    {
        private readonly Memento.Editor _editor;

        public AutoSaveButton(Memento.Editor editor)
        {
            _editor = editor;
        }

        public void Execute()
        {
            _editor._undoHistory.Push(_editor.MakeSnapshot());
        }
    }
}
