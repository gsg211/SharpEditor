/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Implements the Redo command using the Command and Memento patterns. 
 * It enables navigating forward through the editor's history by popping states from the 
 * RedoHistory stack and restoring the editor to the next available snapshot.
 */


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
            if (_editor.RedoHistory.Count <= 1)
                return;

            _editor.RedoHistory.Pop();

            var previous = _editor.RedoHistory.Peek();

            _editor.Restore(previous);
        }
    }
}
