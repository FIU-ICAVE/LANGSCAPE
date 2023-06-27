using UnityEngine;

class UndoCommand : Command {

    private int count;

    // Undoes the last batch of commands the AI returned.
    // Syntax Descriptor: <count> is the number of batches to undo.
    // u <count>
    public static new readonly char SIGNATURE = 'u';
    public static new readonly int REQUIRED_PARAMS = 1;

    public UndoCommand(int count) {
        this.count = count;
        valid = LangscapeError.LLM_INVALID_RESPONSE.code;
    }
    public override void Execute() {
        WorldStateManager.Instance.UndoCommandBatch(count);
    }
    public override void Undo() {
        // Can't undo an undo, that's called a redo stupid.
    }
    public override void Redo() {
        // This is awkward, the undo shouldn't even be available.
    }

    public override string ToString() {
        return "" + SIGNATURE + " " + count;
    }
}