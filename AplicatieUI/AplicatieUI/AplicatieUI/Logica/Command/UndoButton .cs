using AplicatieUI.Logica.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    internal class UndoButton : ICommandButton
    {
        private readonly Memento.Editor _editor;

        public UndoButton(Memento.Editor editor)
        {
            _editor = editor;
        }

        public void Execute()
        {
            if (_editor._undoHistory.Count <= 1)
                return;

            _editor._undoHistory.Pop();

            var previous = _editor._undoHistory.Peek();

            _editor._redoHistory.Push(previous);

            _editor.Restore(previous);
        }
    }
}
