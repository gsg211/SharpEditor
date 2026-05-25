/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * Implements the Command pattern for the auto-save functionality within the undo system. 
 * Each execution triggers a snapshot of the current editor state and pushes it 
 * onto the Undo history stack, allowing the application to track text changes in real-time.
 */

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
            _editor.UndoHistory.Push(_editor.MakeSnapshot());
        }
    }
}
