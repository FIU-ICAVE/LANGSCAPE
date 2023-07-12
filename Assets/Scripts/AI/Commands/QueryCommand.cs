using Unity.VisualScripting;
using UnityEngine;

class QueryCommand : Command {
    private GridCellType[] id;

    // Fills the selected region with a specific block.
    // Syntax Descriptor: <id: int>
    // q <id1> ...
    public static new readonly char SIGNATURE = 'q';
    public static new readonly int REQUIRED_PARAMS = 1;

    public QueryCommand(int[] argv) {
        id = new GridCellType[argv.Length];
        for (int i = 0; i < argv.Length; i++) {
            id[i] = (GridCellType)argv[i];
        }
        valid = LangscapeError.CMD_VALID.code;
    }
    public override void Execute() {
        int[] counts = GridMesh.Instance.GetCount(id);
        //TODO: speak this to the player
        string speak = $"There are a total of {counts[0]} {id[0].ToString().ToLower()} blocks";
        for (int i = 1; i < counts.Length - 1; i++) {
            speak += $", {counts[i]} {id[i].ToString().ToLower()} blocks";
        }
        if (counts.Length > 1)
            speak += $"{((counts.Length == 2) ? "" : ",")} and {counts[counts.Length - 1]} {id[counts.Length - 1].ToString().ToLower()} blocks";
        speak += '.';
        //Debug.Log(speak);
        ErrorInterface.Instance.ThrowUserError(new LangscapeError(0, speak));
    }
    public override void Undo() { }
    public override void Redo() { }

    public override string ToString() {
        return SIGNATURE +
        " <" + id + ">";
    }
}
