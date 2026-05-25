using AplicatieUI.Logica.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    internal class RedoButton : ICommandButton
    {
        private readonly Memento.Editor _editor;

        public RedoButton(Memento.Editor editor)
        {
            _editor = editor;
        }

        public void Execute()
        {
            if (_editor._redoHistory.Count <= 1)
                return;

            _editor._redoHistory.Pop();

            var previous = _editor._redoHistory.Peek();

            _editor.Restore(previous);
        }
    }
}
