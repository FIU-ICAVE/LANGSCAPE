using UnityEngine;

class RedoCommand : Command {

    private int count;

    // Undoes the last batch of commands the AI returned.
    // Syntax Descriptor: <count> is the number of batches to undo.
    // v <count>
    public static new readonly char SIGNATURE = 'v';
    public static new readonly int REQUIRED_PARAMS = 1;

    public RedoCommand(int count) {
        this.count = count;
        valid = LangscapeError.CMD_VALID.code;
    }
    public override void Execute() {
        WorldStateManager.Instance.RedoCommandBatch(count);
    }
    public override void Undo() {
        
    }
    public override void Redo() {
        
    }

    public override string ToString() {
        return "" + SIGNATURE + " " + count;
    }
}