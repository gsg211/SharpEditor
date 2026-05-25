/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Implements the Undo command using the Command and Memento patterns. 
 * It allows the user to revert the editor to previous states by popping snapshots 
 * from the UndoHistory stack and restoring them, while also managing the RedoHistory 
 * to enable seamless navigation through document changes.
 */


using AplicatieUI.Logica.Memento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AplicatieUI.Logica.Command
{
    public class UndoButton : ICommandButton
    {
        private readonly Memento.Editor _editor;

        public UndoButton(Memento.Editor editor)
        {
            _editor = editor;
        }

        public void Execute()
        {
            if (_editor.UndoHistory.Count <= 1)
                return;

            _editor.UndoHistory.Pop();

            var previous = _editor.UndoHistory.Peek();

            _editor.RedoHistory.Push(previous);

            _editor.Restore(previous);
        }
    }
}
