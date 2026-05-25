/* 
 * Author: Lionte Eduard-Iulian
 * Description:
 * The Originator class in the Memento pattern. 
 * It maintains the current editor state (text and metadata) and manages the Undo/Redo 
 * history stacks. It provides methods to capture the current state into a Snapshot 
 * and restore the editor's content from a previous snapshot.
 */


namespace AplicatieUI.Logica.Memento
{
    public class Editor
    {
        public string Text { get; set; } = "";

        public string NumeDocument { get; set; } = "";

        public string DataAndTime { get; set; } = "";

        public Stack<Snapshot> UndoHistory { get; set; } = new();
        public Stack<Snapshot> RedoHistory { get; set; } = new();



        /// <summary>
        /// Creates a new snapshot of the current editor state.
        /// </summary>
        public Snapshot MakeSnapshot()
        {
            return new Snapshot(Text,NumeDocument,DataAndTime);
        }



        /// <summary>
        /// Restores the editor text from a given snapshot.
        /// </summary>
        public void Restore(Snapshot snapshot)
        {
            if (snapshot != null)
                Text = snapshot.Text;
        }
    }
}