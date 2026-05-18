namespace AplicatieUI.Logica.Memento
{
    internal class Editor
    {
        public string Text { get; set; } = "";

        public string NumeDocument { get; set; } = "";

        public string DataAndTime { get; set; } = "";

        public Stack<Snapshot> _undoHistory { get; set; } = new();
        public Stack<Snapshot> _redoHistory { get; set; } = new();

        public Snapshot MakeSnapshot()
        {
            return new Snapshot(Text,NumeDocument,DataAndTime);
        }

        public void Restore(Snapshot snapshot)
        {
            if (snapshot != null)
                Text = snapshot.Text;
        }
    }
}