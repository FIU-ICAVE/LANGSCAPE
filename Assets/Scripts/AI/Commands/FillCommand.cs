using UnityEngine;

class FillCommand : Command {
    private Vector3Int position;
    private Vector3Int size;
    private GridCellData cell;

    private GridCellData[,,] replace;

    // Fills the selected region with a specific block.
    // Syntax Descriptor: <position: p> <size: s> <block: 0-4> [color: hexadecimal]
    // f <x> <y> <z> <sx> <sy> <sz> <block> [color]
    public static new readonly char SIGNATURE = 'f';
    public static new readonly int REQUIRED_PARAMS = 7;

    public FillCommand(int[] argv, Color color) {
        position = new Vector3Int(argv[0], argv[1], argv[2]);
        size = new Vector3Int(argv[3], argv[4], argv[5]);
        cell = new GridCellData(argv[6], color);
        valid = LangscapeError.CMD_VALID.code;

        if (IsInvalidPosition(position, size))
        {
            ErrorInterface.Instance.ThrowUserError(LangscapeError.CMD_POSITION_OUT_OF_WORLD);
            return;
        }
    }
    public override void Execute() {

        //[[[TEST CODE]]]
        //LangscapeError.Instance.ThrowUserError(LangscapeError.CMD_POSITION_OUT_OF_WORLD);

        replace = GridMesh.Instance.Replace(position, size, cell);

        //updating world stats
        WorldStateManager.Instance.UpdateTotalWorldObjects();
    }
    public override void Undo() {
        GridMesh.Instance.Set(position, replace);
    }
    public override void Redo() { 
        GridMesh.Instance.Fill(position, size, cell);
    }

    public override string ToString() {
        return SIGNATURE +
            " <" + position.x + " " + position.y + " " + position.z + "> " +
            "<" + size.x + " " + size.y + " " + size.z + "> " +
            "<" + (int)cell.type + "> " +
            "<" + ColorUtility.ToHtmlStringRGB(cell.color) + ">";
    }
}