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
        valid = CODE_VALID;
    }
    public override void Execute() {
        GridMesh.Instance.GetCount(id);
        //TODO: speak this to the player
    }
    public override void Undo() { }
    public override void Redo() { }

    public override string ToString() {
        return SIGNATURE +
        " <" + id + ">";
    }
}
